using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>, IDamageable, IAttacker
{
    [Header("Statics")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _cam;
    [SerializeField] private AudioSource _jetpackBubbleSFX;
    [SerializeField] private AudioSource _cluckSFX;
    [SerializeField] private AudioSource _quackSFX;
    [SerializeField] private AudioSource _takeDamageSFX;
    [SerializeField] private Transform _dartSpawnPoint;
    [SerializeField] private AudioSource _shootDartSFX;
    [Header("Movement Properties")]
    [Min(0)]
    [SerializeField] private float jetpackAcceleration = 1;
    [SerializeField] private float pitchRotationSpeed = 1;
    [SerializeField] private float yawRotationSpeed = 1;
    [SerializeField] private float rollRotationSpeed = 1;
    [Min(0)]
    [SerializeField] private float maxVelocity = 1;
    [SerializeField] private float rollAccelerationBoost = 1;
    [SerializeField] private float rollSpeedBoost = 1;
    [SerializeField] private float rollCameraForwardOffset;
    [SerializeField] private float cameraNormalFOV = 60;
    [SerializeField] private float cameraRollFOV = 50;
    [SerializeField] private float _minCluckQuackDelay = 5;
    [SerializeField] private float _maxCluckQuackDelay = 15;
    [Header("Weapon Properties")]
    [SerializeField] private Rigidbody _dartPrefab;
    [SerializeField] private float _initialDartVelocity = 1;
    [SerializeField] private float _initialDartSpin = 1;
    [SerializeField] private LayerMask _weaponHitCheckLayerMask;
    [Range(0f, 1f)]
    [SerializeField] private float _rangeInFrontOfNozzleForAimAssist = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _playerVelocityInfluenceOverWeaponTrajectories = 0.2f;
    [SerializeField] private float _minDartShootPitch = 1.44f;
    [SerializeField] private float _maxDartShootPitch = 1.44f;

    public Camera Cam => _cam;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private Vector3 originLocalCameraLocalPos;
    private bool dying = false;
    private float cluckQuackTimer = 0;

    protected override void Awake()
    {
        base.Awake();

        originLocalCameraLocalPos = _cam.transform.localPosition;
        cluckQuackTimer = _maxCluckQuackDelay;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        lookInput = new Vector2(value.Get<Vector2>().x, value.Get<Vector2>().y);
    }

    private void OnFireWeapon(InputValue value)
    {
        Attack();
    }

    private void FixedUpdate()
    {
        if (dying)
            return;

        // Apply movement input
        Vector3 forwardMoveInput = transform.forward * moveInput.y * (jetpackAcceleration + (moveInput.x != 0 ? rollAccelerationBoost : 0)) * Time.deltaTime;
        _rb.AddForce(forwardMoveInput, ForceMode.Acceleration);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity + (moveInput.x != 0 ? rollSpeedBoost : 0));
    }

    private void Update()
    {
        if (dying)
            return;

        // Rotate pitch, yaw, and roll
        transform.Rotate(new Vector3(-lookInput.y * pitchRotationSpeed, lookInput.x * yawRotationSpeed, -moveInput.x * rollRotationSpeed * Time.deltaTime), Space.Self);

        // Camera zoom if rolling
        _cam.transform.localPosition = originLocalCameraLocalPos + (Vector3.forward * (moveInput.x != 0 ? rollCameraForwardOffset : 0));
        _cam.fieldOfView = moveInput.x != 0 ? cameraRollFOV : cameraNormalFOV;

        ProcessAudio();
    }

    public void TryAttack()
    {
        // Needed?
    }

    public void Attack()
    {
        if (dying)
            return;

        // Try spawn dart
        if (_dartPrefab == null)
            return;

        Rigidbody newDart = Instantiate(_dartPrefab, _dartSpawnPoint.position, Quaternion.LookRotation(_dartSpawnPoint.forward, transform.up));

        // Calculate dart trajectory towards first impact point
        // If nothing hit in-front of nozzle, return shooting along my default trajectory based on spawn point forward
        Vector3 dartForwardVelocity = _dartSpawnPoint.forward * _initialDartVelocity;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, 1000, _weaponHitCheckLayerMask))
        {
            if (Vector3.Dot(_dartSpawnPoint.forward, (hit.point - _dartSpawnPoint.position)) > _rangeInFrontOfNozzleForAimAssist)
            {
                // Otherwise if (closest target in-front of nozzle is found, aim at that target
                dartForwardVelocity = (hit.point - _dartSpawnPoint.position).normalized * _initialDartVelocity;
            }
        }

        // Add slight influence from player's current velocity (in RightUp planes only) to trajectory velocity
        Vector3 playerRightUpVelcoity = _rb.velocity - _dartSpawnPoint.forward;
        Vector3 playerVelocityInfluence = playerRightUpVelcoity * _playerVelocityInfluenceOverWeaponTrajectories;

        newDart.AddForce(dartForwardVelocity + playerVelocityInfluence, ForceMode.Impulse);
        newDart.AddTorque(dartForwardVelocity.normalized * _initialDartSpin, ForceMode.Impulse);

        _shootDartSFX.pitch = Random.Range(_minDartShootPitch, _maxDartShootPitch);
        _shootDartSFX.Play();
    }

    private void ProcessAudio()
    {
        // Movement SFX
        if (moveInput != Vector2.zero && !_jetpackBubbleSFX.isPlaying)
            _jetpackBubbleSFX.Play();
        else if (moveInput == Vector2.zero && _jetpackBubbleSFX.isPlaying)
            _jetpackBubbleSFX.Stop();

        // Clucks & Quacks
        cluckQuackTimer -= Time.deltaTime;
        if (cluckQuackTimer <= 0)
        {
            cluckQuackTimer = Random.Range(_minCluckQuackDelay, _maxCluckQuackDelay);
            switch (Random.Range(0, 3))
            {
                case 0:
                case 1:
                    _cluckSFX.Play();
                    break;
                case 2:
                    _quackSFX.Play();
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (dying)
            return;

        Debug.Log("Player TakeDamage!");

        for (int i = 0; i < damage; i++)
        {
            UI.Instance.Damage();

            _takeDamageSFX.Play();
        }
    }

    public void OnDeath()
    {
        dying = true;

        // Play death sound
        _quackSFX.Play();

        // Trigger end of game cut-scene
        GameManager.Instance.FailGame();
    }
}

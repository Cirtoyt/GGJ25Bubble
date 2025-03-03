using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>, IDamageable, IAttacker
{
    [Header("Statics")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera _cam;
    [SerializeField] private ParticleSystem _jetpackBubblePS;
    [SerializeField] private AudioSource _jetpackBubbleSFX;
    [SerializeField] private AudioSource _cluckSFX;
    [SerializeField] private AudioSource _quackSFX;
    [SerializeField] private AudioSource _takeDamageSFX;
    [SerializeField] private Transform _dartSpawnPoint;
    [SerializeField] private AudioSource _shootDartSFX;
    [SerializeField] private GameObject _heldHarpoonVisual;
    [SerializeField] private Transform _harpoonSpawnPoint;
    [SerializeField] private AudioSource _shootHarpoonSFX;
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
    [SerializeField] private LayerMask _weaponHitCheckLayerMask;
    [Range(0f, 1f)]
    [SerializeField] private float _rangeInFrontOfNozzleForAimAssist = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _playerVelocityInfluenceOverWeaponTrajectories = 0.2f;
    [SerializeField] private Rigidbody _dartPrefab;
    [SerializeField] private float _initialDartVelocity = 1;
    [SerializeField] private float _initialDartSpin = 1;
    [SerializeField] private float _minDartShootPitch = 1.44f;
    [SerializeField] private float _maxDartShootPitch = 1.44f;
    [SerializeField] private Rigidbody _harpoonPrefab;
    [SerializeField] private float _initialHarpoonVelocity = 1;
    [SerializeField] private float _minHarpoonShootPitch = 1.44f;
    [SerializeField] private float _maxHarpoonShootPitch = 1.44f;
    [SerializeField] private Rigidbody _shurikenPrefab;
    [Min(1)]
    [SerializeField] private int _shurikenCount = 6;
    [SerializeField] private float _initialShurikenVelocity = 1;
    [SerializeField] private float _initialShurikenSpin = 1;
    [Range(0, 180)]
    [SerializeField] private float _initialShurikenTiltAngleOffset = 10;
    [Range(-180, 180)]
    [SerializeField] private float _initialShurikenMinSpreadAngleOffset = 0;
    [Range(-180, 180)]
    [SerializeField] private float _initialShurikenMaxSpreadAngleOffset = 0;
    [SerializeField] private float _shurikenSphereCastRadius = 2;
    [SerializeField] private int _shurikenDamage = 5;

    public Camera Cam => _cam;
    public GameObject HeldHarpoonVisual => _heldHarpoonVisual;

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

        HeldHarpoonVisual.SetActive(false);

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

    private void OnFireWeapon()
    {
        Attack();
    }

    private void OnFireShuriken()
    {
        if (dying || UI.Instance.GetAmmoForPowerUp(Weapon.WeaponType.Shuriken) <= 0)
            return;

        // Try spawn harpoon
        if (_harpoonPrefab == null)
            return;

        for (int i = 0; i < _shurikenCount; i++)
        {
            Rigidbody newWeapon = Instantiate(_shurikenPrefab, transform.position, Quaternion.LookRotation(transform.forward, transform.up));
            newWeapon.name = _shurikenPrefab.name;

            // Calculate rotation around player 360 degrees
            float forwardDegreesOffsetMultiplier = 360f * ((i + 1f) / (float)_shurikenCount);
            Vector3 weaponForwardVelocity = transform.forward * _initialShurikenVelocity;
            Quaternion playerYAxisRotation = Quaternion.AngleAxis(forwardDegreesOffsetMultiplier, transform.up);

            // Calculate randomised up/down angle offset
            float spreadRotation = Random.Range(_initialShurikenMinSpreadAngleOffset, _initialShurikenMaxSpreadAngleOffset);
            Quaternion playerXAxisRotation = Quaternion.AngleAxis(spreadRotation, transform.right);

            // Apply local rotations
            Quaternion spreadTiltRotation = Quaternion.AngleAxis(spreadRotation, Vector3.Cross(weaponForwardVelocity.normalized, transform.up));
            Quaternion tiltRotation = Quaternion.AngleAxis(_initialShurikenTiltAngleOffset, weaponForwardVelocity.normalized);
            newWeapon.rotation = playerXAxisRotation * tiltRotation;

            // Calculate forward trajectory
            Vector3 rotatedForwardVelocity = playerXAxisRotation * playerYAxisRotation * weaponForwardVelocity;

            // Apply forward & spin forces
            newWeapon.AddForce(rotatedForwardVelocity, ForceMode.Impulse);
            newWeapon.AddTorque(newWeapon.transform.up * _initialShurikenSpin, ForceMode.Impulse);
        }

        _shootHarpoonSFX.pitch = Random.Range(_minHarpoonShootPitch, _maxHarpoonShootPitch);
        _shootHarpoonSFX.Play();

        UI.Instance.UseWeapon(Weapon.WeaponType.Shuriken);

        // Deal damage using sphere-cast
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _shurikenSphereCastRadius, transform.forward, _shurikenSphereCastRadius * 2, _weaponHitCheckLayerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Player" || hit.transform.name == "Shuriken")
                return;

            // Deal damage if possible
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                if (hit.transform.gameObject.TryGetComponent(out EnemyController enemyController))
                {
                    if (enemyController.CurrentLifePoints > 0)
                    {
                        damageable.TakeDamage(_shurikenDamage);
                    }
                }
                else
                {
                    damageable.TakeDamage(_shurikenDamage);
                }
            }
        }
    }

    private void OnFireHarpoon()
    {
        if (dying || UI.Instance.GetAmmoForPowerUp(Weapon.WeaponType.Harpoon) <= 0)
            return;

        // Try spawn harpoon
        if (_harpoonPrefab == null)
            return;

        ShootDirectedWeapon(_harpoonPrefab, _harpoonSpawnPoint, _initialHarpoonVelocity, 0);

        _shootHarpoonSFX.pitch = Random.Range(_minHarpoonShootPitch, _maxHarpoonShootPitch);
        _shootHarpoonSFX.Play();

        UI.Instance.UseWeapon(Weapon.WeaponType.Harpoon);
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

        // Play/Pause jetpack bubbles
        if (moveInput != Vector2.zero)
            _jetpackBubblePS.Play();
        else if (moveInput == Vector2.zero && _jetpackBubblePS.isPlaying)
            _jetpackBubblePS.Stop();

        ProcessAudio();
    }

    public void TryAttack()
    {
        // Not needed
    }

    public void Attack()
    {
        if (dying)
            return;

        // Try spawn dart
        if (_dartPrefab == null)
            return;

        ShootDirectedWeapon(_dartPrefab, _dartSpawnPoint, _initialDartVelocity, _initialDartSpin);

        _shootDartSFX.pitch = Random.Range(_minDartShootPitch, _maxDartShootPitch);
        _shootDartSFX.Play();
    }

    private void ShootDirectedWeapon(Rigidbody weaponPrefab, Transform spawnPoint, float initialVelocity, float initialSpin)
    {
        Rigidbody newWeapon = Instantiate(weaponPrefab, spawnPoint.position, Quaternion.LookRotation(spawnPoint.forward, transform.up));

        // Calculate dart trajectory towards first impact point
        // If nothing hit in-front of nozzle, return shooting along my default trajectory based on spawn point forward
        Vector3 weaponForwardVelocity = spawnPoint.forward * initialVelocity;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, 1000, _weaponHitCheckLayerMask))
        {
            if (Vector3.Dot(spawnPoint.forward, (hit.point - spawnPoint.position)) > _rangeInFrontOfNozzleForAimAssist)
            {
                // Otherwise if (closest target in-front of nozzle is found, aim at that target
                weaponForwardVelocity = (hit.point - spawnPoint.position).normalized * initialVelocity;
            }
        }

        // Add slight influence from player's current velocity (in RightUp planes only) to trajectory velocity
        Vector3 playerRightUpVelocity = _rb.velocity - spawnPoint.forward;
        Vector3 playerVelocityInfluence = playerRightUpVelocity * _playerVelocityInfluenceOverWeaponTrajectories;

        newWeapon.AddForce(weaponForwardVelocity + playerVelocityInfluence, ForceMode.Impulse);
        newWeapon.AddTorque(weaponForwardVelocity.normalized * initialSpin, ForceMode.Impulse);
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

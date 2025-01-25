using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>, IDamageable, IAttackable
{
    [Header("Statics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    [Header("Properties")]
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

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool fireWeaponInput = false;

    private Vector3 originLocalCameraLocalPos;
    private bool dying

    protected override void Awake()
    {
        base.Awake();

        originLocalCameraLocalPos = cam.transform.localPosition;

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
        fireWeaponInput = value.Get<float>() > 0;
    }

    private void FixedUpdate()
    {
        // Apply movement input
        Vector3 forwardMoveInput = transform.forward * moveInput.y * (jetpackAcceleration + (moveInput.x != 0 ? rollAccelerationBoost : 0)) * Time.deltaTime;
        rb.AddForce(forwardMoveInput, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity + (moveInput.x != 0 ? rollSpeedBoost : 0));
    }

    private void Update()
    {
        // Rotate pitch, yaw, and roll
        transform.Rotate(new Vector3(-lookInput.y * pitchRotationSpeed * Time.deltaTime, lookInput.x * yawRotationSpeed * Time.deltaTime, -moveInput.x * rollRotationSpeed * Time.deltaTime), Space.Self);

        // Camera zoom if rolling
        cam.transform.localPosition = originLocalCameraLocalPos + (Vector3.forward * (moveInput.x != 0 ? rollCameraForwardOffset : 0));
        cam.fieldOfView = moveInput.x != 0 ? cameraRollFOV : cameraNormalFOV;

        // Fire weapon
    }

    public void Attack()
    {
        
    }

    public void TakeDamage(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            UI.Instance.Damage();
        }
    }

    public void OnDeath()
    {
        // Play death sound
    }
}

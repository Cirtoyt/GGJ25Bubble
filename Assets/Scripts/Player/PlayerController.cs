using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>
{
    [Header("Statics")]
    [SerializeField] private Rigidbody rb;
    [Header("Properties")]
    [Min(0)]
    [SerializeField] private float jetpackAcceleration = 1;
    [SerializeField] private float pitchRotationSpeed = 1;
    [SerializeField] private float yawRotationSpeed = 1;
    [SerializeField] private float rollRotationSpeed = 1;
    [Min(0)]
    [SerializeField] private float maxVelocity = 1;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool fireWeaponInput = false;

    protected override void Awake()
    {
        base.Awake();

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
        Vector3 forwardMoveInput = transform.forward * moveInput.y * jetpackAcceleration * Time.deltaTime;
        rb.AddForce(forwardMoveInput, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    private void Update()
    {
        // Rotate pitch, yaw, and roll
        transform.Rotate(new Vector3(-lookInput.y * pitchRotationSpeed, lookInput.x * yawRotationSpeed, moveInput.x * rollRotationSpeed), Space.Self);
    }
}

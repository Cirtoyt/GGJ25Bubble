using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        float lookX = 0;
        if (value.Get<Vector2>().x != 0)
            lookX = value.Get<Vector2>().x > 0 ? 1 : -1;

        float lookY = 0;
        if (value.Get<Vector2>().y != 0)
            lookY = value.Get<Vector2>().y > 0 ? 1 : -1;

        lookInput = new Vector2(lookX, lookY);
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
        transform.Rotate(new Vector3(-lookInput.y * pitchRotationSpeed * Time.deltaTime, lookInput.x * yawRotationSpeed * Time.deltaTime, moveInput.x * rollRotationSpeed * Time.deltaTime), Space.Self);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    [SerializeField] private float yRotationSpeed = 1;

    private float yRotation = 0;

    private void Update()
    {
        // Calculate constant y rotation spin
        yRotation += yRotationSpeed * Time.deltaTime;
        yRotation %= 360;

        // Align this' up vector to the player's up vector
        transform.LookAt(PlayerController.Instance.transform, PlayerController.Instance.transform.up);

        // Apply y rotation spin
        transform.Rotate(Vector3.up, yRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        OnPickup();

        // Do pop particles

        Destroy(gameObject);
    }

    protected abstract void OnPickup();
}

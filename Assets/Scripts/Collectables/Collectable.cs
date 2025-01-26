using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

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

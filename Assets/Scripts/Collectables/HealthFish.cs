using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFish : Collectable
{
    protected override void OnPickup()
    {
        UI.Instance.Heal();
    }
}

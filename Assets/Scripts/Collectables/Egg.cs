using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Collectable
{
    protected override void OnPickup()
    {
        GameManager.Instance.WinGame();
    }
}

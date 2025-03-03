using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPowerUp : Collectable
{
    [SerializeField] private Weapon.WeaponType _weaponType;
    [SerializeField] private int _ammoAmount;

    protected override void OnPickup()
    {
        UI.Instance.AddPowerUp(_weaponType, _ammoAmount);
    }
}

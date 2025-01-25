using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [Header("Statics")]
    [SerializeField] GameObject mesh;
    [SerializeField] AudioSource _onHitAudio;
    [Header("Properties")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int pierce;
    [SerializeField] float _audioLength = 1;

    public enum WeaponType
    {
        Dart,
        Kunai,
        Shuriken,
        Boomerang,
        Harpoon,
    }

    public WeaponType type;

    private void Awake()
    {

    }

    private void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(damage);

        if (pierce < 1)
        {
            mesh.SetActive(false);
            Destroy(mesh, _audioLength);
        }
        pierce--;
    }

}

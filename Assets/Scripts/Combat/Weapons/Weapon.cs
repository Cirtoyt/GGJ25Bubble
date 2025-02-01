using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [Header("Statics")]
    [SerializeField] GameObject _mesh;
    [SerializeField] Collider _collider;
    [SerializeField] AudioSource _onHitAudio;
    [Header("Properties")]
    [SerializeField] int _damage = 1;
    [SerializeField] int _pierce = 0;
    [SerializeField] float _hitAudioDuration = 1;

    public enum WeaponType
    {
        Dart,
        Shuriken,
        Harpoon,
        Boomerang,
    }

    public WeaponType type;

    private bool selfDestructing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (selfDestructing)
            return;

        // Ignore the player
        if (other.tag == "Player")
            return;

        // Deal damage if possible
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }

        // Check if destroy self when cannot pierce any longer
        if (_pierce < 1)
        {
            selfDestructing = true;
            _collider.enabled = false;

            // Hide mesh
            _mesh.SetActive(false);

            // Play audio
            _onHitAudio.Play();

            // Triggered audio length delayed self-destruction
            Destroy(gameObject, _hitAudioDuration);
        }

        // Reduce pierce counter
        _pierce--;
    }

}

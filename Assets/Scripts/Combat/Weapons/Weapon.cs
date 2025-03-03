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
    [SerializeField] bool _selfDestruct = false;
    [SerializeField] float _timeTillSelfDestruct = 0.5f;

    public enum WeaponType
    {
        Dart,
        Shuriken,
        Harpoon,
        Boomerang,
    }

    public WeaponType type;

    private bool selfDestructing = false;

    private void Awake()
    {
        if (_selfDestruct)
            StartCoroutine(StartDelayedSelfDestruct());
    }

    private IEnumerator StartDelayedSelfDestruct()
    {
        yield return new WaitForSeconds(_timeTillSelfDestruct);

        if (selfDestructing)
            yield break;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (selfDestructing)
            return;

        // Ignore the player
        if (other.tag == "Player" || other.name == "Shuriken")
            return;

        // Deal damage if possible
        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (other.gameObject.TryGetComponent(out EnemyController enemyController))
            {
                if (enemyController.CurrentLifePoints > 0)
                {
                    damageable.TakeDamage(_damage);
                }
            }
            else
            {
                damageable.TakeDamage(_damage);
            }
        }

        // Check if destroy self when cannot pierce any longer
        if (_pierce < 1)
        {
            selfDestructing = true;
            _collider.enabled = false;

            // Hide mesh
            _mesh.SetActive(false);

            // Play audio
            if (_onHitAudio != null)
                _onHitAudio.Play();

            // Triggered audio length delayed self-destruction
            Destroy(gameObject, _hitAudioDuration);

            //Debug.Log($"Weapon {name} hit {other.name}!");
        }

        // Reduce pierce counter
        _pierce--;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField ]GameObject mesh;
    [SerializeField] int damage;
    [SerializeField] int speed;

    [SerializeField] int pierce;
    // Start is called before the first frame update

    public enum WeaponType
    {
        Dart,
        Kunai,
        Shuriken
    }

    public WeaponType type;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(pierce < 1)
        {
            Destroy(mesh);
        }
        pierce--;
    }

}

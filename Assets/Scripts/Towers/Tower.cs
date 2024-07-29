using UnityEngine;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] protected TowerInfo scriptableObject;
        
        [SerializeField] protected GameObject turret;
        
        //TODO: Change this from a string to a int perhaps.
        [SerializeField] protected string[] validTargets;
        
        protected float RateOfFireTimer;
        protected CapsuleCollider CapsuleCollider;
        
        //
        protected readonly Collider[] Colliders = new Collider[10];

        [SerializeField] protected LayerMask towerLayerMask;

        [Header("Range Values")]
        protected float Range;
        [Header("Damage Values")]
        protected float Damage;
        [Header("Rate of Fire Values")]
        protected float RateOfFire;
        
        //Used to allow/block attacks.
        [SerializeField] protected bool isFireRunning;

        //Sounds.
        [SerializeField] protected AudioSource weaponFire;

        //Explosives.
        [SerializeField] protected bool hasExplosiveAmmo;
        [SerializeField] protected GameObject explosiveEffect;
    }
}
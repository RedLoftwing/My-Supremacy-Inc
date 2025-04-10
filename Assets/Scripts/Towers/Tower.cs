using UnityEngine;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] protected SO_Scripts.TowerInfo scriptableObject;
        [SerializeField] protected GameObject[] horizontalTurret;
        [SerializeField] protected GameObject[] verticalTurret;
        
        //TODO: Change this from a string to a int perhaps.
        [SerializeField] protected string[] validTargets;
        
        protected float RateOfFireTimer;
        public CapsuleCollider capsuleCollider;
        
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
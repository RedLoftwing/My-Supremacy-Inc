using UnityEngine;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [TextArea(minLines: 1, maxLines: 1)] public string towerName;
        [SerializeField] protected GameObject turret;
        public int towerCost;
        [SerializeField] protected float damage;
        [SerializeField] protected string[] validTargets;

        [Header("Rate of Fire Values")] [SerializeField]
        protected float rateOfFire;

        protected float RateOfFireTimer;
        protected CapsuleCollider CapsuleCollider;
        [SerializeField] protected float range;

        [Min(15f)]
        //DefaultValues.
        protected float DefaultDamage;

        protected float DefaultRateOfFire;

        protected float DefaultRange;

        //
        protected readonly Collider[] Colliders = new Collider[10];

        [SerializeField] protected LayerMask towerLayerMask;

        //[DEV] May be redundant...
        protected bool IsRotatable;

        [SerializeField] protected float rotationSpeed;

        //Used to allow/block attacks.
        [SerializeField] protected bool isFireRunning;

        //Sounds.
        [SerializeField] protected AudioSource weaponFire;

        //Explosives.
        [SerializeField] protected bool hasExplosiveAmmo;
        [SerializeField] protected GameObject explosiveEffect;
    }
}
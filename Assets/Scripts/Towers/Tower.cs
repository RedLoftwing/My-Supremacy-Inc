using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [TextArea(minLines: 1, maxLines: 1)] public string towerName;
    [SerializeField] protected GameObject turret;
    public int towerCost;
    [SerializeField] protected float damage;
    [SerializeField] protected string[] validTargets; 
    [Header("Rate of Fire Values")]
    [SerializeField] protected float rateOfFire;
    protected float rateOfFireTimer;
    protected CapsuleCollider capsuleCollider;
    [SerializeField] protected float range; [Min(15f)]
    //DefaultValues.
    protected float defaultDamage;
    protected float defaultRateOfFire;
    protected float defaultRange;
    //
    protected readonly Collider[] colliders = new Collider[10];
    [SerializeField] protected LayerMask towerLayerMask;
    //[DEV] May be redundant...
    protected bool isRotatable;
    [SerializeField] protected float rotationSpeed;
    //Used to allow/block attacks.
    [SerializeField] protected bool isFireRunning = false;
    //Sounds.
    [SerializeField] protected AudioSource weaponFire;
    //Explosives.
    [SerializeField] protected bool hasExplosiveAmmo;
    [SerializeField] protected GameObject explosiveEffect;
}

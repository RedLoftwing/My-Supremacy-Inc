using UnityEngine;

[CreateAssetMenu(fileName = "PurchasableOptionsInfo", menuName = "ScriptableObjects/PurchasableOptionsInfo", order = 1)]
public class PurchasableOptionsInfo : ScriptableObject
{
    public string towerName;
    public string towerDescription;
    
    public int towerCost;
    public string attackTypeInfo;

    [Header("Valid Targets")] 
    public bool infantry;
    public bool unarmoured;
    public bool armoured;
    public bool air;
    
    public enum TargetTypes
    {
        automatic,
        manualTargetPlacement,
        NotApplicable
    }
    public TargetTypes validTargetTypes;
    
    [Header("Default Values")]
    public float defaultRange;
    public float defaultDamage;
    public float defaultRateOfFire;
    public float rotationSpeed;

    [Header("SoundFX")] public AudioSource weaponFire;
    
    public int maxNoOfTargets;
    
    public GameObject towerPrefab;
}

using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "TowerInfo", menuName = "ScriptableObjects/TowerInfo", order = 1)]
    public class TowerInfo : ScriptableObject {
        public enum TargetTypes {
            Unarmoured = 0,
            Armoured = 1,
            Aerial = 2
        };
        [Header("Details")]
        public string towerName;
        public string towerDescription;
        public string attackTypeInfo;
        [Header("Expenditure")]
        public int towerCost;
        [Header("Valid Targets")] 
        public TargetTypes[] targetTypes = {};
        public bool infantry;
        public bool unarmoured;
        public bool armoured;
        public bool air;
        public enum TargettingTypes
        {
            automatic,
            manualTargetPlacement,
            NotApplicable
        }
        [Header("Targeting Options")]
        public TargettingTypes targetingType;
        public int maxNoOfTargets;
        [Header("Default Values")] 
        public float defaultRange;
        public float defaultDamage;
        public float defaultRateOfFire;
        public float rotationSpeed;
        [Header("SoundFX")] 
        public AudioSource weaponFire;
        [Header("Prefabs")] 
        public GameObject towerPrefab;
        public GameObject hologramPrefab;
    }
}
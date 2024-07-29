using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "TowerInfo", menuName = "ScriptableObjects/TowerInfo", order = 1)]
    public class TowerInfo : ScriptableObject
    {
        public string towerName;
        public string towerDescription;

        public int towerCost;
        public string attackTypeInfo;

        [Header("Valid Targets")] public bool infantry;
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

        [Header("Default Values")] public float defaultRange;
        public float defaultDamage;
        public float defaultRateOfFire;
        public float rotationSpeed;

        [Header("SoundFX")] public AudioSource weaponFire;

        public int maxNoOfTargets;

        [Header("Prefabs")] public GameObject towerPrefab;
        public GameObject hologramPrefab;
    }
}
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "AbilityInfo", menuName = "ScriptableObjects/AbilityInfo", order = 1)]
    public class AbilityInfo : ScriptableObject
    {
        [Header("Details")]
        public string abilityName;
        public string abilityDescription;
        public enum CurrencyTypes
        {
            Water,
            Cash,
            NoneSelected
        }
        [Header("Expenditure")]
        public CurrencyTypes abilityExpenditureCurrencyType;
        public Sprite abilityExpenditureCurrencyTypeIcon;
        public int abilityExpenditureAmount;
        [Header("Income - If applicable")]
        public CurrencyTypes abilityIncomeCurrencyType;
        public int abilityIncomeAmount;
        [Header("Prefabs")]
        public GameObject abilityPrefab;
    }
}
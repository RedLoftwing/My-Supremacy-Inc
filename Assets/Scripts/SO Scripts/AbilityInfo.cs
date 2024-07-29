using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "AbilityInfo", menuName = "ScriptableObjects/AbilityInfo", order = 1)]
    public class AbilityInfo : ScriptableObject
    {
        public string abilityName;
        public string abilityDescription;
        public enum CurrencyTypes
        {
            Water,
            Cash,
            NoneSelected
        }
        [Header("Expenditure")]
        public CurrencyTypes expenditureCurrencyType;
        public int abilityExpenditureAmount;
        [Header("Income - If applicable")]
        public CurrencyTypes incomeCurrencyType;
        public int abilityIncomeAmount;
        public GameObject abilityPrefab;
    }
}
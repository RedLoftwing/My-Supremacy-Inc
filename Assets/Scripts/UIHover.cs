using UnityEngine;
using UnityEngine.EventSystems;
using System.Globalization;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SO_Scripts.TowerInfo towerInfo;
    public SO_Scripts.AbilityInfo abilityInfo;
    [SerializeField] private Sprite costTypeIcon;
    public GameObject buttonGreyOutFilter;

    public enum PurchasableType
    {
        Tower,
        Ability
    }
    public PurchasableType purchasableType;
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //When the cursor enters the button area, set the relevant info panel to active AND set the text fields to the relevant information for the ability or tower.
        if (towerInfo)
        {
            UserInterface.Instance.towerInfoPanel.SetActive(true);
            UserInterface.Instance.isCursorOverTowerButton = true;
            UserInterface.Instance.towerInfoPanelTitle.SetText(towerInfo.towerName);
            UserInterface.Instance.towerInfoPanelCost.SetText(towerInfo.towerCost.ToString(CultureInfo.InvariantCulture));
            UserInterface.Instance.towerInfoPanelAttackInfo.SetText(towerInfo.attackTypeInfo);
            UserInterface.Instance.towerInfoPanelDescription.SetText(towerInfo.towerDescription);
        }
        else
        {
            UserInterface.Instance.abilityInfoPanel.SetActive(true);
            UserInterface.Instance.isCursorOverAbilityButton = true;
            UserInterface.Instance.abilityInfoPanelTitle.SetText(abilityInfo.abilityName);
            UserInterface.Instance.abilityInfoPanelCost.SetText(abilityInfo.abilityExpenditureAmount.ToString(CultureInfo.InvariantCulture) + " " + abilityInfo.abilityExpenditureCurrencyType.ToString());
            UserInterface.Instance.abilityInfoPanelCostIcon.sprite = abilityInfo.abilityExpenditureCurrencyTypeIcon;
            UserInterface.Instance.abilityInfoPanelDescription.SetText(abilityInfo.abilityDescription);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //When the cursor leaves the button area, set the info panels to inactive.
        if(UserInterface.Instance.towerInfoPanel.activeInHierarchy) { UserInterface.Instance.isCursorOverTowerButton = false; }
        if(UserInterface.Instance.abilityInfoPanel.activeInHierarchy) { UserInterface.Instance.isCursorOverAbilityButton = false; }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using System.Globalization;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UserInterface userInterfaceScript;
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
    
    private void Start()
    {
        userInterfaceScript = FindObjectOfType<UserInterface>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //When the cursor enters the button area, set the relevant info panel to active AND set the text fields to the relevant information for the ability or tower.
        if (towerInfo)
        {
            userInterfaceScript.towerInfoPanel.SetActive(true);
            userInterfaceScript.isCursorOverTowerButton = true;
            userInterfaceScript.towerInfoPanelTitle.SetText(towerInfo.towerName);
            userInterfaceScript.towerInfoPanelCost.SetText(towerInfo.towerCost.ToString(CultureInfo.InvariantCulture));
            userInterfaceScript.towerInfoPanelAttackInfo.SetText(towerInfo.attackTypeInfo);
            userInterfaceScript.towerInfoPanelDescription.SetText(towerInfo.towerDescription);
        }
        else
        {
            userInterfaceScript.abilityInfoPanel.SetActive(true);
            userInterfaceScript.isCursorOverAbilityButton = true;
            userInterfaceScript.abilityInfoPanelTitle.SetText(abilityInfo.abilityName);
            userInterfaceScript.abilityInfoPanelCost.SetText(abilityInfo.abilityExpenditureAmount.ToString(CultureInfo.InvariantCulture) + " " + abilityInfo.abilityExpenditureCurrencyType.ToString());
            userInterfaceScript.abilityInfoPanelCostIcon.sprite = abilityInfo.abilityExpenditureCurrencyTypeIcon;
            userInterfaceScript.abilityInfoPanelDescription.SetText(abilityInfo.abilityDescription);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //When the cursor leaves the button area, set the info panels to inactive.
        if(userInterfaceScript.towerInfoPanel.activeInHierarchy) { userInterfaceScript.isCursorOverTowerButton = false; }
        if(userInterfaceScript.abilityInfoPanel.activeInHierarchy) { userInterfaceScript.isCursorOverAbilityButton = false; }
    }
}

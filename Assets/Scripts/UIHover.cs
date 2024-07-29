using UnityEngine;
using UnityEngine.EventSystems;
using System.Globalization;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UserInterface userInterfaceScript;
    public TowerInfo towerInfo;
    //public AbilityInfo abilityInfo;
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
        //When the cursor enters the button area, set the towerInfoPanel to active AND set the text fields to the relevant information for the tower.
        userInterfaceScript.towerInfoPanel.SetActive(true);
        userInterfaceScript.towerInfoPanelTitle.SetText(towerInfo.towerName);
        userInterfaceScript.towerInfoPanelCost.SetText(towerInfo.towerCost.ToString(CultureInfo.InvariantCulture));
        userInterfaceScript.towerInfoPanelAttackInfo.SetText(towerInfo.attackTypeInfo);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //When the cursor leaves the button area, set the towerInfoPanel to inactive.
        userInterfaceScript.towerInfoPanel.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UserInterface userInterfaceScript;
    [TextArea(minLines: 1, maxLines: 1)] public string title;
    [TextArea(minLines: 1, maxLines: 1)] public string cost;
    [TextArea(minLines: 1, maxLines: 3)] public string attackInfo;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //When the cursor enters the button area, set the towerInfoPanel to active AND set the text fields to the relevant information for the tower.
        userInterfaceScript.towerInfoPanel.SetActive(true);
        userInterfaceScript.towerInfoPanelTitle.SetText(title);
        userInterfaceScript.towerInfoPanelCost.SetText(cost);
        userInterfaceScript.towerInfoPanelAttackInfo.SetText(attackInfo);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //When the cursor leaves the button area, set the towerInfoPanel to inactive.
        userInterfaceScript.towerInfoPanel.SetActive(false);
    }
}

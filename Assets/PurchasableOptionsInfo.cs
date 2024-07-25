using UnityEngine;

[CreateAssetMenu(fileName = "PurchasableOptionsInfo", menuName = "ScriptableObjects/PurchasableOptionsInfo", order = 1)]
public class PurchasableOptionsInfo : ScriptableObject
{
    public string towerName;
    public float towerCost;
    public string attackTypeInfo;
    public string towerDescription;
}

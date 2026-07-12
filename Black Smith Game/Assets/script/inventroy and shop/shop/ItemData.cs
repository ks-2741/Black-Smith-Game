using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int basePrice;
    public bool isCraftingMaterial; // true for ore/ingots, false for finished goods
}

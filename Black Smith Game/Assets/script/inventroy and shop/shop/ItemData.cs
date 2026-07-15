using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int basePrice;   // what the player pays to buy this in the shop
    public int sellPrice;   // what the player earns when a customer buys this from them
    public bool isCraftingMaterial; // true for ore/ingots/blades, false for finished goods
}
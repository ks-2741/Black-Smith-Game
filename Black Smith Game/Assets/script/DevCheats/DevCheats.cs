using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    [Header("Dev Cheat Items")]
    public ItemData ingotItem;
    public ItemData bladeItem;   // rough blade (pre-grind)
    public ItemData swordItem;   // finished sword

    [Header("Settings")]
    public bool enableCheats = true; // easy master switch to disable before shipping

    void Update()
    {
        if (!enableCheats)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
            GiveItem(ingotItem);

        if (Keyboard.current.oKey.wasPressedThisFrame)
            GiveItem(bladeItem);

        if (Keyboard.current.pKey.wasPressedThisFrame)
            GiveItem(swordItem);
    }

    void GiveItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("DevCheats: item slot not assigned.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("DevCheats: InventoryManager.Instance is null.");
            return;
        }

        InventoryManager.Instance.AddItem(item, 1);
        Debug.Log("[DEV CHEAT] Added 1x " + item.itemName);
    }
}
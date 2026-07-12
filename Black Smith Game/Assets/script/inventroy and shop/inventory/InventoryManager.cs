using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> slots = new List<InventorySlot>();

    public event System.Action OnInventoryChanged;

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("InventoryManager: tried to add a null item.");
            return;
        }

        InventorySlot slot = slots.Find(s => s.item == item);

        if (slot != null)
            slot.quantity += amount;
        else
            slots.Add(new InventorySlot { item = item, quantity = amount });

        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        InventorySlot slot = slots.Find(s => s.item == item);
        return slot != null && slot.quantity >= amount;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        InventorySlot slot = slots.Find(s => s.item == item);

        if (slot == null || slot.quantity < amount)
            return false;

        slot.quantity -= amount;

        if (slot.quantity <= 0)
            slots.Remove(slot);

        OnInventoryChanged?.Invoke();
        return true;
    }
}

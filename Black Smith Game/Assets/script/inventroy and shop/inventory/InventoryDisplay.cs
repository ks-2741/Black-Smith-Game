using UnityEngine;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    [Header("References")]
    public Transform contentParent;      // the parent that holds spawned slot UIs (e.g. a Grid Layout Group object)
    public InventorySlotUI slotPrefab;   // prefab with the InventorySlotUI script on it

    [Header("Starting Items (set in Inspector)")]
    public List<InventorySlot> startingItems = new List<InventorySlot>();

    void Start()
    {
        AddStartingItems();
    }

    void OnEnable()
    {
        Refresh();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }

    // Adds everything in the "Starting Items" list to the inventory.
    // Runs automatically on Start, and can also be triggered manually
    // in the Editor (right-click the component header -> "Add Starting Items Now").
    [ContextMenu("Add Starting Items Now")]
    public void AddStartingItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryDisplay: InventoryManager.Instance is null, cannot add starting items.");
            return;
        }

        foreach (InventorySlot entry in startingItems)
        {
            if (entry.item == null || entry.quantity <= 0)
                continue;

            InventoryManager.Instance.AddItem(entry.item, entry.quantity);
            Debug.Log("Added starting item: " + entry.quantity + "x " + entry.item.itemName);
        }
    }

    // Call this at runtime (e.g. from another script) to add a single item on demand.
    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null || InventoryManager.Instance == null)
            return;

        InventoryManager.Instance.AddItem(item, amount);
    }

    public void Refresh()
    {
        if (contentParent == null || slotPrefab == null)
        {
            Debug.LogWarning("InventoryDisplay: Content Parent or Slot Prefab not assigned.");
            return;
        }

        // Clear old slots
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (InventoryManager.Instance == null)
            return;

        // Spawn one slot per item
        foreach (InventorySlot slot in InventoryManager.Instance.slots)
        {
            if (slot.item == null || slot.quantity <= 0)
                continue;

            InventorySlotUI slotUI = Instantiate(slotPrefab, contentParent);
            slotUI.Setup(slot.item, slot.quantity);
        }
    }
}
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [Header("References")]
    public Transform contentParent;      // the parent that holds spawned slot UIs (e.g. a Grid Layout Group object)
    public InventorySlotUI slotPrefab;   // prefab with the InventorySlotUI script on it

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
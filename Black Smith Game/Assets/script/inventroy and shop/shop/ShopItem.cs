using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Item")]
    public ItemData item;
    public int amountToBuy = 1;

    [Header("Stock Limit")]
    public int maxQuantity = 0; // 0 = unlimited. Otherwise blocks buying once the player owns this many.
    public TMP_Text maxReachedText; // optional, shows a message when the limit is hit

    [Header("Optional Price UI")]
    public TMP_Text priceText;

    private Button button;

    void Start()
    {
        if (priceText != null && item != null)
            priceText.text = "£" + item.basePrice;

        button = GetComponent<Button>();

        if (maxReachedText != null)
            maxReachedText.gameObject.SetActive(false);

        RefreshButtonState();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshButtonState;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshButtonState;
    }

    // Hook this up to the Button's OnClick in the Inspector
    public void Buy()
    {
        if (item == null)
        {
            Debug.LogWarning("ShopItem: No ItemData assigned.");
            return;
        }

        if (IsAtLimit())
        {
            Debug.Log("Can't buy more " + item.itemName + " - stock limit reached.");

            if (maxReachedText != null)
                maxReachedText.gameObject.SetActive(true);

            return;
        }

        // If/when you add currency, check + deduct gold here before adding.
        // e.g. if (!CurrencyManager.Instance.Spend(item.basePrice)) return;

        InventoryManager.Instance.AddItem(item, amountToBuy);

        // If this is the ore item, also drop physical ore into the pile.
        if (OrePileManager.Instance != null && OrePileManager.Instance.oreItem == item)
        {
            for (int i = 0; i < amountToBuy; i++)
                OrePileManager.Instance.AddOre();
        }

        Debug.Log("Bought " + amountToBuy + "x " + item.itemName);

        RefreshButtonState();
    }

    bool IsAtLimit()
    {
        if (maxQuantity <= 0 || item == null || InventoryManager.Instance == null)
            return false; // unlimited

        return InventoryManager.Instance.GetQuantity(item) >= maxQuantity;
    }

    void RefreshButtonState()
    {
        bool atLimit = IsAtLimit();

        if (button != null)
            button.interactable = !atLimit;

        if (maxReachedText != null && !atLimit)
            maxReachedText.gameObject.SetActive(false);
    }
}
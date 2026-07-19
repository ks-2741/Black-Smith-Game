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
    public TMP_Text maxReachedText; // optional, shows a message when the stock limit is hit

    [Header("Currency")]
    public TMP_Text notEnoughGoldText; // optional, shows a message when the player can't afford it

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

        if (notEnoughGoldText != null)
            notEnoughGoldText.gameObject.SetActive(false);

        RefreshButtonState();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshButtonState;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += RefreshButtonState;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshButtonState;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= RefreshButtonState;
    }

    // Hook this up to the Button's OnClick in the Inspector
    public void Buy()
    {
        if (item == null)
        {
            Debug.LogWarning("ShopItem: No ItemData assigned.");
            return;
        }

        if (IsAtStockLimit())
        {
            Debug.Log("Can't buy more " + item.itemName + " - stock limit reached.");

            if (maxReachedText != null)
                maxReachedText.gameObject.SetActive(true);

            return;
        }

        int totalCost = item.basePrice * amountToBuy;

        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.SpendGold(totalCost))
        {
            Debug.Log("Can't buy " + item.itemName + " - not enough gold. Need £" + totalCost);

            if (notEnoughGoldText != null)
                notEnoughGoldText.gameObject.SetActive(true);

            return;
        }

        InventoryManager.Instance.AddItem(item, amountToBuy);

        Debug.Log("Bought " + amountToBuy + "x " + item.itemName + " for £" + totalCost);

        RefreshButtonState();
    }

    bool IsAtStockLimit()
    {
        if (maxQuantity <= 0 || item == null || InventoryManager.Instance == null)
            return false; // unlimited

        return InventoryManager.Instance.GetQuantity(item) >= maxQuantity;
    }

    bool CanAffordIt()
    {
        if (item == null || CurrencyManager.Instance == null)
            return true; // no currency system in scene, don't block

        return CurrencyManager.Instance.CanAfford(item.basePrice * amountToBuy);
    }

    void RefreshButtonState()
    {
        bool atStockLimit = IsAtStockLimit();
        bool canAfford = CanAffordIt();

        if (button != null)
            button.interactable = !atStockLimit && canAfford;

        if (maxReachedText != null && !atStockLimit)
            maxReachedText.gameObject.SetActive(false);

        if (notEnoughGoldText != null && canAfford)
            notEnoughGoldText.gameObject.SetActive(false);
    }
}
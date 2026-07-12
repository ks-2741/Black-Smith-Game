using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Item")]
    public ItemData item;
    public int amountToBuy = 1;

    [Header("Optional Price UI")]
    public TMP_Text priceText;

    void Start()
    {
        if (priceText != null && item != null)
            priceText.text = "£" + item.basePrice;
    }

    // Hook this up to the Button's OnClick in the Inspector
    public void Buy()
    {
        if (item == null)
        {
            Debug.LogWarning("ShopItem: No ItemData assigned.");
            return;
        }

        // If/when you add currency, check + deduct gold here before adding.
        // e.g. if (!CurrencyManager.Instance.Spend(item.basePrice)) return;

        InventoryManager.Instance.AddItem(item, amountToBuy);

        Debug.Log("Bought " + amountToBuy + "x " + item.itemName);
    }
}

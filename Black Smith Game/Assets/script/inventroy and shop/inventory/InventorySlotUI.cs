using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text quantityText;

    public void Setup(ItemData item, int quantity)
    {
        if (iconImage != null)
        {
            if (item.icon != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }

        if (nameText != null)
            nameText.text = item.itemName;

        if (quantityText != null)
            quantityText.text = "x" + quantity;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class MapMenuUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;        // the panel holding Desk/Anvil/Furnace/Grindstone/Shop/Inv buttons
    public Button toggleButton;     // the small always-visible "Menu" / "Map" button

    void Start()
    {
        if (panel != null)
            panel.SetActive(false); // closed by default

        // Wire the toggle button via code, so it works even if the
        // Inspector OnClick() gets wiped for any reason.
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(ToggleMenu);
            toggleButton.onClick.AddListener(ToggleMenu);
        }

        WireAutoClose();
    }

    // Finds every button already inside the panel (Desk, Anvil, Furnace,
    // Grindstone, Shop, Inv, etc.) and adds an extra listener that closes
    // the menu after they're clicked - without touching whatever each
    // button already does in the Inspector.
    void WireAutoClose()
    {
        if (panel == null)
            return;

        Button[] childButtons = panel.GetComponentsInChildren<Button>(true);

        foreach (Button b in childButtons)
        {
            b.onClick.AddListener(CloseMenu);
        }

        Debug.Log("MapMenuUI: auto-close wired to " + childButtons.Length + " buttons.");
    }

    public void ToggleMenu()
    {
        if (panel == null)
            return;

        bool newState = !panel.activeSelf;
        panel.SetActive(newState);

        Debug.Log("Map menu toggled: " + newState);
    }

    public void OpenMenu()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
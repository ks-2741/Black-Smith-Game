using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Canvases")]
    public GameObject stationButtonsUI;   // Desk / Anvil / Furnace / Grindstone menu
    public GameObject shopUI;
    public GameObject inventoryUI;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Default state on scene load
        ShowStationButtons();
    }

    public void OpenShop()
    {
        Debug.Log("Opening Shop");

        if (stationButtonsUI != null)
            stationButtonsUI.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        if (shopUI != null)
            shopUI.SetActive(true);
    }

    public void OpenInventory()
    {
        Debug.Log("Opening Inventory");

        if (stationButtonsUI != null)
            stationButtonsUI.SetActive(false);

        if (shopUI != null)
            shopUI.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.SetActive(true);
    }

    public void ShowStationButtons()
    {
        Debug.Log("Showing Station Buttons");

        if (shopUI != null)
            shopUI.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        if (stationButtonsUI != null)
            stationButtonsUI.SetActive(true);
    }
}

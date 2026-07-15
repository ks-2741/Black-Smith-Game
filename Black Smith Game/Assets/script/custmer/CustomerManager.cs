using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    [Header("Spawn Setup")]
    public Customer customerPrefab;
    public Transform spawnPoint;
    public Transform waitPoint;
    public Transform exitPoint;

    [Header("Requests")]
    public ItemData[] possibleRequests; // e.g. Sword, drag any sellable ItemData in here

    [Header("Timing")]
    public float delayBetweenCustomers = 2f;

    [Header("Camera")]
    public CameraSwitcher cameraSwitcher;
    public CameraSwitcher.CameraView customerCameraView = CameraSwitcher.CameraView.Forge; // Forge = index 0 = your "desk" / Shop front cam view

    [Header("Give Button UI")]
    public GameObject giveButtonUI; // wire OnClick -> CustomerManager.OnGiveButtonPressed
    public GameObject notEnoughItemText; // optional, shows if player doesn't have the item

    private Customer currentCustomer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (giveButtonUI != null)
            giveButtonUI.SetActive(false);

        if (notEnoughItemText != null)
            notEnoughItemText.SetActive(false);

        SpawnNextCustomer();
    }

    void Update()
    {
        // Show/hide the Give button based on whether we're looking at the
        // customer counter AND a customer is actually there waiting.
        bool lookingAtCustomer =
            cameraSwitcher != null &&
            cameraSwitcher.IsCameraActive(customerCameraView);

        bool customerWaiting =
            currentCustomer != null &&
            currentCustomer.CurrentState == Customer.State.Waiting;

        bool shouldShowButton = lookingAtCustomer && customerWaiting;

        if (giveButtonUI != null && giveButtonUI.activeSelf != shouldShowButton)
        {
            giveButtonUI.SetActive(shouldShowButton);
            Debug.Log("Give button set to: " + shouldShowButton +
                " (lookingAtCustomer=" + lookingAtCustomer +
                ", customerWaiting=" + customerWaiting + ")");
        }

        if (!shouldShowButton && notEnoughItemText != null && notEnoughItemText.activeSelf)
            notEnoughItemText.SetActive(false);
    }

    void SpawnNextCustomer()
    {
        if (customerPrefab == null || spawnPoint == null || waitPoint == null || exitPoint == null)
        {
            Debug.LogWarning("CustomerManager: missing references, cannot spawn.");
            return;
        }

        if (possibleRequests == null || possibleRequests.Length == 0)
        {
            Debug.LogWarning("CustomerManager: no possible requests assigned.");
            return;
        }

        ItemData request = possibleRequests[Random.Range(0, possibleRequests.Length)];

        currentCustomer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        currentCustomer.Setup(this, request, waitPoint, exitPoint);

        Debug.Log("Spawned customer wanting: " + request.itemName);
    }

    // Hook this up to the Give button's OnClick in the Inspector
    public void OnGiveButtonPressed()
    {
        Debug.Log("===== GIVE BUTTON PRESSED =====");

        if (currentCustomer == null)
        {
            Debug.Log("Give failed: no current customer.");
            return;
        }

        if (currentCustomer.CurrentState != Customer.State.Waiting)
        {
            Debug.Log("Give failed: customer state is " + currentCustomer.CurrentState + " (not Waiting).");
            return;
        }

        ItemData wanted = currentCustomer.WantedItem;

        Debug.Log("Customer wants: " + wanted.itemName);

        if (InventoryManager.Instance == null)
        {
            Debug.Log("Give failed: InventoryManager.Instance is null.");
            return;
        }

        if (!InventoryManager.Instance.HasItem(wanted, 1))
        {
            Debug.Log("Give failed: player does not have " + wanted.itemName);

            if (notEnoughItemText != null)
                notEnoughItemText.SetActive(true);

            return;
        }

        InventoryManager.Instance.RemoveItem(wanted, 1);

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(wanted.sellPrice);
            Debug.Log("Customer paid £" + wanted.sellPrice + " for " + wanted.itemName);
        }

        Debug.Log("Success! Gave " + wanted.itemName + " to customer.");

        if (giveButtonUI != null)
            giveButtonUI.SetActive(false);

        if (notEnoughItemText != null)
            notEnoughItemText.SetActive(false);

        currentCustomer.ReceiveItem();
    }

    // Called by Customer once it reaches the exit point and despawns
    public void OnCustomerFinished(Customer customer)
    {
        if (customer == currentCustomer)
            currentCustomer = null;

        Invoke(nameof(SpawnNextCustomer), delayBetweenCustomers);
    }
}
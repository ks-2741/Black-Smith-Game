using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Customer : MonoBehaviour
{
    public enum State
    {
        MovingToWaitPoint,
        Waiting,
        MovingToExit,
        Done
    }

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Speech Bubble UI")]
    public GameObject speechBubble;
    public Image itemIcon;
    public TMP_Text requestText;

    [Header("Give Button (optional - auto-found if left empty)")]
    public Button giveButton;

    public State CurrentState { get; private set; }
    public ItemData WantedItem { get; private set; }

    private Transform waitPoint;
    private Transform exitPoint;
    private CustomerManager manager;

    public void Setup(CustomerManager owningManager, ItemData itemWanted, Transform waitPos, Transform exitPos)
    {
        // Force the whole customer + its children active, no matter what state
        // the prefab/instance was saved in.
        gameObject.SetActive(true);

        manager = owningManager;
        WantedItem = itemWanted;
        waitPoint = waitPos;
        exitPoint = exitPos;

        CurrentState = State.MovingToWaitPoint;

        if (speechBubble != null)
            speechBubble.SetActive(false);

        // Auto-find the give button among children if not assigned in the Inspector.
        if (giveButton == null)
            giveButton = GetComponentInChildren<Button>(true);

        if (giveButton != null)
        {
            // Force it active and force-wire the click via code, so it works
            // even if it starts disabled or its Inspector OnClick() got wiped.
            giveButton.gameObject.SetActive(true);
            giveButton.onClick.RemoveListener(OnGiveButtonClicked);
            giveButton.onClick.AddListener(OnGiveButtonClicked);
        }
        else
        {
            Debug.LogWarning("Customer: no Button found in children for the give button.");
        }
    }

    void OnGiveButtonClicked()
    {
        if (manager != null)
            manager.OnGiveButtonPressed();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case State.MovingToWaitPoint:
                MoveTowards(waitPoint.position);

                if (Vector3.Distance(transform.position, waitPoint.position) < 0.1f)
                {
                    CurrentState = State.Waiting;
                    ShowRequest();
                }
                break;

            case State.Waiting:
                // Standing here, waiting for the player to bring the item
                break;

            case State.MovingToExit:
                MoveTowards(exitPoint.position);

                if (Vector3.Distance(transform.position, exitPoint.position) < 0.1f)
                {
                    CurrentState = State.Done;
                    manager.OnCustomerFinished(this);
                    Destroy(gameObject);
                }
                break;
        }
    }

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime);
    }

    void ShowRequest()
    {
        if (speechBubble != null)
            speechBubble.SetActive(true);

        if (itemIcon != null && WantedItem.icon != null)
            itemIcon.sprite = WantedItem.icon;

        if (requestText != null)
            requestText.text = "I need a " + WantedItem.itemName + "!";

        Debug.Log("Customer wants: " + WantedItem.itemName);
    }

    // Called by CustomerManager once the player successfully hands over the item
    public void ReceiveItem()
    {
        Debug.Log("Customer received " + WantedItem.itemName);

        if (speechBubble != null)
            speechBubble.SetActive(false);

        CurrentState = State.MovingToExit;
    }
}
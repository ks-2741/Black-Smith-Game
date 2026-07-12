using UnityEngine;
using UnityEngine.UI;

// Runs before other scripts, so it fixes things up before anything else has a chance to break them.
[DefaultExecutionOrder(-1000)]
public class ForceCustomerSetup : MonoBehaviour
{
    [Header("Force these active on Play")]
    public GameObject[] objectsToForceActive;

    [Header("Force this button to call CustomerManager.OnGiveButtonPressed")]
    public Button giveButton;

    [Header("Safety Net")]
    [Tooltip("If true, keeps re-checking every frame in case something disables things again mid-play.")]
    public bool keepReChecking = true;

    void Awake()
    {
        FixEverything();
    }

    void Update()
    {
        if (keepReChecking)
            FixEverything();
    }

    void FixEverything()
    {
        // Force objects active
        if (objectsToForceActive != null)
        {
            foreach (GameObject obj in objectsToForceActive)
            {
                if (obj != null && !obj.activeSelf)
                {
                    obj.SetActive(true);
                    Debug.Log("[ForceCustomerSetup] Force-enabled: " + obj.name);
                }
            }
        }

        // Force the Give button's listener to be wired via code,
        // regardless of whether the Inspector OnClick() got wiped.
        if (giveButton != null)
        {
            // Remove first so we never double-fire if it's actually still there.
            giveButton.onClick.RemoveListener(CallGivePressed);
            giveButton.onClick.AddListener(CallGivePressed);
        }
    }

    void CallGivePressed()
    {
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.OnGiveButtonPressed();
        }
        else
        {
            Debug.LogWarning("[ForceCustomerSetup] CustomerManager.Instance is null, cannot give item.");
        }
    }
}
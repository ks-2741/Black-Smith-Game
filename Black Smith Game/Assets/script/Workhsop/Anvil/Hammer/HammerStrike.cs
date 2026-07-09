using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerStrike : MonoBehaviour
{
    [Header("Hammer")]
    public GameObject hammer;
    public Animator hammerAnimator;

    [Header("Timing")]
    public float animationTime = 0.5f;
    public float cooldown = 1f;

    private bool canStrike = true;

    void Start()
    {
        hammer.SetActive(false);
        Debug.Log("Hammer hidden on start.");
    }

    void Update()
    {
        // Only allow hammering in the Anvil camera
        if (!CameraSwitcher.Instance.IsCameraActive(CameraSwitcher.CameraView.Anvil))
            return;

        // Detect left mouse click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left mouse button pressed.");

            if (canStrike)
            {
                Debug.Log("Starting hammer strike.");
                StartCoroutine(StrikeHammer());
            }
            else
            {
                Debug.Log("Hammer is on cooldown.");
            }
        }
    }

    IEnumerator StrikeHammer()
    {
        canStrike = false;

        Debug.Log("Showing hammer.");
        hammer.SetActive(true);

        Debug.Log("Playing HammerStrike animation.");
        hammerAnimator.Play("HammerStrike", 0, 0);

        yield return new WaitForSeconds(animationTime);

        Debug.Log("Hiding hammer.");
        hammer.SetActive(false);

        Debug.Log("Cooldown started.");
        yield return new WaitForSeconds(cooldown);

        canStrike = true;
        Debug.Log("Cooldown finished. Hammer ready.");
    }
}
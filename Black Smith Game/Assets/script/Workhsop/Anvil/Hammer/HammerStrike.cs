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
        Debug.Log("Hammer hidden.");
    }

    void Update()
    {
        if (!CameraSwitcher.Instance.IsCameraActive(CameraSwitcher.CameraView.Anvil))
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Mouse Click");

            if (canStrike)
            {
                StartCoroutine(StrikeHammer());
            }
            else
            {
                Debug.Log("Hammer Cooling Down");
            }
        }
    }

    IEnumerator StrikeHammer()
    {
        canStrike = false;

        hammer.SetActive(true);

        Debug.Log("Playing HammerStrike animation");

        hammerAnimator.Play("HammerStrike", 0, 0);

        yield return new WaitForSeconds(animationTime);

        hammer.SetActive(false);

        yield return new WaitForSeconds(cooldown);

        canStrike = true;

        Debug.Log("Hammer Ready");
    }
}
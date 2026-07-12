using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance;


    public enum CameraView
    {
        Forge,
        Anvil,
        Furnace,
        Grindstone,
        Workbench
    }


    [Header("Cinemachine Cameras")]
    public CinemachineCamera[] cameras;


    [Header("Camera Buttons")]
    public Button[] cameraButtons;


    [Header("Camera UI")]
    public GameObject cameraButtonsUI;


    [Header("Switch Timing")]
    public float switchDelay = 0.5f;

    public float switchCooldown = 1f;



    [Header("Forging")]
    public ForgingGameManager forgingGameManager;


    [Header("Smelting")]
    public SmeltingGameManager smeltingGameManager;


    [Header("Grindstone")]
    public GrindstoneGameManager grindstoneGameManager;



    public int CurrentCamera { get; private set; }

    public CameraView CurrentView { get; private set; }



    private bool isSwitching;

    private bool minigameActive;



    private void Awake()
    {
        Instance = this;
    }



    private void Start()
    {
        Debug.Log("CameraSwitcher Loaded");

        CurrentCamera = -1;

        SwitchToCamera(0);
    }



    public void SwitchToCamera(int cameraIndex)
    {
        if (isSwitching)
            return;


        if (cameraIndex < 0 || cameraIndex >= cameras.Length)
        {
            Debug.LogWarning("Invalid camera index: " + cameraIndex);
            return;
        }


        if (cameraIndex == CurrentCamera)
            return;


        StartCoroutine(SwitchCameraRoutine(cameraIndex));
    }



    private IEnumerator SwitchCameraRoutine(int cameraIndex)
    {
        isSwitching = true;


        foreach (Button button in cameraButtons)
        {
            if (button != null)
                button.interactable = false;
        }



        yield return new WaitForSeconds(switchDelay);



        CurrentCamera = cameraIndex;

        CurrentView = (CameraView)cameraIndex;



        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority =
                (i == cameraIndex) ? 10 : 0;
        }



        Debug.Log("Switched to: " + CurrentView);



        // =========================
        // FORGING
        // =========================

        if (forgingGameManager != null)
        {
            if (CurrentView == CameraView.Anvil)
            {
                forgingGameManager.ShowStartButton();
            }
            else
            {
                forgingGameManager.HideStartButton();
            }
        }



        // =========================
        // SMELTING
        // =========================

        if (smeltingGameManager != null)
        {
            if (CurrentView == CameraView.Furnace)
            {
                smeltingGameManager.ShowStartButton();
            }
            else
            {
                smeltingGameManager.HideStartButton();
            }
        }



        // =========================
        // GRINDSTONE
        // =========================

        if (grindstoneGameManager != null)
        {
            if (CurrentView == CameraView.Grindstone)
            {
                Debug.Log("Entered Grindstone view.");

                grindstoneGameManager.ShowStartButton();
            }
            else
            {
                grindstoneGameManager.HideStartButton();
            }
        }



        yield return new WaitForSeconds(switchCooldown);



        if (!minigameActive)
        {
            for (int i = 0; i < cameraButtons.Length; i++)
            {
                if (cameraButtons[i] != null)
                {
                    cameraButtons[i].interactable =
                        (i != cameraIndex);
                }
            }
        }


        isSwitching = false;
    }




    public void SetMinigameActive(bool state)
    {
        minigameActive = state;


        if (cameraButtonsUI != null)
            cameraButtonsUI.SetActive(!state);


        foreach (Button button in cameraButtons)
        {
            if (button != null)
                button.interactable = !state;
        }
    }




    public bool IsCameraActive(CameraView view)
    {
        return CurrentView == view;
    }
}
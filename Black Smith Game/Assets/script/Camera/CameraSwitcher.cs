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

    [Header("Switch Timing")]
    [Tooltip("Delay before the camera switches.")]
    public float switchDelay = 0.5f;

    [Tooltip("Delay after arriving before another switch is allowed.")]
    public float switchCooldown = 1f;


    [Header("Forging")]
    public ForgingGameManager forgingGameManager;


    [Header("Smelting")]
    public SmeltingGameManager smeltingGameManager;


    public int CurrentCamera { get; private set; }
    public CameraView CurrentView { get; private set; }


    private bool isSwitching;


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
            cameras[i].Priority = (i == cameraIndex) ? 10 : 0;
        }



        Debug.Log("Switched to: " + CurrentView);



        // =========================
        // FORGING STATION
        // =========================

        if (forgingGameManager != null)
        {
            if (CurrentView == CameraView.Anvil)
            {
                Debug.Log("Entered Anvil view.");

                forgingGameManager.ShowStartButton();
            }
            else
            {
                forgingGameManager.HideStartButton();
            }
        }



        // =========================
        // SMELTING STATION
        // =========================

        if (smeltingGameManager != null)
        {
            if (CurrentView == CameraView.Furnace)
            {
                Debug.Log("Entered Furnace view.");

                smeltingGameManager.ShowStartButton();
            }
            else
            {
                smeltingGameManager.HideStartButton();
            }
        }



        yield return new WaitForSeconds(switchCooldown);



        for (int i = 0; i < cameraButtons.Length; i++)
        {
            if (cameraButtons[i] != null)
            {
                cameraButtons[i].interactable = (i != cameraIndex);
            }
        }


        isSwitching = false;
    }



    public bool IsCameraActive(CameraView view)
    {
        return CurrentView == view;
    }
}
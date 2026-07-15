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
        Workbench,
        SwordAssembly
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


    [System.Serializable]
    public class StationBinding
    {
        public CameraView view;
        public MonoBehaviour managerBehaviour; // drag any station manager here - it must implement IStationGameManager
        [System.NonSerialized] public IStationGameManager manager;
    }

    [Header("Stations")]
    public StationBinding[] stations;


    public int CurrentCamera { get; private set; }

    public CameraView CurrentView { get; private set; }



    private bool isSwitching;

    private bool minigameActive;



    private void Awake()
    {
        Instance = this;

        foreach (StationBinding binding in stations)
        {
            binding.manager = binding.managerBehaviour as IStationGameManager;

            if (binding.manager == null && binding.managerBehaviour != null)
            {
                Debug.LogWarning("CameraSwitcher: " + binding.managerBehaviour.name +
                    " does not implement IStationGameManager.");
            }
        }
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
        // STATIONS (generic loop - no need to edit this per station)
        // =========================

        foreach (StationBinding binding in stations)
        {
            if (binding.manager == null)
                continue;

            if (CurrentView == binding.view)
                binding.manager.ShowStartButton();
            else
                binding.manager.HideStartButton();
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
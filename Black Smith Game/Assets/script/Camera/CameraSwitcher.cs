using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance;

    public enum CameraView
    {
        Forge,
        Anvil,
        Grindstone,
        Workbench
    }

    [Header("Cinemachine Cameras")]
    public CinemachineCamera[] cameras;

    [Header("Forging")]
    public ForgingGameManager forgingGameManager;

    public int CurrentCamera { get; private set; }
    public CameraView CurrentView { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("CameraSwitcher Loaded");

        SwitchToCamera(0);
    }

    public void SwitchToCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= cameras.Length)
        {
            Debug.LogWarning("Invalid camera index: " + cameraIndex);
            return;
        }

        CurrentCamera = cameraIndex;
        CurrentView = (CameraView)cameraIndex;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == cameraIndex) ? 10 : 0;
        }

        Debug.Log("Switched to: " + CurrentView);

        if (forgingGameManager == null)
        {
            Debug.LogError("ForgingGameManager reference is missing!");
            return;
        }

        if (CurrentView == CameraView.Anvil)
        {
            Debug.Log("Player entered Anvil view.");

            forgingGameManager.ShowStartButton();
        }
        else
        {
            Debug.Log("Player left Anvil view.");

            forgingGameManager.HideStartButton();
        }
    }

    public bool IsCameraActive(CameraView view)
    {
        return CurrentView == view;
    }
}
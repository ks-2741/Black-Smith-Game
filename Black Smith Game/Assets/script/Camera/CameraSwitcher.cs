using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera[] camera;

   

    void Start()
    {
        SwitchToCamera(0);
    }

    public void SwitchToCamera(int cameraIndex)
    {
        for(int i = 0; i <camera.Length; i++)
        {
            camera[i].Priority = (i == cameraIndex) ? 10 : 0;
        }
    }
}
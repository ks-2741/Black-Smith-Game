using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;

    private bool usingCamera1 = true;

    void Start()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            usingCamera1 = !usingCamera1;

            camera1.SetActive(usingCamera1);
            camera2.SetActive(!usingCamera1);
        }
    }
}
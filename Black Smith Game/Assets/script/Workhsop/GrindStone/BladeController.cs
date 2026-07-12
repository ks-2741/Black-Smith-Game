using UnityEngine;
using UnityEngine.InputSystem;

public class BladeController : MonoBehaviour
{
    [Header("References")]
    public GrindstoneGameManager gameManager;
    public RectTransform track;

    [Header("Movement")]
    public float moveSpeed = 500f;

    private RectTransform blade;

    void Awake()
    {
        blade = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (gameManager == null)
        {
            Debug.Log("Blade: No Game Manager");
            return;
        }

        if (!gameManager.IsGrindingActive())
        {
            Debug.Log("Blade: Game not active");
            return;
        }

        Debug.Log("Blade: Running");



        if (CameraSwitcher.Instance == null)
            return;

        if (!CameraSwitcher.Instance.IsCameraActive(CameraSwitcher.CameraView.Grindstone))
            return;

        if (gameManager == null)
            return;

        if (!gameManager.IsGrindingActive())
            return;

        if (Keyboard.current == null)
            return;

        float move = 0f;

        if (Keyboard.current.aKey.isPressed)
            move = -1f;

        if (Keyboard.current.dKey.isPressed)
            move = 1f;

        Vector2 pos = blade.anchoredPosition;
        pos.x += move * moveSpeed * Time.deltaTime;

        float limit = (track.rect.width / 2f) - (blade.rect.width / 2f);

        pos.x = Mathf.Clamp(pos.x, -limit, limit);

        blade.anchoredPosition = pos;
    }
}
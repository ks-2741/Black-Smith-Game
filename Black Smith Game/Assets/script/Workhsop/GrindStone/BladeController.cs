using UnityEngine;
using UnityEngine.InputSystem;

public class BladeController : MonoBehaviour
{
    public float moveSpeed = 350f;
    public RectTransform track;

    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        float move = 0f;

        if (Keyboard.current.aKey.isPressed)
            move = -1f;

        if (Keyboard.current.dKey.isPressed)
            move = 1f;

        Vector2 pos = rt.anchoredPosition;
        pos.x += move * moveSpeed * Time.deltaTime;

        float limit = track.rect.width / 2f;

        pos.x = Mathf.Clamp(pos.x, -limit, limit);

        rt.anchoredPosition = pos;
    }
}
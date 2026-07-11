using UnityEngine;

public class LineMover : MonoBehaviour
{
    public float moveDistance = 250f;
    public float moveSpeed = 2f;

    RectTransform rt;
    Vector2 startPos;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        rt.anchoredPosition =
            new Vector2(startPos.x + x, startPos.y);
    }
}
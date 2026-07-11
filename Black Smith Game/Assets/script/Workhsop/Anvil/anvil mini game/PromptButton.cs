using UnityEngine;
using UnityEngine.UI;

public class PromptButton : MonoBehaviour
{
    private ForgingGameManager manager;

    private float timer;

    public bool Finished { get; private set; }

    public float TimeRemaining => timer;

    public void Setup(ForgingGameManager gameManager, float lifeTime)
    {
        manager = gameManager;

        timer = lifeTime;

        Finished = false;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClicked);

        RandomisePosition();

        Debug.Log("Prompt Spawned");
    }

    void Update()
    {
        if (Finished)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Finished = true;

            Debug.Log("Prompt Missed");
        }
    }

    void OnClicked()
    {
        if (Finished)
            return;

        Finished = true;

        manager.AddPoint();

        Debug.Log("Prompt Clicked");
    }

    void RandomisePosition()
    {
        RectTransform rt = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();

        float width = canvas.GetComponent<RectTransform>().rect.width;
        float height = canvas.GetComponent<RectTransform>().rect.height;

        float x = Random.Range(-width / 2 + 100, width / 2 - 100);
        float y = Random.Range(-height / 2 + 100, height / 2 - 100);

        rt.anchoredPosition = new Vector2(x, y);

        Debug.Log("Prompt Position = " + rt.anchoredPosition);
    }
}
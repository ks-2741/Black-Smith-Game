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
        RectTransform container =
            transform.parent.GetComponent<RectTransform>();

        RectTransform rt =
            GetComponent<RectTransform>();

        float x = Random.Range(
            -container.rect.width / 2 + 75,
             container.rect.width / 2 - 75);

        float y = Random.Range(
            -container.rect.height / 2 + 75,
             container.rect.height / 2 - 75);

        rt.anchoredPosition = new Vector2(x, y);

        Debug.Log("Prompt Position: " + rt.anchoredPosition);
    }
}
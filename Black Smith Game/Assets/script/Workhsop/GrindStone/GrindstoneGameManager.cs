using UnityEngine;
using TMPro;

public class GrindstoneGameManager : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject finishUI;

    public TMP_Text timerText;
    public TMP_Text resultText;

    public RectTransform blade;
    public RectTransform movingLine;

    public float gameLength = 10f;

    float timer;
    float score;

    bool playing;

    void Start()
    {
        finishUI.SetActive(false);
        timerText.text = "";
    }

    public void StartGame()
    {
        timer = gameLength;
        score = 0;

        playing = true;

        timerText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!playing)
            return;

        timer -= Time.deltaTime;

        timerText.text = timer.ToString("0.0");

        CheckAccuracy();

        if (timer <= 0)
            FinishGame();
    }

    void CheckAccuracy()
    {
        float distance =
            Mathf.Abs(blade.anchoredPosition.x -
                      movingLine.anchoredPosition.x);

        if (distance < 40)
            score += Time.deltaTime;
        else
            score -= Time.deltaTime;

        score = Mathf.Clamp(score, 0, gameLength);
    }

    void FinishGame()
    {
        playing = false;

        timerText.gameObject.SetActive(false);

        gameUI.SetActive(false);
        finishUI.SetActive(true);

        float percent = score / gameLength;

        if (percent > .9f)
            resultText.text = "Perfect";
        else if (percent > .7f)
            resultText.text = "Good";
        else if (percent > .5f)
            resultText.text = "OK";
        else if (percent > .3f)
            resultText.text = "Bad";
        else
            resultText.text = "Poor";
    }

    public void ContinueGame()
    {
        finishUI.SetActive(false);
        gameUI.SetActive(true);
    }
}
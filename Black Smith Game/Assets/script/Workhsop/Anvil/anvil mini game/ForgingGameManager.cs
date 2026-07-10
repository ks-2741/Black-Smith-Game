using System.Collections;
using UnityEngine;
using TMPro;

public class ForgingGameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startButton;
    public RectTransform promptContainer;
    public PromptButton promptPrefab;
    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Game Settings")]
    public int totalPrompts = 10;
    public float promptLifetime = 2f;

    private int score;
    private int promptsCompleted;

    void Start()
    {
        Debug.Log("ForgingGameManager Loaded");

        if (startButton == null)
            Debug.LogError("Start Button not assigned!");

        if (promptContainer == null)
            Debug.LogError("Prompt Container not assigned!");

        if (promptPrefab == null)
            Debug.LogError("Prompt Prefab not assigned!");

        startButton.SetActive(false);

        timerText.text = "";
        resultText.text = "";
    }

    public void ShowStartButton()
    {
        Debug.Log("Showing Start Button");
        startButton.SetActive(true);
    }

    public void HideStartButton()
    {
        Debug.Log("Hiding Start Button");
        startButton.SetActive(false);
    }

    public void StartForging()
    {
        Debug.Log("===== FORGING STARTED =====");

        HideStartButton();

        score = 0;
        promptsCompleted = 0;

        resultText.text = "";

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (promptsCompleted < totalPrompts)
        {
            Debug.Log($"Round {promptsCompleted + 1}");

            PromptButton prompt =
                Instantiate(promptPrefab, promptContainer);

            prompt.Setup(this, promptLifetime);

            while (!prompt.Finished)
            {
                timerText.text = prompt.TimeRemaining.ToString("0.0");
                yield return null;
            }

            promptsCompleted++;

            Destroy(prompt.gameObject);

            yield return new WaitForSeconds(0.25f);
        }

        FinishGame();
    }

    public void AddPoint()
    {
        score++;

        Debug.Log("Correct! Score = " + score);
    }

    void FinishGame()
    {
        timerText.text = "";

        float percent = (float)score / totalPrompts;

        string rank;

        if (percent >= 0.9f)
            rank = "Perfect";
        else if (percent >= 0.7f)
            rank = "Good";
        else if (percent >= 0.5f)
            rank = "OK";
        else if (percent >= 0.3f)
            rank = "Bad";
        else
            rank = "Poor";

        resultText.text = rank;

        Debug.Log("===== GAME OVER =====");
        Debug.Log("Final Score: " + score + "/" + totalPrompts);
        Debug.Log("Rank: " + rank);

        ShowStartButton();
    }
}
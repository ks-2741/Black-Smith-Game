using System.Collections;
using UnityEngine;
using TMPro;

public class ForgingGameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject stationButtons;

    public RectTransform promptContainer;
    public PromptButton promptPrefab;

    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Game Settings")]
    public int totalPrompts = 10;
    public float promptLifetime = 2f;

    private int score;
    private int promptsCompleted;
    private Coroutine gameLoop;

    [Header("Results Screen")]
    public GameObject gameplayUI;
    public GameObject resultsCanvas;

    public TMP_Text weaponNameText;
    public TMP_Text qualityText;
    public TMP_Text sellValueText;


    void Start()
    {
        Debug.Log("ForgingGameManager Loaded");

        if (startButton == null)
            Debug.LogError("Start Button not assigned!");

        if (exitButton == null)
            Debug.LogError("Exit Button not assigned!");

        if (stationButtons == null)
            Debug.LogError("Station Buttons not assigned!");

        if (promptContainer == null)
            Debug.LogError("Prompt Container not assigned!");

        if (promptPrefab == null)
            Debug.LogError("Prompt Prefab not assigned!");

        startButton.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(false);

        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
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

        if (stationButtons != null)
            stationButtons.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(true);

        score = 0;
        promptsCompleted = 0;

        timerText.text = "";
        resultText.text = "";

        timerText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);

        gameLoop = StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        Debug.Log("GameLoop Started");

        while (promptsCompleted < totalPrompts)
        {
            Debug.Log("Round " + (promptsCompleted + 1));

            PromptButton prompt = Instantiate(promptPrefab, promptContainer);

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

    public void ExitForging()
    {
        Debug.Log("Exited Forging");

        if (gameLoop != null)
            StopCoroutine(gameLoop);

        foreach (Transform child in promptContainer)
        {
            Destroy(child.gameObject);
        }

        timerText.text = "";
        resultText.text = "";

        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        if (exitButton != null)
            exitButton.SetActive(false);

        ShowStartButton();
    }
    public void ContinueForging()
    {
        resultsCanvas.SetActive(false);

        gameplayUI.SetActive(true);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        if (exitButton != null)
            exitButton.SetActive(false);

        ShowStartButton();

        resultText.text = "";
        timerText.text = "";
    }

    void FinishGame()
    {
        timerText.text = "";
        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(true);

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
        Debug.Log("Score: " + score + "/" + totalPrompts);
        Debug.Log("Rank: " + rank);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        if (exitButton != null)
            exitButton.SetActive(false);
        timerText.gameObject.SetActive(false);

        gameplayUI.SetActive(false);

        resultsCanvas.SetActive(true);

        weaponNameText.text = "Iron Sword";

        qualityText.text = rank;

        int sellValue = Mathf.RoundToInt(120 * percent);

        sellValueText.text = "£" + sellValue;

        ShowStartButton();
    }
}
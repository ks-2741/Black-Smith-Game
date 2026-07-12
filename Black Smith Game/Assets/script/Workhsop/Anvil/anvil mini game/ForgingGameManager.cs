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

    [Header("Inventory / Crafting")]
    public ItemData ingotItem;      // item consumed to start forging (output of smelting)
    public ItemData roughBladeItem; // item produced when forging finishes
    public TMP_Text notEnoughMaterialsText; // optional, shows a message if no ingot

    public bool IsForgingActive { get; private set; }


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

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);
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

        // Require an ingot before allowing the minigame to start
        if (ingotItem != null)
        {
            if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(ingotItem, 1))
            {
                Debug.Log("Not enough ingots to forge.");

                if (notEnoughMaterialsText != null)
                    ShowNotEnoughMaterials("Need " + ingotItem.itemName + " to forge!");

                return; // block starting the minigame
            }

            InventoryManager.Instance.RemoveItem(ingotItem, 1);
        }

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);

        IsForgingActive = true;

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

        IsForgingActive = false;

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
        Debug.Log("Continue pressed");

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
        IsForgingActive = false;

        timerText.text = "";
        timerText.gameObject.SetActive(false);

        // Hide the old gameplay result text
        resultText.gameObject.SetActive(false);

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

        Debug.Log("===== GAME OVER =====");
        Debug.Log("Score: " + score + "/" + totalPrompts);
        Debug.Log("Rank: " + rank);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        if (exitButton != null)
            exitButton.SetActive(false);

        gameplayUI.SetActive(false);
        resultsCanvas.SetActive(true);

        weaponNameText.text = "Iron Blade";

        // Keep this one
        qualityText.text = rank;

        int sellValue = Mathf.RoundToInt(120 * percent);

        sellValueText.text = "£" + sellValue;

        // Give the player a rough (forged) blade as long as forging didn't totally fail.
        if (roughBladeItem != null && percent > 0f && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(roughBladeItem, 1);
            Debug.Log("Added 1x " + roughBladeItem.itemName + " to inventory");
        }
    }

    private Coroutine notEnoughMaterialsRoutine;

    void ShowNotEnoughMaterials(string message)
    {
        if (notEnoughMaterialsText == null)
            return;

        if (notEnoughMaterialsRoutine != null)
            StopCoroutine(notEnoughMaterialsRoutine);

        notEnoughMaterialsRoutine = StartCoroutine(NotEnoughMaterialsRoutine(message));
    }

    IEnumerator NotEnoughMaterialsRoutine(string message)
    {
        notEnoughMaterialsText.text = message;
        notEnoughMaterialsText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notEnoughMaterialsText.gameObject.SetActive(false);
    }
}
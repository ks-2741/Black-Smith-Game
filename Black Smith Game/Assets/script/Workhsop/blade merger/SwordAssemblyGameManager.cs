using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwordAssemblyGameManager : MonoBehaviour, IStationGameManager
{
    [Header("UI")]
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject stationButtons;
    public GameObject gameplayUI;
    public GameObject resultsCanvas;

    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Results Screen")]
    public TMP_Text weaponNameText;
    public TMP_Text qualityText;
    public TMP_Text sellValueText;

    [Header("Grid")]
    public Transform gridParent;   // parent holding all the MemoryCell buttons
    private MemoryCell[] cells;

    [Header("Game Settings")]
    public int patternSize = 6;     // how many cells the player needs to remember
    public float showDuration = 3f; // how long the pattern is revealed before hiding
    public float inputTime = 8f;    // how long the player has to click them all

    [Header("Animator")]
    public Animator swordAnimator; // triggers an assembly animation on finish
    public string successTrigger = "AssembleSuccess";
    public string failTrigger = "AssembleFail";

    [Header("Inventory / Crafting")]
    public ItemData hiltItem;
    public ItemData crossguardItem;
    [Tooltip("An unsharpened blade - if this is what gets used, the result is a dull sword that still needs the Grindstone.")]
    public ItemData roughBladeItem;
    [Tooltip("A sharpened blade - if this is what gets used, the result is the final finished sword.")]
    public ItemData sharpBladeItem;
    [Tooltip("Assembled sword, not yet sharpened (used if built with a rough blade).")]
    public ItemData unsharpSwordItem;
    [Tooltip("The final, finished, sharpened sword (used if built with a sharp blade).")]
    public ItemData sharpSwordItem;
    public TMP_Text notEnoughMaterialsText;

    public bool IsAssemblyActive { get; private set; }

    private enum BladeSource { None, Rough, Sharp }
    private BladeSource currentBladeSource = BladeSource.None;

    private List<int> patternIndices = new List<int>();
    private int correctClicksNeeded;
    private int correctClicks;
    private int wrongClicks;
    private Coroutine gameRoutine;
    private Coroutine notEnoughMaterialsRoutine;

    void Start()
    {
        Debug.Log("SwordAssemblyGameManager Loaded");

        if (startButton != null)
            startButton.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);

        BuildGrid();
    }

    void BuildGrid()
    {
        if (gridParent == null)
        {
            Debug.LogError("SwordAssemblyGameManager: Grid Parent not assigned!");
            return;
        }

        cells = gridParent.GetComponentsInChildren<MemoryCell>(true);

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Setup(this, i);
        }

        Debug.Log("Sword Assembly grid built with " + cells.Length + " cells.");
    }

    public void ShowStartButton()
    {
        if (startButton != null)
            startButton.SetActive(true);
    }

    public void HideStartButton()
    {
        if (startButton != null)
            startButton.SetActive(false);
    }

    public void StartAssembly()
    {
        Debug.Log("===== SWORD ASSEMBLY STARTED =====");

        currentBladeSource = BladeSource.None;

        bool hasHiltAndGuard =
            InventoryManager.Instance != null &&
            hiltItem != null && crossguardItem != null &&
            InventoryManager.Instance.HasItem(hiltItem, 1) &&
            InventoryManager.Instance.HasItem(crossguardItem, 1);

        if (!hasHiltAndGuard)
        {
            Debug.Log("Not enough parts to assemble a sword - missing Hilt/Crossguard.");
            ShowNotEnoughMaterials("Need a Hilt and Crossguard!");
            return;
        }

        // Prefer a sharp blade if the player has one (skips needing the grindstone afterwards),
        // otherwise fall back to a rough blade (will need sharpening after assembly).
        if (sharpBladeItem != null && InventoryManager.Instance.HasItem(sharpBladeItem, 1))
        {
            currentBladeSource = BladeSource.Sharp;
        }
        else if (roughBladeItem != null && InventoryManager.Instance.HasItem(roughBladeItem, 1))
        {
            currentBladeSource = BladeSource.Rough;
        }
        else
        {
            Debug.Log("Not enough parts to assemble a sword - missing a Blade.");
            ShowNotEnoughMaterials("Need a Blade (sharp or unsharpened)!");
            return;
        }

        // Consume the parts now that we know we can proceed.
        InventoryManager.Instance.RemoveItem(hiltItem, 1);
        InventoryManager.Instance.RemoveItem(crossguardItem, 1);

        if (currentBladeSource == BladeSource.Sharp)
            InventoryManager.Instance.RemoveItem(sharpBladeItem, 1);
        else
            InventoryManager.Instance.RemoveItem(roughBladeItem, 1);

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);

        IsAssemblyActive = true;

        HideStartButton();

        if (stationButtons != null)
            stationButtons.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(true);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        if (resultText != null)
        {
            resultText.text = "";
            resultText.gameObject.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(true);
        }

        if (gameRoutine != null)
            StopCoroutine(gameRoutine);

        gameRoutine = StartCoroutine(RunMemoryGame());
    }

    IEnumerator RunMemoryGame()
    {
        correctClicks = 0;
        wrongClicks = 0;
        patternIndices.Clear();

        // ---- Pick a random pattern of cells ----
        List<int> allIndices = new List<int>();
        for (int i = 0; i < cells.Length; i++)
            allIndices.Add(i);

        int count = Mathf.Min(patternSize, allIndices.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, allIndices.Count);
            patternIndices.Add(allIndices[randomIndex]);
            allIndices.RemoveAt(randomIndex);
        }

        correctClicksNeeded = patternIndices.Count;

        Debug.Log("Pattern chosen: " + string.Join(",", patternIndices));

        // ---- Show phase: buttons disabled, pattern highlighted ----
        SetAllInteractable(false);

        foreach (int index in patternIndices)
            cells[index].SetHighlighted(true);

        float showTimer = showDuration;
        while (showTimer > 0)
        {
            if (timerText != null)
                timerText.text = "Memorize! " + showTimer.ToString("0");

            showTimer -= Time.deltaTime;
            yield return null;
        }

        // ---- Hide phase: un-highlight, re-enable for input ----
        foreach (int index in patternIndices)
            cells[index].SetHighlighted(false);

        SetAllInteractable(true);

        float inputTimer = inputTime;
        while (inputTimer > 0 && correctClicks + wrongClicks < correctClicksNeeded)
        {
            if (timerText != null)
                timerText.text = "Click them! " + inputTimer.ToString("0");

            inputTimer -= Time.deltaTime;
            yield return null;
        }

        FinishAssembly();
    }

    // Called by MemoryCell when clicked
    public void OnCellClicked(int index)
    {
        if (!IsAssemblyActive)
            return;

        cells[index].SetInteractable(false);

        if (patternIndices.Contains(index))
        {
            correctClicks++;
            cells[index].SetCorrect();
            Debug.Log("Correct cell clicked. " + correctClicks + "/" + correctClicksNeeded);
        }
        else
        {
            wrongClicks++;
            cells[index].SetWrong();
            Debug.Log("Wrong cell clicked.");
        }
    }

    void SetAllInteractable(bool state)
    {
        foreach (MemoryCell cell in cells)
            cell.SetInteractable(state);
    }

    void FinishAssembly()
    {
        Debug.Log("===== SWORD ASSEMBLY FINISHED =====");

        IsAssemblyActive = false;

        if (gameRoutine != null)
        {
            StopCoroutine(gameRoutine);
            gameRoutine = null;
        }

        HideStartButton();

        if (exitButton != null)
            exitButton.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(true);

        float quality = correctClicksNeeded > 0
            ? (float)correctClicks / correctClicksNeeded
            : 0f;

        string rank;

        if (quality >= 0.9f)
            rank = "Perfect";
        else if (quality >= 0.7f)
            rank = "Good";
        else if (quality >= 0.5f)
            rank = "OK";
        else if (quality >= 0.3f)
            rank = "Bad";
        else
            rank = "Poor";

        Debug.Log("Correct: " + correctClicks + "/" + correctClicksNeeded + " Rank: " + rank);

        bool willBeSharp = currentBladeSource == BladeSource.Sharp;

        if (weaponNameText != null)
            weaponNameText.text = willBeSharp ? "Iron Sword" : "Iron Sword (Unsharpened)";

        if (qualityText != null)
            qualityText.text = rank;

        int sellValue = Mathf.RoundToInt(150 * quality);

        if (sellValueText != null)
            sellValueText.text = "£" + sellValue;

        // Play the animator based on outcome
        if (swordAnimator != null)
        {
            if (quality > 0f)
                swordAnimator.SetTrigger(successTrigger);
            else
                swordAnimator.SetTrigger(failTrigger);
        }

        // Produce the correct output depending on which blade was used.
        if (quality > 0f && InventoryManager.Instance != null)
        {
            if (willBeSharp && sharpSwordItem != null)
            {
                InventoryManager.Instance.AddItem(sharpSwordItem, 1);
                Debug.Log("Added 1x " + sharpSwordItem.itemName + " to inventory (finished)");
            }
            else if (!willBeSharp && unsharpSwordItem != null)
            {
                InventoryManager.Instance.AddItem(unsharpSwordItem, 1);
                Debug.Log("Added 1x " + unsharpSwordItem.itemName + " to inventory (needs sharpening)");
            }
        }

        if (stationButtons != null)
            stationButtons.SetActive(true);
    }

    public void ContinueAssembly()
    {
        Debug.Log("Continue pressed");

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        ShowStartButton();

        ResetGrid();
    }

    public void ExitAssembly()
    {
        Debug.Log("Exited Sword Assembly");

        IsAssemblyActive = false;

        if (gameRoutine != null)
        {
            StopCoroutine(gameRoutine);
            gameRoutine = null;
        }

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        if (exitButton != null)
            exitButton.SetActive(false);

        if (stationButtons != null)
            stationButtons.SetActive(true);

        ResetGrid();

        ShowStartButton();
    }

    void ResetGrid()
    {
        if (cells == null)
            return;

        foreach (MemoryCell cell in cells)
        {
            cell.SetHighlighted(false);
            cell.SetInteractable(false);
        }
    }

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
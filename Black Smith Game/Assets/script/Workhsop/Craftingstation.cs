using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RecipeInput
{
    public ItemData item;
    public int amount = 1;
}

[System.Serializable]
public class Recipe
{
    public string recipeName = "Recipe";
    public List<RecipeInput> inputs = new List<RecipeInput>();
    public ItemData outputItem;
    public int outputAmount = 1;
}

public enum StationInteractionMode
{
    Hold,   // hold left click, progress fills while held, cancels if released early (e.g. Anvil hammering)
    Toggle  // click once to start, runs automatically to completion on its own (e.g. Furnace)
}

// One script, reused for every station (Furnace, Anvil, Grindstone, Assembler, and
// any future station). Configure the recipes + interaction mode + skill in the
// Inspector - no new code needed to add a station. All interaction is left-click,
// while the player is standing in the station's trigger zone.
public class CraftingStation : MonoBehaviour, IStationGameManager
{
    [Header("UI")]
    public GameObject stationButtons; // shared station-select menu, if you still use one
    public GameObject promptUI;       // "Click to activate" / "Hold to hammer" prompt, shown by StationTrigger
    public GameObject progressUI;     // holds the progress bar while crafting
    public Image progressFill;        // Image with Fill Type = Radial/Horizontal
    public TMP_Text notEnoughMaterialsText;
    public TMP_Text craftedFlashText; // optional "Crafted!" popup

    [Header("Interaction")]
    public StationInteractionMode interactionMode = StationInteractionMode.Hold;

    [Header("Recipes")]
    [Tooltip("Checked in order - the first recipe whose inputs are all in stock is the one that gets crafted.")]
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Skill")]
    public SkillType relevantSkill;
    public float baseHoldTime = 3f;
    [Tooltip("Time reduced by this many seconds per skill level (never goes below minHoldTime).")]
    public float holdTimeReductionPerLevel = 0.2f;
    public float minHoldTime = 0.5f;

    private bool playerInRange;
    private float progress;
    private bool crafting;
    private Recipe currentRecipe;
    private Coroutine flashRoutine;
    private Coroutine toggleRoutine;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);

        if (progressUI != null)
            progressUI.SetActive(false);

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);

        if (craftedFlashText != null)
            craftedFlashText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Mouse.current == null)
            return;

        if (interactionMode == StationInteractionMode.Hold)
            HandleHoldMode();
        else
            HandleToggleMode();
    }

    // ---- HOLD MODE (e.g. Anvil - hold left click to hammer) ----

    void HandleHoldMode()
    {
        bool holding = Mouse.current.leftButton.isPressed;

        if (holding)
        {
            if (!crafting)
                TryStartCrafting();

            if (crafting)
                ProgressHoldCrafting();
        }
        else
        {
            if (crafting)
                CancelCrafting();
        }
    }

    void ProgressHoldCrafting()
    {
        float requiredTime = GetRequiredTime();

        progress += Time.deltaTime;

        if (progressFill != null)
            progressFill.fillAmount = progress / requiredTime;

        if (progress >= requiredTime)
            FinishCrafting();
    }

    // ---- TOGGLE MODE (e.g. Furnace - click once, runs on its own) ----

    void HandleToggleMode()
    {
        if (crafting)
            return; // already running, ignore further clicks until it finishes

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (TryStartCrafting())
                toggleRoutine = StartCoroutine(ToggleCraftRoutine());
        }
    }

    IEnumerator ToggleCraftRoutine()
    {
        float requiredTime = GetRequiredTime();
        float timer = 0f;

        while (timer < requiredTime)
        {
            timer += Time.deltaTime;

            if (progressFill != null)
                progressFill.fillAmount = timer / requiredTime;

            yield return null;
        }

        FinishCrafting();
    }

    // ---- SHARED ----

    bool TryStartCrafting()
    {
        currentRecipe = FindAvailableRecipe();

        if (currentRecipe == null)
        {
            ShowNotEnoughMaterials("Missing materials!");
            return false;
        }

        crafting = true;
        progress = 0f;

        if (progressUI != null)
            progressUI.SetActive(true);

        if (progressFill != null)
            progressFill.fillAmount = 0f;

        return true;
    }

    Recipe FindAvailableRecipe()
    {
        if (InventoryManager.Instance == null)
            return null;

        foreach (Recipe recipe in recipes)
        {
            bool hasAll = true;

            foreach (RecipeInput input in recipe.inputs)
            {
                if (!InventoryManager.Instance.HasItem(input.item, input.amount))
                {
                    hasAll = false;
                    break;
                }
            }

            if (hasAll)
                return recipe;
        }

        return null;
    }

    float GetRequiredTime()
    {
        int level = SkillManager.Instance != null ? SkillManager.Instance.GetLevel(relevantSkill) : 0;

        float time = baseHoldTime - (holdTimeReductionPerLevel * level);
        return Mathf.Max(minHoldTime, time);
    }

    void FinishCrafting()
    {
        crafting = false;

        if (toggleRoutine != null)
        {
            StopCoroutine(toggleRoutine);
            toggleRoutine = null;
        }

        if (progressUI != null)
            progressUI.SetActive(false);

        if (currentRecipe == null || InventoryManager.Instance == null)
            return;

        foreach (RecipeInput input in currentRecipe.inputs)
            InventoryManager.Instance.RemoveItem(input.item, input.amount);

        if (currentRecipe.outputItem != null)
        {
            InventoryManager.Instance.AddItem(currentRecipe.outputItem, currentRecipe.outputAmount);
            Debug.Log("Crafted " + currentRecipe.outputAmount + "x " + currentRecipe.outputItem.itemName);
            FlashCrafted("Crafted " + currentRecipe.outputItem.itemName + "!");
        }

        currentRecipe = null;
    }

    void CancelCrafting()
    {
        crafting = false;
        progress = 0f;
        currentRecipe = null;

        if (toggleRoutine != null)
        {
            StopCoroutine(toggleRoutine);
            toggleRoutine = null;
        }

        if (progressUI != null)
            progressUI.SetActive(false);

        if (progressFill != null)
            progressFill.fillAmount = 0f;
    }

    void FlashCrafted(string message)
    {
        if (craftedFlashText == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(message));
    }

    IEnumerator FlashRoutine(string message)
    {
        craftedFlashText.text = message;
        craftedFlashText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        craftedFlashText.gameObject.SetActive(false);
    }

    void ShowNotEnoughMaterials(string message)
    {
        if (notEnoughMaterialsText == null)
            return;

        notEnoughMaterialsText.text = message;
        notEnoughMaterialsText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideNotEnoughMaterials));
        Invoke(nameof(HideNotEnoughMaterials), 2f);
    }

    void HideNotEnoughMaterials()
    {
        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);
    }

    // ---- IStationGameManager - called by StationTrigger when player enters/exits range ----

    public void ShowStartButton()
    {
        playerInRange = true;

        if (promptUI != null)
            promptUI.SetActive(true);
    }

    public void HideStartButton()
    {
        playerInRange = false;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (crafting)
            CancelCrafting();
    }
}
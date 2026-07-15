using System.Collections;
using UnityEngine;
using TMPro;

public class GrindstoneGameManager : MonoBehaviour, IStationGameManager
{
    [Header("UI")]
    public GameObject startButton;
    public GameObject stationButtons;
    public GameObject gameplayUI;
    public GameObject finishCanvas;

    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text valueText;


    [Header("References")]
    public RectTransform blade;
    public RectTransform targetZone;


    [Header("Target")]
    public TargetZoneMover targetMover;
    public GameObject targetZoneObject;


    [Header("Game Settings")]
    public float gameLength = 10f;

    [Header("Camera")]
    public CameraSwitcher cameraSwitcher;

    [Header("Inventory / Crafting")]
    [Tooltip("The unassembled, unsharpened blade (output of the Anvil).")]
    public ItemData roughBladeItem;
    [Tooltip("The unassembled, sharpened blade (used later at the Sword Assembler).")]
    public ItemData sharpBladeItem;
    [Tooltip("A fully assembled sword that hasn't been sharpened yet (output of the Sword Assembler when built with a rough blade).")]
    public ItemData unsharpSwordItem;
    [Tooltip("The final, finished, sharpened sword.")]
    public ItemData sharpSwordItem;
    public TMP_Text notEnoughMaterialsText; // optional, shows a message if there's nothing to grind


    private enum GrindMode { None, Blade, Sword }
    private GrindMode currentGrindMode = GrindMode.None;

    private bool active;

    private Coroutine gameRoutine;
    private Coroutine notEnoughMaterialsRoutine;

    private float score;


    void Start()
    {
        Debug.Log("GrindstoneGameManager Loaded");


        if (gameplayUI != null)
            gameplayUI.SetActive(false);


        if (finishCanvas != null)
            finishCanvas.SetActive(false);


        if (startButton != null)
            startButton.SetActive(false);


        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }


        if (resultText != null)
            resultText.text = "";

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);
    }



    public void ShowStartButton()
    {
        if (startButton != null)
            startButton.SetActive(true);

        Debug.Log("ShowStartButton called");
    }



    public void HideStartButton()
    {
        if (startButton != null)
            startButton.SetActive(false);
    }



    public void StartGrinding()
    {
        Debug.Log("===== GRINDING STARTED =====");

        currentGrindMode = GrindMode.None;

        // Decide what we're sharpening: a loose blade, or an already-assembled sword.
        // Whichever the player currently has gets consumed.
        if (roughBladeItem != null && InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(roughBladeItem, 1))
        {
            InventoryManager.Instance.RemoveItem(roughBladeItem, 1);
            currentGrindMode = GrindMode.Blade;
        }
        else if (unsharpSwordItem != null && InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(unsharpSwordItem, 1))
        {
            InventoryManager.Instance.RemoveItem(unsharpSwordItem, 1);
            currentGrindMode = GrindMode.Sword;
        }
        else
        {
            Debug.Log("Nothing to grind - no blade or unsharpened sword.");
            ShowNotEnoughMaterials("Need a Blade or an unsharpened Sword to grind!");
            return; // block starting the minigame
        }

        if (notEnoughMaterialsText != null)
            notEnoughMaterialsText.gameObject.SetActive(false);


        active = true;

        score = 0f;


        HideStartButton();

        if (stationButtons != null)
            stationButtons.SetActive(false);

        if (cameraSwitcher != null)
            cameraSwitcher.SetMinigameActive(true);


        if (finishCanvas != null)
            finishCanvas.SetActive(false);


        if (gameplayUI != null)
            gameplayUI.SetActive(true);



        if (targetZoneObject != null)
        {
            targetZoneObject.SetActive(true);
            Debug.Log("TargetZone Enabled");
        }



        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(true);
        }



        if (resultText != null)
            resultText.text = "";



        if (gameRoutine != null)
            StopCoroutine(gameRoutine);



        StartCoroutine(StartTargetMovementNextFrame());


        gameRoutine = StartCoroutine(GrindingTimer());
    }



    IEnumerator StartTargetMovementNextFrame()
    {
        yield return null;


        if (targetMover != null)
        {
            targetMover.StartMoving();
            Debug.Log("Target Movement Started");
        }
    }



    IEnumerator GrindingTimer()
    {
        float timer = gameLength;


        while (timer > 0)
        {
            timer -= Time.deltaTime;


            if (timerText != null)
                timerText.text = timer.ToString("0");


            CheckScore();


            yield return null;
        }


        FinishGrinding();
    }



    void CheckScore()
    {
        if (blade == null || targetZone == null)
            return;


        float distance =
            Mathf.Abs(
                blade.anchoredPosition.x -
                targetZone.anchoredPosition.x);



        float allowed =
            targetZone.rect.width / 2f;



        if (distance <= allowed)
            score += Time.deltaTime;
        else
            score -= Time.deltaTime * 0.5f;



        score = Mathf.Clamp(score, 0f, gameLength);
    }




    void FinishGrinding()
    {
        Debug.Log("===== GRINDING FINISHED =====");


        active = false;


        HideStartButton();

        if (stationButtons != null)
            stationButtons.SetActive(false);



        if (targetMover != null)
            targetMover.StopMoving();



        if (targetZoneObject != null)
            targetZoneObject.SetActive(false);



        if (gameplayUI != null)
            gameplayUI.SetActive(false);



        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }



        if (finishCanvas != null)
            finishCanvas.SetActive(true);



        float quality = score / gameLength;


        string result;


        if (quality >= 0.9f)
            result = "Perfect";
        else if (quality >= 0.7f)
            result = "Good";
        else if (quality >= 0.5f)
            result = "OK";
        else if (quality >= 0.3f)
            result = "Bad";
        else
            result = "Poor";



        if (resultText != null)
            resultText.text = result;

        int value = Mathf.RoundToInt(quality * 100);

        if (valueText != null)
            valueText.text = "£" + value;

        // Produce the correct output depending on what we sharpened.
        if (quality > 0f && InventoryManager.Instance != null)
        {
            if (currentGrindMode == GrindMode.Blade && sharpBladeItem != null)
            {
                InventoryManager.Instance.AddItem(sharpBladeItem, 1);
                Debug.Log("Added 1x " + sharpBladeItem.itemName + " to inventory");
            }
            else if (currentGrindMode == GrindMode.Sword && sharpSwordItem != null)
            {
                InventoryManager.Instance.AddItem(sharpSwordItem, 1);
                Debug.Log("Added 1x " + sharpSwordItem.itemName + " to inventory");
            }
        }

        if (cameraSwitcher != null)
            cameraSwitcher.SetMinigameActive(true);



        Debug.Log("Result: " + result);
        Debug.Log("Quality: " + quality);
        Debug.Log("Value: " + value);
    }




    public void ContinueGrinding()
    {
        Debug.Log("Continue pressed");


        if (finishCanvas != null)
            finishCanvas.SetActive(false);



        if (stationButtons != null)
            stationButtons.SetActive(true);



        ShowStartButton();



        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }



        if (resultText != null)
            resultText.text = "";

        if (valueText != null)
            valueText.text = "";

        if (cameraSwitcher != null)
            cameraSwitcher.SetMinigameActive(false);
    }




    public void ExitGrinding()
    {
        Debug.Log("Exited Grinding");


        active = false;



        if (targetMover != null)
            targetMover.StopMoving();



        if (targetZoneObject != null)
            targetZoneObject.SetActive(false);



        if (gameRoutine != null)
        {
            StopCoroutine(gameRoutine);
            gameRoutine = null;
        }



        if (gameplayUI != null)
            gameplayUI.SetActive(false);



        if (finishCanvas != null)
            finishCanvas.SetActive(false);



        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }



        if (resultText != null)
            resultText.text = "";

        if (valueText != null)
            valueText.text = "";



        if (stationButtons != null)
            stationButtons.SetActive(true);

        if (cameraSwitcher != null)
            cameraSwitcher.SetMinigameActive(false);



        ShowStartButton();
    }




    public bool IsGrindingActive()
    {
        return active;
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
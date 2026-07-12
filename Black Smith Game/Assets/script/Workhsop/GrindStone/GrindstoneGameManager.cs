using System.Collections;
using UnityEngine;
using TMPro;

public class GrindstoneGameManager : MonoBehaviour
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


    private bool active;

    private Coroutine gameRoutine;

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
}

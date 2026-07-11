using System.Collections;
using UnityEngine;
using TMPro;

public class SmeltingGameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startButton;
    public GameObject smeltingMiniGameUI;
    public GameObject resultsCanvas;

    public TMP_Text timerText;

    public TMP_Text qualityText;
    public TMP_Text ingotNameText;
    public TMP_Text valueText;

    [Header("Game Settings")]
    public float smeltTime = 15f;

    [Header("Temperature")]
    public TemperatureController temperatureController;

    private bool active;
    private Coroutine smeltingRoutine;

    void Start()
    {
        Debug.Log("SmeltingGameManager Loaded");

        if (smeltingMiniGameUI != null)
            smeltingMiniGameUI.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (startButton != null)
            startButton.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);
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

    public void StartSmelting()
    {
        Debug.Log("===== SMELTING STARTED =====");

        active = true;

        HideStartButton();

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (smeltingMiniGameUI != null)
            smeltingMiniGameUI.SetActive(true);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(true);
        }

        if (temperatureController != null)
            temperatureController.StartTemperature();

        if (smeltingRoutine != null)
            StopCoroutine(smeltingRoutine);

        smeltingRoutine = StartCoroutine(SmeltingTimer());
    }

    IEnumerator SmeltingTimer()
    {
        float timer = smeltTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = timer.ToString("0");

            yield return null;
        }

        FinishSmelting();
    }

    void FinishSmelting()
    {
        Debug.Log("===== SMELTING FINISHED =====");

        active = false;

        HideStartButton();

        if (temperatureController != null)
            temperatureController.StopTemperature();

        if (smeltingMiniGameUI != null)
            smeltingMiniGameUI.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        if (resultsCanvas != null)
            resultsCanvas.SetActive(true);

        float quality = 0f;

        if (temperatureController != null)
            quality = temperatureController.goodTemperatureTime / smeltTime;

        string result;

        if (quality >= 0.9f)
            result = "Perfect Ingot";
        else if (quality >= 0.7f)
            result = "High Quality";
        else if (quality >= 0.5f)
            result = "Normal";
        else if (quality >= 0.3f)
            result = "Poor";
        else
            result = "Failed";

        if (qualityText != null)
            qualityText.text = result;

        if (ingotNameText != null)
            ingotNameText.text = "Iron Ingot";

        int value = Mathf.RoundToInt(quality * 100);

        if (valueText != null)
            valueText.text = "£" + value;

        Debug.Log("Result: " + result);
        Debug.Log("Quality: " + quality);
    }

    public void ContinueSmelting()
    {
        Debug.Log("Continue pressed");

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        ShowStartButton();

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }
    }

    public void ExitSmelting()
    {
        Debug.Log("Exited Smelting");

        active = false;

        if (smeltingRoutine != null)
        {
            StopCoroutine(smeltingRoutine);
            smeltingRoutine = null;
        }

        if (temperatureController != null)
            temperatureController.StopTemperature();

        if (smeltingMiniGameUI != null)
            smeltingMiniGameUI.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        ShowStartButton();
    }

    public bool IsSmeltingActive()
    {
        return active;
    }
}
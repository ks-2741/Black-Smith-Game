using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TemperatureController : MonoBehaviour
{
    [Header("Temperature")]
    public float temperature = 50f;

    public float increaseSpeed = 25f;
    public float decreaseSpeed = 15f;

    public float minTemperature = 0f;
    public float maxTemperature = 100f;

    [Header("Perfect Temperature Range")]
    public float perfectMin = 40f;
    public float perfectMax = 60f;

    [Header("UI")]
    public RectTransform temperatureMarker;
    public RectTransform temperatureBar;

    [Header("Game")]
    public SmeltingGameManager smeltingManager;

    public float goodTemperatureTime;

    private bool active;


    void Start()
    {
        UpdateMarker();
    }


    void Update()
    {
        if (!active)
            return;


        if (Keyboard.current.spaceKey.isPressed)
        {
            temperature += increaseSpeed * Time.deltaTime;
        }
        else
        {
            temperature -= decreaseSpeed * Time.deltaTime;
        }


        temperature = Mathf.Clamp(
            temperature,
            minTemperature,
            maxTemperature
        );


        if (IsPerfectTemperature())
        {
            goodTemperatureTime += Time.deltaTime;
        }


        UpdateMarker();
    }


    public void StartTemperature()
    {
        active = true;
        temperature = 50f;
        goodTemperatureTime = 0;

        UpdateMarker();
    }


    public void StopTemperature()
    {
        active = false;
    }


    bool IsPerfectTemperature()
    {
        return temperature >= perfectMin &&
               temperature <= perfectMax;
    }


    void UpdateMarker()
    {
        if (temperatureMarker == null)
            return;


        float percentage =
            temperature / maxTemperature;


        Vector2 position =
            temperatureMarker.anchoredPosition;


        position.y =
            percentage *
            temperatureBar.rect.height;


        temperatureMarker.anchoredPosition =
            position;
    }
}
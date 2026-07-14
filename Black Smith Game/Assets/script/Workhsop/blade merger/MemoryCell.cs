using UnityEngine;
using UnityEngine.UI;

public class MemoryCell : MonoBehaviour
{
    [Header("References")]
    public Button button;
    public Image highlightImage; // the visual that shows "this one is part of the pattern"

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    public int Index { get; private set; }

    private SwordAssemblyGameManager manager;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (highlightImage == null)
            highlightImage = GetComponent<Image>();
    }

    public void Setup(SwordAssemblyGameManager owningManager, int index)
    {
        manager = owningManager;
        Index = index;

        button.onClick.RemoveListener(OnClicked);
        button.onClick.AddListener(OnClicked);

        SetVisual(normalColor);
    }

    void OnClicked()
    {
        if (manager != null)
            manager.OnCellClicked(Index);
    }

    public void SetHighlighted(bool state)
    {
        SetVisual(state ? highlightColor : normalColor);
    }

    public void SetCorrect()
    {
        SetVisual(correctColor);
    }

    public void SetWrong()
    {
        SetVisual(wrongColor);
    }

    void SetVisual(Color color)
    {
        if (highlightImage != null)
            highlightImage.color = color;
    }

    public void SetInteractable(bool state)
    {
        if (button != null)
            button.interactable = state;
    }
}
using System.Collections;
using UnityEngine;
using TMPro;

public class CurrencyDisplay : MonoBehaviour
{
    public TMP_Text goldText;

    void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += Refresh;
            Refresh();
        }
        else
        {
            // CurrencyManager hasn't run its Awake yet - wait for it instead of giving up.
            StartCoroutine(WaitForManager());
        }
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= Refresh;
    }

    IEnumerator WaitForManager()
    {
        while (CurrencyManager.Instance == null)
            yield return null;

        CurrencyManager.Instance.OnCurrencyChanged += Refresh;
        Refresh();
    }

    void Refresh()
    {
        if (goldText == null)
        {
            Debug.LogWarning("CurrencyDisplay: Gold Text not assigned.");
            return;
        }

        if (CurrencyManager.Instance == null)
            return;

        goldText.text = "£" + CurrencyManager.Instance.CurrentGold;
    }
}
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Starting Balance")]
    public int startingGold = 0;

    public int CurrentGold { get; private set; }

    public event System.Action OnCurrencyChanged;

    void Awake()
    {
        Instance = this;
        CurrentGold = startingGold;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        CurrentGold += amount;

        Debug.Log("Gold added: +" + amount + " (Total: " + CurrentGold + ")");

        OnCurrencyChanged?.Invoke();
    }

    // Returns true if the player could afford it (and deducts the gold).
    // Returns false if they couldn't afford it (nothing is deducted).
    public bool SpendGold(int amount)
    {
        if (amount <= 0)
            return true; // free item, nothing to spend

        if (CurrentGold < amount)
        {
            Debug.Log("Not enough gold. Have " + CurrentGold + ", need " + amount);
            return false;
        }

        CurrentGold -= amount;

        Debug.Log("Gold spent: -" + amount + " (Total: " + CurrentGold + ")");

        OnCurrencyChanged?.Invoke();
        return true;
    }

    public bool CanAfford(int amount)
    {
        return CurrentGold >= amount;
    }
}
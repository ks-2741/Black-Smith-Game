using UnityEngine;

public class SmeltingStation : MonoBehaviour
{
    public GameObject smeltingPanel;
    public SmeltingGameManager smeltingGameManager;

    public void ShowPanel()
    {
        if (smeltingPanel != null)
            smeltingPanel.SetActive(true);
    }

    public void HidePanel()
    {
        if (smeltingGameManager != null)
            smeltingGameManager.ExitSmelting();

        if (smeltingPanel != null)
            smeltingPanel.SetActive(false);
    }
}
using UnityEngine;

public class AnvilGame : MonoBehaviour
{
    public GameObject panel;

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void ShidePanel()
    {
        panel.SetActive(false);
    }
}

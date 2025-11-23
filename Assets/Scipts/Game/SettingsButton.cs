using UnityEngine.UI;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public Button openSettings;
    public Button closeSettings;

    public void OpenSettings()
    {
        openSettings.gameObject.SetActive(false);
        closeSettings.gameObject.SetActive(true);
        closeSettings.interactable = true;
    }

    public void CloseSettings()
    {
        openSettings.gameObject.SetActive(true);
        openSettings.interactable = true;
        closeSettings.gameObject.SetActive(false);
    }
}

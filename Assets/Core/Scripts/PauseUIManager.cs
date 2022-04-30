using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseUIContainer;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject pauseBackground;

    [SerializeField] Button playButton, settingsButton, quitButton;

    public bool UIActive { get; set; }

    private void Awake()
    {
        pauseUIContainer.SetActive(false);
        settingsUI.SetActive(false);
        pauseBackground.SetActive(false);
    }

    public void OnSettingsPressed()
    {
        settingsUI.SetActive(true);
        pauseBackground.SetActive(false);
        playButton.interactable = false;
        settingsButton.interactable = false;
        quitButton.interactable = false;
    }

    public void OnBackPressed()
    {
        settingsUI.SetActive(false);
        pauseBackground.SetActive(true);
        playButton.interactable = true;
        settingsButton.interactable = true;
        quitButton.interactable = true;
    }

    public void ShowPauseUI()
    {
        pauseUIContainer.SetActive(true);
        pauseBackground.SetActive(true);

        playButton.interactable = true;
        settingsButton.interactable = true;
        quitButton.interactable = true;

        UIActive = true;
    }

    public void HidePauseUI()
    {
        pauseBackground.SetActive(false);
        settingsUI.SetActive(false);
        pauseUIContainer.SetActive(false);

        UIActive = false;
    }
}

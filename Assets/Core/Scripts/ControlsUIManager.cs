using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsUIManager : MonoBehaviour
{
    [SerializeField] private GameObject keyboardControlsContainer;
    [SerializeField] private GameObject gamepadControlsContainer;

    void Start()
    {
        keyboardControlsContainer.SetActive(true);
        gamepadControlsContainer.SetActive(false);
    }

    public void OnKeyboardButtonPressed()
    {
        gamepadControlsContainer.SetActive(false);
        keyboardControlsContainer.SetActive(true);
    }

    public void OnGamepadButtonPressed()
    {
        keyboardControlsContainer.SetActive(false);
        gamepadControlsContainer.SetActive(true);
    }
}

using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField] PauseUIManager pauseUIManager;

    public bool CanPause { get; private set; } = true;

    public bool paused = false;

    private ControlType cachedControlType;

    private void Awake()
    {
        Instance = this;
        //pauseUIManager = FindObjectOfType<PauseUIManager>();
    }

    public void SetCanPause(bool canPause)
    {
        CanPause = canPause;
    }

    public void ChangePauseState()
    {
        if (paused)
        {
            Unpause();
        }
        else
        {
            Pause(true);
        }
    }

    public void Pause(bool showUI)
    {
        if (!CanPause)
        {
            return;
        }

        cachedControlType = InputManager.GetCurrentControlType();

        InputManager.SetInputType(ControlType.NONE);

        if(cachedControlType == ControlType.MECH)
        {
            InputManager.MECH.Pause.Enable();
        }
        else
        {
            InputManager.VTOL.Pause.Enable();
        }

        CursorManager.EnableCursor("pause-ui");

        if(showUI)
        {
            pauseUIManager.ShowPauseUI();
        }
        paused = true;
    }

    public void Unpause()
    {
        /*if(!AIUI.Instance?.IsMapOpen() ?? false)
        {
            InputManager.SetInputType(cachedControlType);
        }
        else
        {
            InputManager.MECH.ToggleMap.Enable();
        }*/

        InputManager.SetInputType(cachedControlType);

        pauseUIManager.HidePauseUI();
        CursorManager.DisableCursor("pause-ui");
        paused = false;
    }
}
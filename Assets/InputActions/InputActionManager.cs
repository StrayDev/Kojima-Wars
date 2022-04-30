using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Please use the InputManager class instead
/// You can look at InputExample for an example
/// </summary>

public class InputActionManager : MonoBehaviour
{
    // INPUT ACTION ASSET
    public InputActions inputActions;
    // the current active map
    InputActionMap currentActionMap;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        inputActions = new InputActions();

        // enable ui
        //inputActions.UI.Enable();
        //currentActionMap = inputActions.UI;

        currentActionMap = inputActions.Mech;
        inputActions.Mech.Enable();
        //inputActions.Mech.Enable();
    }

    void Start()
    {

    }

    public void EnableInputActionMap(InputActionMap _actionMapToEnable)
    {
        if(currentActionMap != null)
        {
            currentActionMap.Disable();
        }
        _actionMapToEnable.Enable();
        currentActionMap = _actionMapToEnable;
        Debug.Log(currentActionMap);
    }

    InputActionMap GetCurrentActionMap()
    {
        return currentActionMap;
    }

    void Update()
    {
        
    }
}

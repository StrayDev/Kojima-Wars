using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlType
{
    NONE,
    MECH,
    VTOL,
    JET,
    MAPUI
}

/// <summary>
/// Use SetInputType(ControlType type) to set and activate the action map that you need.
/// Access the action map that you need through InputManager.Mech / .Vtol / .Jet, 
///
/// DO NOT manually enable or disable the ActionMaps.
///
/// Use GetCurrentType(ControlType type) if you want to check the current state. 
/// </summary>
/// 
public static class InputManager
{
    private static readonly InputActions _inputActions;
    private static InputActionMap _currentMap;

    public static InputActions.UIActions UI => _inputActions.UI;
    public static InputActions.MechActions MECH => _inputActions.Mech;
    public static InputActions.VTOLActions VTOL => _inputActions.VTOL;
    public static InputActions.JetActions JET => _inputActions.Jet;

    // static constructor is called the first time the class is accessed
    static InputManager()
    {
        // create the input actions before they are used
        _inputActions = new InputActions();

        UI.Enable();
        // defaults to Mech at player start
        _currentMap = MECH;
        _currentMap.Enable();
    }

    private static bool toggle = true;
    public static void ToggleInput()
    {
        //Debug.Log(toggle);
        if(toggle)
        {
            _currentMap.Disable();
            toggle = !toggle;
            return;
        }

        _currentMap.Enable();
        toggle = !toggle;
    }

    public static ControlType GetCurrentControlType()
    {
        if (_currentMap == (InputActionMap) MECH) return ControlType.MECH;
        if (_currentMap == (InputActionMap) VTOL) return ControlType.VTOL;
        return _currentMap == (InputActionMap) JET ? ControlType.JET : ControlType.NONE;
    }

    public static void SetInputType(ControlType type)
    {
        switch (type)
        {
            case ControlType.NONE:
                // disable all actions except UI
                _inputActions.Disable();
                UI.Enable();
                break;
            case ControlType.MECH:
                ChangeActionMap(MECH);
                break;
            case ControlType.VTOL:
                ChangeActionMap(VTOL);
                break;
            case ControlType.JET:
                ChangeActionMap(JET);
                break;
            default:
                Debug.LogError("Unknown ControlType in InputManager SetInputType()");
                break;
        }
    }

    private static void ChangeActionMap(InputActionMap actionMap)
    {
        _currentMap.Disable();
        _currentMap = actionMap;
        _currentMap.Enable();
    }
}

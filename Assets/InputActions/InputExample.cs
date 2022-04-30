using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Inherit from network behaviour instead of MonoBehaviour
// Make sure the GameObject has a NetworkObject component on it
public class InputExample : NetworkBehaviour 
{
    // Start is called before the first frame update
    void OnEnable()
    {
        // dont do anything if not the local player 
        if (!IsLocalPlayer) return;
        
        // Set the Input To the Type you Want 
        InputManager.SetInputType(ControlType.MECH);
        
        // use the hooks to set your callbacks in OnEnable()
        InputManager.MECH.Jump.performed += InputTest;
    }

    private void OnDisable()
    {
        // dont do anything if not the local player 
        if (!IsLocalPlayer) return;
        
        // make sure you are unsubscribing everything in OnDisable()
        InputManager.MECH.Jump.performed += InputTest;
    }

    private void InputTest(InputAction.CallbackContext context)
    {
        // use get current control type for comparisons
        var type = InputManager.GetCurrentControlType();

        if (type == ControlType.MECH)
        {
            Debug.Log("Robo go brrrrrrrr");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_InputManager : MonoBehaviour
{
    
    public CS_PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new CS_PlayerInput();
        playerInput.Enable();
        playerInput.PlayerGroundMovement.Enable();
    }

    private void OnEnable()
    {
        //playerInput.PlayerGroundMovement.Enable();
    }

    private void OnDisable()
    {
        //playerInput.PlayerGroundMovement.Disable();
    }
}

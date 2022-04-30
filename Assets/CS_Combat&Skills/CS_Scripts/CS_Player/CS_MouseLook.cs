using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_MouseLook : NetworkBehaviour
{
    public InputActionManager inputActions;

    [SerializeField] private Transform playerCamera;
    private float xClamp = 85f;
    private float xRotation;
    private CS_PlayerStats playerStats;
    //private CS_PlayerInput playerInput;

    //private CS_PlayerInput playerInput;

    float mX;
    float mY;

    private void Start()
    {
       
        inputActions.inputActions.Mech.View.performed += View;

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CS_PlayerStats>();
        playerStats.currentSensitivity = playerStats.unscopedSensitivity;
    }

    void Update()
    {
        // removed this as i am not sure this is being used and is paused can be private
        /*
        if (!Object.FindObjectOfType<CS_PauseManager>().isPaused)
        {
            Turn();
        }
        */
    }

    public void View(InputAction.CallbackContext context)
    {
        mX = context.ReadValue<Vector2>().x;
        mY = context.ReadValue<Vector2>().y;
    }

    void Turn()
    {
        mX = mX * playerStats.currentSensitivity * Time.deltaTime;
        mY = mY * playerStats.currentSensitivity * Time.deltaTime;

        xRotation -= mY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mX);
    }
}

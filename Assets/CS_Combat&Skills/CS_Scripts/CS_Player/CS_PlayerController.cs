using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class CS_PlayerController : MonoBehaviour
{
    [SerializeField] private CS_MouseLook mouseLook;
    public InputActionManager inputActions;


    private bool isGrounded;
    public bool playerStunned = false;

    private Rigidbody rb;
    Vector2 inputVector;
    
    private Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CS_PlayerStats playerStats;

    private const float maxHealth = 100f;
    //private float currentHealth = maxHealth;
    
    [SerializeField] private const float maxShields = 200f;
    [SerializeField] private float currentShields = maxShields;

    [SerializeField] private TextMeshProUGUI ShieldsText;
    [SerializeField] private TextMeshProUGUI HealthText;
    
    private void Awake()
    {
        groundCheck = GameObject.FindGameObjectWithTag("Ground Check").transform;
        rb = GetComponent<Rigidbody>();
        playerStats = GetComponent<CS_PlayerStats>();

        inputActions.inputActions.Mech.Jump.performed += Jump;
    }
    
    void FixedUpdate()
    {
        Vector2 inputVector = inputActions.inputActions.Mech.Movement.ReadValue<Vector2>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * playerStats.speed * Time.fixedDeltaTime);
        
    }
  
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Debug.Log("Jump");
            rb.AddForce(transform.up * playerStats.jumpForce);
        }
    }
}

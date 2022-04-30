using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechControllerScript : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheckObject;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float mouseSensitivity = 5;
    [SerializeField] public float movementSpeed = 5;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float gravityMultiplier = 3f;

    Vector3 horizontalInput;
    Vector3 verticalInput;
    Vector3 targetMovement;

    [SerializeField] private Rigidbody _rb;
    private StateMachine _mechStateMachine;
    public InputActionManager inputActionManager;

    private float lookX;
    private float lookY;

    private void Awake()
    {
        _mechStateMachine = new StateMachine();
    }

    private void Start()
    {
        if(!IsLocalPlayer)
        {
            foreach(var camera in GetComponentsInChildren<Camera>())
            {
                camera.enabled = false;
            }
            enabled = false;
            return;
        }

        _mechStateMachine.ChangeState(new Mech_IdleState(this));
        _rb = GetComponent<Rigidbody>();

        Physics.gravity *= gravityMultiplier;

        inputActionManager.inputActions.Mech.Jump.performed += Jump;
    }

    private void OnEnable()
    {
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    private void Update()
    {
        //_mechStateMachine.Update();
        //animator.SetFloat("speed", _moveAction.ReadValue<Vector2>().magnitude, 0.1f, Time.deltaTime);
        //PlayerAndCameraRotation(); Fix : conflict

        if (IsLocalPlayer)
        {
            Vector3 horizontalInput = inputActionManager.inputActions.Mech.Movement.ReadValue<Vector2>().x * transform.right;
            Vector3 verticalInput = inputActionManager.inputActions.Mech.Movement.ReadValue<Vector2>().y * transform.forward;

            _rb.velocity += (horizontalInput + verticalInput) * movementSpeed;
        }
    }

    private void FixedUpdate()
    {
        Move();
        if (IsLocalPlayer)
        {
            _rb.AddForce(Physics.gravity * (gravityMultiplier - 1) * _rb.mass);
        }
    }

    private void Move()
    {
        targetMovement = Vector3.MoveTowards(_rb.velocity, (horizontalInput + verticalInput).normalized, Time.deltaTime * movementSpeed * 2.0F);
        _rb.velocity = new Vector3(targetMovement.x, _rb.velocity.y, targetMovement.z);
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            Debug.Log("Jump");
            //movementVector += Vector3.up * jumpForce;
            _rb.velocity += (Vector3.up * jumpForce);
        }
    }

    private void PlayerAndCameraRotation()
    {
        float _lookX = inputActionManager.inputActions.Mech.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        //float _lookY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        lookX += _lookX;
        //lookY += _lookY;

        transform.rotation = Quaternion.Euler(0, lookX, 0);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckObject.position, groundCheckRadius, whatIsGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheckObject.position, groundCheckRadius);
    }
}

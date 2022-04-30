using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VariableFighter : MonoBehaviour
{
    public Rigidbody rb;
    public ConstantForce cf;
    public Camera playerCam;
    public GameObject jetObject;
    public InputActionManager inputActionManager;

    private float minFOV = 60.0f;
    private float maxFOV = 80.0f;

    [HideInInspector] public float pitchInput;
    [HideInInspector] public float rollInput;
    private float yawInput;

    public Transform gunPos;
    public Transform chasePos;

    //Modes
    [Header("Modes")]
    public bool isAiming = false;
    public bool isFiring = false;
    public bool isLanded = true;

    //Thrust
    [Header("Thrust Settings")]
    public float idleSpeed = 10.0f;
    public float maxSpeed = 10.0f;
    public float acceleration = 50.0f;
    private float thrustInput;
    private bool thrusting;

    //Gun
    [Header("Gun Settings")]
    public float spoolTime = 0.5f;
    public float fireRate = 0.2f;
    public float curFireRate = 0.0f;
    public float dispersalRadius = 0.1f;
    public float range = 5000.0f;
    public float overheatTime = 8.0f;

    //Rotations
    [Header("Rotation Settings")]
    public float rollSpeed = 2.0f;
    public float yawSpeed = 2.0f;
    public float pitchSpeed = 2.0f;


    public float deadzoneRadius = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
       //inputActionManager.inputActions.Jet.Look.performed += LookInput;
       //inputActionManager.inputActions.Jet.Thrust.performed += Move;
       //inputActionManager.inputActions.Jet.Fire.performed += Fire;
    }

    private void OnEnable()
    {
        rb.AddForce((transform.forward * acceleration), ForceMode.Force);
        rb.constraints = RigidbodyConstraints.None;
        jetObject.transform.Rotate(0, 0, 0);

    }

    private void OnDisable()
    {
    }

    void Update()
    {
        /*if(inputActionManager.inputActions.Jet.Thrust.WasPressedThisFrame())
        {
            thrusting = true;
        }
        else
        {
            thrusting = false;
        }*/

        /*if(inputActionManager.inputActions.Jet.Fire.WasPressedThisFrame())
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }*/

        JetThrust();
        JetRotation();
        JetFiring();
    }


    public void Move(InputAction.CallbackContext _value)
    { 
        thrustInput = _value.ReadValue<float>();
    }

    public void LookInput(InputAction.CallbackContext _value)
    {
        pitchInput = _value.ReadValue<Vector2>().y;
        rollInput = _value.ReadValue<Vector2>().x;
    }
    public void JetRotation()
    {
        pitchInput = (pitchInput - (Screen.height * 0.5f)) / (Screen.height * 0.5f);
        if (Mathf.Abs(pitchInput) < deadzoneRadius)
        {
            pitchInput = 0;
        }

        rollInput = (rollInput - (Screen.width * 0.5f)) / (Screen.width * 0.5f);
        if (Mathf.Abs(rollInput) < deadzoneRadius)
        {
            rollInput = 0;
        }

        rb.AddTorque(gameObject.transform.forward * rollInput * -rollSpeed * Time.deltaTime);
        rb.AddTorque(gameObject.transform.right * pitchInput * pitchSpeed * Time.deltaTime);
        rb.AddTorque(gameObject.transform.up * yawInput * -yawSpeed * Time.deltaTime);
    }

    public void Fire(InputAction.CallbackContext _value)
    {
        Debug.Log("PEW PEW");
        if (_value.performed)
        {
            isFiring = true;
        }
        else if (_value.canceled)
        {
            isFiring = false;
        }
    }

    public void Aim(InputAction.CallbackContext _value)
    {
        if (_value.performed)
        {
            isAiming = true;
            playerCam.transform.position = gunPos.position;
        }
        else if (_value.canceled)
        {
            isAiming = false;
            playerCam.transform.position = chasePos.position;
        }
    }

    void JetThrust()
    {

        if (thrusting)
        {
            cf.relativeForce = new Vector3(0, 5, 0);
            cf.force = new Vector3(0, 5, 0);

            if (!isAiming)
            {
                if (thrustInput > 0)
                {
                    playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, maxFOV, rb.velocity.magnitude / 10000);
                }
                else if (thrustInput < 0)
                {
                    playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, minFOV, rb.velocity.magnitude / 10000);
                }

            }
            if (rb.velocity.magnitude <= maxSpeed)
            {
                if (transform.InverseTransformDirection(rb.velocity).z >= 5)
                {
                    rb.AddForce((transform.forward * acceleration * thrustInput), ForceMode.Acceleration);
                }
            }

            if (rb.velocity.magnitude < 20)
            {
                cf.relativeForce = new Vector3(0, 0, 0);
                cf.force = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (!isAiming)
            {
                playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, 70, 0.05f);
            }

            if (rb.velocity.magnitude < idleSpeed)
            {
                rb.AddForce((transform.forward * acceleration / 2), ForceMode.Acceleration);
            }
            else if (rb.velocity.magnitude > idleSpeed)
            {
                rb.AddForce((transform.forward * -acceleration / 2), ForceMode.Acceleration);

            }
        }
    }

    void JetFiring()
    {
        if (isFiring)
        {
            Debug.Log("Firing");
            if (spoolTime > 0)
            {
                spoolTime -= Time.deltaTime;
            }
            else
            {
                if (overheatTime > 0)
                {

                    if (curFireRate > 0)
                    {
                        curFireRate -= Time.deltaTime;
                    }
                    else
                    {
                        curFireRate = fireRate;
                        RaycastHit hit;
                        if (Physics.Raycast(gunPos.position, transform.forward, out hit, range))
                        {
                            Debug.Log("Hit " + hit.collider.gameObject);
                            Debug.DrawRay(gunPos.position, transform.forward * range, Color.red, 2);

                        }
                        else
                        {
                            Debug.Log("Miss");
                            Debug.DrawRay(gunPos.position, transform.forward * range, Color.red, 2);
                        }
                    }
                }
            }
        }
        else if (!isFiring)
        {
            if (spoolTime < 0.5)
            {
                spoolTime += Time.deltaTime;
            }
        }
    }
}

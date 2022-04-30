using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VTOL : MonoBehaviour
{
    public InputActionManager inputActionManager;
/*    InputAction inputMoveX;
    InputAction inputMoveZ;
    InputAction inputMoveY;
    InputAction mouseRotX;*/

    Rigidbody rb;

    float velx;
    float vely;
    float velz;

    float rotx;
    float rotz;
    float roty;

    [Header("VTOL GameObject")]
    public GameObject vtolObject;

    [Header("Movement")]
    public float moveStopSpeed;
    public float moveAcceleration;
    public float maxMoveSpeed;
    public float stopSpeedY;
    public float yAcceleration;
    public float maxYMoveSpeed;
    public float oppositeDirectionMultiplier;


    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationAcceleration;
    public float maxAndMinRotation;
    public float mouseSensitivity;

    

    [Header("Velocity")]
    public Vector3 vel;

    bool yMove = false;
    bool moveX = false;
    bool moveZ = false;

    float timer;
   


    // Start is called before the first frame update
    void Start()
    {       
        //inputActionManager.inputActions.VTOL.MouseX.performed += Rotation;
        //inputActionManager.inputActions.VTOL.Move.performed += PlayerInput;


        //inputMap.Enable();
        rb = GetComponent<Rigidbody>();
        //rotx = transform.eulerAngles.x;
        roty = transform.eulerAngles.y;
        //rotz = transform.eulerAngles.z;
        
    }

    // Update is called once per frame
    void Update()
    {
        //sets the max speeds
        vely = Mathf.Clamp(vely, -maxYMoveSpeed, maxYMoveSpeed);
        velx = Mathf.Clamp(velx, -maxMoveSpeed, maxMoveSpeed);
        velz = Mathf.Clamp(velz, -maxMoveSpeed, maxMoveSpeed);



        vel = new Vector3(velx, vely, velz);

    }

    void Rotation(InputAction.CallbackContext _value)
    {
        if (!GetComponent<TransformController>().isEnabled)
        {
            //X and Z rotation
           // rotx = Mathf.Clamp(rotx, -maxAndMinRotation, maxAndMinRotation);
          //  rotz = Mathf.Clamp(rotz, -maxAndMinRotation - 10, maxAndMinRotation + 10);


            vtolObject.transform.rotation = Quaternion.Slerp(vtolObject.transform.rotation, Quaternion.Euler(rotz, transform.eulerAngles.y + 90, rotx), rotationSpeed * Time.deltaTime);

            //Y rotation with mouse
            //roty += inputActionManager.inputActions.VTOL.MouseX.ReadValue<float>() * mouseSensitivity * 10 * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, roty, transform.rotation.z), 2 * Time.deltaTime);
        }


    }

    void StopMovement()
    {
        //lerps velocity back to 0 when not moving
        if (!yMove)
        {
            vely = Mathf.Lerp(vely, 0, stopSpeedY * Time.deltaTime);

        }

        if (!moveX)
        {
            velx = Mathf.Lerp(velx, 0, moveStopSpeed * Time.deltaTime);
            

        }

        if (!moveZ)
        {
            velz = Mathf.Lerp(velz, 0, moveStopSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        StopMovement();

        rb.velocity = transform.forward * velz + transform.up * vely + transform.right * velx;
    }

    void PlayerInput(InputAction.CallbackContext _value)
    {

        //Y axis input
        //float moveYAxis = inputActionManager.inputActions.VTOL.MoveY.ReadValue<float>();

      
        //if(moveYAxis > 0)
        //{
        //    yMove = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (vely < 0)
        //    {
        //        vely += yAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
        //    }
        //    else
        //    {
        //        vely += yAcceleration * Time.deltaTime;
        //    }
        //}

        //if(moveYAxis < 0)
        //{
        //    yMove = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (vely > 0)
        //    {
        //        vely -= yAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
        //    }
        //    else
        //    {
        //        vely -= yAcceleration * Time.deltaTime;
        //    }
        //}

        //if(moveYAxis == 0)
        //{
        //    yMove = false;
        //}


        //X axis input
        //float moveXAxis = inputActionManager.inputActions.VTOL.MoveX.ReadValue<float>();

        //if (moveXAxis < 0)
        //{
        //    moveX = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (velx > 0)
        //    {
        //        velx -= moveAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
        //    }
        //    else
        //    {
        //        velx -= moveAcceleration * Time.deltaTime;
        //    }
        //}

        //if(moveXAxis > 0)
        //{
        //    moveX = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (velx < 0)
        //    {
        //        velx += moveAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
        //    }
        //    else
        //    {
        //        velx += moveAcceleration * Time.deltaTime;
        //    }
        //}

        //if (moveXAxis == 0)
        //{
        //    moveX = false;
        //}

        //Z axis input
        //float moveZAxis = inputActionManager.inputActions.VTOL.MoveZ.ReadValue<float>();

        //if (moveZAxis < 0)
        //{
        //    moveZ = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (velz > 0)
        //    {
        //        velz -= moveAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
                
        //    }
        //    else
        //    {
        //        velz -= moveAcceleration * Time.deltaTime;
        //    }
        //}


        //if (moveZAxis > 0)
        //{
        //    moveZ = true;
        //    //reduce speed quicker if opposite direction is being pressed
        //    if (velz < 0)
        //    {
        //        velz += moveAcceleration * oppositeDirectionMultiplier * Time.deltaTime;
        //    }
        //    else
        //    {
        //        velz += moveAcceleration * Time.deltaTime;
        //    }
        //}

        //if (moveZAxis == 0)
        //{
        //    moveZ = false;
        //}

        //non-mouse rotation input
        /*
        float axisZ = inputActionManager.inputActions.VTOL.MoveZ.ReadValue<float>();
        float axisX = inputActionManager.inputActions.VTOL.MoveX.ReadValue<float>();


        if (axisX == 0)
        {
            rotz = Mathf.Lerp(rotz, 0, rotationAcceleration * Time.deltaTime);
        }

        if (axisZ == 0)
        {
            rotx = Mathf.Lerp(rotx, 0, rotationAcceleration * Time.deltaTime);
        }

        rotz += axisZ * rotationAcceleration * 10 * Time.deltaTime;
        rotx += axisX * rotationAcceleration * 10 * Time.deltaTime;
        */

    }
}

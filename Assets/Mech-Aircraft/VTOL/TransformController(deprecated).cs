using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransformController : NetworkBehaviour
{
    public VariableFighter jetScript;
    public VTOL vtolScript;
    public MechControllerScript mechScript;
    public CS_UseAbilities abilites;
    public CS_MouseLook mechMouseLook;
    public GameObject recoilElement;

    public Transform groundCheckObj;
    public LayerMask groundLayer;
    bool goingVtol = false;
    float vtolTime = 1.0f;

    public int mode;
    public GameObject mechObject;
    public GameObject vtolObject;
    public GameObject jetObject;
    public bool isDisabled;
    public bool isEnabled;
    Vector3 startRot;
    float timer;

    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;

    public InputActionManager inputActionManager;

    public bool set = false;

    // Start is called before the first frame update
    void Start()
    { 
        if(!IsLocalPlayer)
        {
            enabled = false;
        }
        //inputActionManager.inputActions.Mech.Transform.performed += VTOLTransform;
        //inputActionManager.inputActions.Jet.Transform.performed += VTOLTransform;
        inputActionManager.inputActions.VTOL.Transform.performed += VTOLTransform;

        firstPersonCamera.SetActive(true);
        thirdPersonCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer)
        {
            return;
        }
        if (isDisabled)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Euler(startRot);
            vtolObject.transform.rotation = Quaternion.Lerp(vtolObject.transform.rotation, Quaternion.Euler(0, vtolObject.transform.eulerAngles.y, 0), timer / 10);
            if (timer / 10 > 0.5f / 10)
            {
                GetComponent<VariableFighter>().rollInput = 0.0f;
                GetComponent<VariableFighter>().pitchInput = 0.0f;
                isDisabled = false;
                timer = 0;
            }
        }


        //if (isEnabled)
        //{
        //    timer += Time.deltaTime;
        //    vtolObject.transform.rotation = Quaternion.Lerp(vtolObject.transform.rotation, Quaternion.Euler(0, vtolObject.transform.eulerAngles.y, 0), timer / 100);
        //    if (timer / 100 > 1f / 100)
        //    {
        //        isEnabled = false;
        //        timer = 0;
        //    }
        //}


        if (GroundCheck())
        {
            vtolTime = 1.0f;

            if (mode == 1)
            {
                set = false;
                mode = 0;
            }
        }
        else if (!GroundCheck())
        {
            if (inputActionManager.inputActions.Mech.Jump.IsPressed())
            {
                if (mode == 0)
                {
                    vtolTime -= Time.deltaTime;
                    if (vtolTime <= 0)
                    {
                        mode = 1;
                        vtolTime = 1.0f;
                    }
                }
            }
        }

        switch (mode)
        {
            // mech
            case 0:
                if (!set)
                {
                    inputActionManager.EnableInputActionMap(inputActionManager.inputActions.Mech);
                    mechObject.SetActive(true);
                    jetObject.SetActive(false);
                    vtolScript.enabled = false;
                    jetScript.enabled = false;
                    mechScript.enabled = true;
                    abilites.enabled = true;
                    recoilElement.SetActive(true);
                    mechMouseLook.enabled = true;

                    firstPersonCamera.SetActive(true);
                    thirdPersonCamera.SetActive(false);
                    set = true;
                }
                break;
            // vtol
            case 1:

                inputActionManager.EnableInputActionMap(inputActionManager.inputActions.VTOL);
                mechObject.SetActive(false);
                jetObject.SetActive(true);
                vtolScript.enabled = true;
                jetScript.enabled = false;
                mechScript.enabled = false;
                abilites.enabled = false;
                recoilElement.SetActive(false);
                mechMouseLook.enabled = true;

                firstPersonCamera.SetActive(false);
                thirdPersonCamera.SetActive(true);


                break;
            // jet
            case 2:

                inputActionManager.EnableInputActionMap(inputActionManager.inputActions.Jet);
                mechObject.SetActive(false);
                jetObject.SetActive(true);
                vtolScript.enabled = false;
                jetScript.enabled = true;
                mechScript.enabled = false;
                abilites.enabled = false;
                recoilElement.SetActive(false);
                mechMouseLook.enabled = false;
                firstPersonCamera.SetActive(false);
                thirdPersonCamera.SetActive(true);

                /* NOT NEEDED BUT MAYBE
                * abilities disable - Player
                * disable RecoilElement
                */
                break;
        }

    }

    public void VTOLEnabled()
    {
        //isEnabled = true;
    }

    public void VTOLDisabled()
    {
        startRot = transform.eulerAngles;
        isDisabled = true;
    }

    public bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheckObj.position, 5, groundLayer);
    }

    public void VTOLTransform(InputAction.CallbackContext _value)
    {
        if (mode == 2)
        {
            set = false;
            mode = 1;
        }
        else if (mode == 1)
        {
            set = false;
            mode = 2;
        }
    }
}

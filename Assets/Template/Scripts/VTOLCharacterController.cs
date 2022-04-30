using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using Networking;

public class VTOLCharacterController : NetworkBehaviour
{
    public static readonly List<VTOLCharacterController> List = new List<VTOLCharacterController>();
    NetworkTransformComponent networkManager;
    private bool isZoomed = false;
    public bool isJet = true;
    public Rigidbody rb = default;
    public float deadzoneRadius = 0.15f;
    public LayerMask groundMask;
    private enum JetState { IDLE, MAX, SLOW };
    JetState state = JetState.IDLE;
    JetState newState;

    int invertedControls = 1;

    [Header("Jet Settings")]
    /*public float jetSpeed = 50.0f;
    public float jetIdleSpeed = 25.0f;*/
    public AnimationCurve jetMaxSpeedCurve;
    public float jetMinSpeedMultiplier = 0.25F;
    public float jetAcceleration = 3.0f;
    public AnimationCurve jetElevationCurve;

    public float rollSpeed = 2.0f;
    public float pitchSpeed = 2.0f;
    public float yawSpeed = 10.0f;
    public float altitudRef;


    [HideInInspector] public Vector3 inputVector = default;
    [HideInInspector] public Vector3 inputRotation = default;

    private Camera cam;

    [Header("Camera Settings")]
    public float zoomTime;
    public float maxFov;
    public float minFov;
    public float defaultFov;

    [Header("UI Objects")]
    public Image vectorUI;
    public Text speedText;
    public Text altitudeText;
    [Header("Engine Settings")]
    public ParticleSystem[] engines;
    public float engineMaxForceLifeTime;
    public float engineMaxSpeed;

    public float engineDefaultForceLifeTime;
    public float engineDefaultSpeed;

    public float engineMinForceLifeTime;
    public float engineMinSpeed;


    private float zoomTimeElapsed = 0;
    private float zoomOutTime = 0;

    private float camIdleTime = 0;
    private float camSlowTime = 0;

    private float transformTime = 0;
    private float jetVelocityLerpAmount = 0.0F;

    private FMOD.Studio.EventInstance jetEngineAudio;

    [HideInInspector] public CinemachineVirtualCamera jetCam;

    [Header("Debug Bools")] private bool _showGizmo;

    [SerializeField] private AppDataSO appData = default;

    public bool collided = false;

    public void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogWarning($"{gameObject.name} does not have a rigidbody", this);
        }
        networkManager = this.gameObject.GetComponent<NetworkTransformComponent>();

    }

    private void Start()
    {
        jetEngineAudio = FMODUnity.RuntimeManager.CreateInstance(AudioManager.Instance.events.playerAudioEvents.airMovement);
        AudioManager.Instance.UpdatePosition(jetEngineAudio, transform.position);
    }

    private void OnEnable()
    {
        CursorManager.ConfineCursor("vtol-controller", false, true);
        jetSetup();
        transformTime = Time.time;

        if (AudioManager.Instance.GetPlaybackState(jetEngineAudio) != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            jetEngineAudio.setParameterByName("END", 0.0f);
            jetEngineAudio.start();
        }
    }

    private void OnDisable()
    {
        CursorManager.ConfineCursor("vtol-controller", false, false);

        if (AudioManager.Instance.GetPlaybackState(jetEngineAudio) == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            jetEngineAudio.setParameterByName("END", 1.0f);
        }

        if (AudioManager.Instance.GetPlaybackState(jetEngineAudio) != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            jetEngineAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public override void OnNetworkSpawn()
    {
        List.Add(this);
        if (!IsOwner) return;
        //InputManager.VTOL.Transform.performed += OnTransform;
        InputManager.VTOL.MouseLook.performed += MouseAirRotation;
        InputManager.VTOL.GamepadLook.performed += GamepadAirRotation;
        InputManager.VTOL.Move.performed += AirInput;
        //InputManager.VTOL.AirTransform.performed += AirTransform;
        InputManager.VTOL.Pause.performed += OnPauseGame;

        // Set references
        //SetReferencesServerRpc();

        // ^ Above is the solution to this.
        //FindObjectOfType<IconsTransition>().vtol = this;
        //FindObjectOfType<IconsTransition>().jetShoot = GetComponent<JetShoot>();
    }

    //[ServerRpc]
    //private void SetReferencesServerRpc()
    //{
    //    SetReferencesClientRpc();
    //}

    //[ClientRpc]
    //private void SetReferencesClientRpc()
    //{
    //    if (!IsOwner) return;

    //    var classes = FindObjectsOfType</* Insert your class here*/>();
    //    foreach(var class in classes)
    //    {
    //        class.SetReference(this);
    //    }
    //}

    public override void OnNetworkDespawn()
    {
        List.Remove(this);

        if (!IsOwner) return;
        //InputManager.VTOL.Transform.performed -= OnTransform;
        InputManager.VTOL.MouseLook.performed -= MouseAirRotation;
        InputManager.VTOL.GamepadLook.performed -= GamepadAirRotation;
        InputManager.VTOL.Move.performed -= AirInput;
        //InputManager.VTOL.AirTransform.performed -= AirTransform;
        InputManager.VTOL.Pause.performed -= OnPauseGame;
    }

    // Depreciated method of transforming
    /*private void OnTransform(InputAction.CallbackContext obj)
    {
        if (!isJet)
        {
            TransformManager.ToggleMechMode(OwnerClientId);
        }
    }*/


    private IEnumerator LerpFov()
    {
        return null;
    }



    //private void AirTransform(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        isJet = !isJet;

    //        if (isJet)
    //        {
    //            jetSetup();
    //        }
    //        else
    //        {
    //            VTOLSetup();
    //        }
    //    }

    //}

    private void AirInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector3>();
    }

    private void MouseAirRotation(InputAction.CallbackContext context)
    {
        inputRotation = context.ReadValue<Vector2>();
    }

    private void GamepadAirRotation(InputAction.CallbackContext context)
    {
        inputRotation = (context.ReadValue<Vector2>() * 0.5F + new Vector2(0.5f, 0.5f)) * new Vector2(Screen.width, Screen.height);
    }

    private void jetSetup()
    {
        
        jetCam = VCameraSetup.vtolCam?.GetVirtualCamera();
        if (jetCam != null)
        {
            jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = defaultFov;
        }
 
        //rb.useGravity = true;
    }
 
    private void OnPauseGame(InputAction.CallbackContext context)
    {
        GameObject.Find("PauseManager").GetComponent<PauseManager>().ChangePauseState();
    }
 

    void ResetEngines()
    {
        foreach (var engine in engines)
        {
            engine.Clear();
        }
    }

    void JetEngines()
    {
        
        if(state != newState)
        {
            ResetEngines();
            newState = state;
        }



        if(state == JetState.IDLE)
        {
            foreach (var eng in engines)
            {
                var particleMain = eng.main;
                particleMain.startLifetime = engineDefaultForceLifeTime;
                particleMain.startSpeed = engineDefaultSpeed;
            }
        }
        else if(state == JetState.MAX)
        {
            foreach (var eng in engines)
            {
                var particleMain = eng.main;
                particleMain.startLifetime = engineMaxForceLifeTime;
                particleMain.startSpeed = engineMaxSpeed;
            }
        }
        else if(state == JetState.SLOW)
        {
            foreach (var eng in engines)
            {
                var particleMain = eng.main;
                particleMain.startLifetime = engineMinForceLifeTime;
                particleMain.startSpeed = engineMinSpeed;
            }
        }
    }

    void JetCamera()
    {
        if (!InputManager.VTOL.Zoom.IsPressed())
        {
            if (state == JetState.MAX)
            {
                camSlowTime = 0;
                camIdleTime = 0;
                float mag = rb.velocity.magnitude;
                float fov = (1) / (1 + Mathf.Pow(2.71828f, -zoomTime * (mag - 60)));
                float camFov = jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
                camFov = Mathf.Clamp(camFov, minFov, maxFov);
                //jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = camFov + (fov * 2);
            }
            else if (state == JetState.IDLE)
            {
                camSlowTime = 0;
                if (camIdleTime < zoomTime)
                {
                    if(jetCam != null)
                    {
                        //float fov = Mathf.Lerp(jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, defaultFov, camIdleTime / zoomTime);
                        //jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                        camIdleTime += Time.deltaTime;
                    }

                }
            }
            else if (state == JetState.SLOW)
            {
                camIdleTime = 0;
                if (camSlowTime < zoomTime)
                {
                    //float fov = Mathf.Lerp(jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, defaultFov - 20, camSlowTime / zoomTime);
                    //jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                    camSlowTime += Time.deltaTime;
                }
            }

        }


    }

    void JetZoom()
    {
        float fov = Mathf.Lerp(defaultFov, maxFov, jetVelocityLerpAmount);
        zoomTimeElapsed = Mathf.Clamp(zoomTimeElapsed + Time.deltaTime * (InputManager.VTOL.Zoom.IsPressed() ? 1.0F : -1.0F), 0.0F, zoomTime);
        fov = Mathf.Lerp(fov, minFov, zoomTimeElapsed / zoomTime);

        if (!jetCam) return;   // this is a temporary solution to stop the null refrence 

        jetCam.m_Lens.FieldOfView = fov;
        
        /*if (InputManager.VTOL.Zoom.IsPressed())
        {
            float fov = Mathf.MoveTowards(jetCam.m_Lens.FieldOfView, minFov, zoomTime * Time.deltaTime);
            jetCam.
            zoomOutTime = 0;
            if (zoomTimeElapsed < zoomTime)
            {
                float fov = Mathf.Lerp(jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, minFov, zoomTimeElapsed / zoomTime);
                jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                zoomTimeElapsed += Time.deltaTime * 5;
            }
        }
        else
        {
            zoomTimeElapsed = 0;
            if(jetCam != null)
            {
                if (jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView < 90)
                {
                    if (zoomOutTime < zoomTime)
                    {
                        float fov = Mathf.Lerp(minFov, defaultFov, zoomOutTime / zoomTime);
                        jetCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                        zoomOutTime += Time.deltaTime * 5;
                    }
                }
            }

        }*/
    }

    private void Update()
    {
        if (!IsOwner) return;
        JetZoom();
        JetCamera();
        JetEngines();

        AudioManager.Instance.UpdatePosition(jetEngineAudio, transform.position);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            CursorManager.ConfineCursor("vtol-controller", false, true);
        }

        /*        Debug.Log(appData.invertedFlightControls);
                //VTOL Rotation
                if (!isJet)
                {
                    var target = (transform.right * inputVector.x + transform.up * inputVector.y + transform.forward * inputVector.z) * vtolSpeed;
                    rb.velocity = Vector3.Lerp(rb.velocity, target, vtolAcceleration);

                    float yawInput = (inputRotation.x - (Screen.width / 2)) / (Screen.width / 2);

                    yawInput = Mathf.Clamp(yawInput, -1, 1);

                    if (Mathf.Abs(yawInput) < deadzoneRadius)
                    {
                        yawInput = 0;
                    }

                    rb.AddTorque(gameObject.transform.up * (yawInput * yawSpeed * vtolYawMultiplier * Time.deltaTime));

                    // VTOL Balancing
                    var rot = transform.rotation;
                    rot.w = 0;
                    transform.Rotate(MoveToZero(rot.x),0,MoveToZero(rot.z));
                    //Debug.Log("Current Rot: " + rot.eulerAngles);
                    //BUG: If the user keeps holding rotate to either side for ages the VTOL will flip
                    //BUG: 360 degrees = 0 degrees, and if it goes to 359 it'll lerp to 0 aggressively
                }

                else *///Jet
        {
            invertedControls = appData.invertedFlightControls ?  1 : -1;

            //Thrust Input
            float timeSinceTransform = Time.time - transformTime;
            float targetVelocity = jetMaxSpeedCurve.Evaluate(timeSinceTransform);
            jetVelocityLerpAmount = Mathf.MoveTowards(jetVelocityLerpAmount, inputVector.z / 2.0F + 0.5F,jetAcceleration * Time.deltaTime);
            targetVelocity *= Mathf.Lerp(jetMinSpeedMultiplier, 1.0F, jetVelocityLerpAmount);
            Vector3 targetVec = transform.forward * targetVelocity;
            float elevationAmount = jetElevationCurve.Evaluate(timeSinceTransform);
            targetVec.y += elevationAmount * 2.0F;
            rb.transform.Rotate(-elevationAmount * Time.deltaTime,0.0F,0.0F, Space.Self);
            rb.velocity = targetVec;

            state = inputVector.z > 0.5F ? JetState.MAX : inputVector.z > -0.5F ? JetState.IDLE : JetState.SLOW;

            //Rotation Input
            float pitchInput = (inputRotation.y - (Screen.height / 2)) / (Screen.height / 2);

            pitchInput = Mathf.Clamp(pitchInput, -1, 1);

            if (Mathf.Abs(pitchInput) < deadzoneRadius)
            {
                pitchInput = 0;
            }

            float yawInput = (inputRotation.x - (Screen.width / 2)) / (Screen.width / 2);

            yawInput = Mathf.Clamp(yawInput, -1, 1);

            if (Mathf.Abs(yawInput) < deadzoneRadius)
            {
                yawInput = 0;
            }

            float rollInput = inputVector.x;

            //Add rotation
            rb.AddTorque(gameObject.transform.forward * rollInput * -rollSpeed * Time.deltaTime);
            rb.AddTorque(gameObject.transform.right * pitchInput * pitchSpeed * invertedControls * Time.deltaTime);
            rb.AddTorque(gameObject.transform.up * yawInput * yawSpeed * Time.deltaTime);

            float altitude = 0;
            RaycastHit altCheck;
            if (Physics.Raycast(transform.position, Vector3.down, out altCheck, 100000, groundMask))
            {
                altitude = transform.position.y - altCheck.point.y;
                altitudRef = altitude;
            }


            cam = Camera.main;
            var camTransform = cam.transform;
            Transform centrepoint = camTransform;
            centrepoint.RotateAround(camTransform.position, Vector3.forward + new Vector3(0, 0, 5), 0);

            Vector3 resultV = camTransform.forward * 5;
            vectorUI.transform.rotation = Quaternion.Euler(resultV);

            if(Physics.Raycast(transform.position, transform.forward, 2, groundMask))
            {
                collided = true;
                networkManager.SwitchOnCollision();
            }
            
        }
    }
 
    //private static float MoveToZero(float current)
    //{
    //    var newMoveAngle = Mathf.MoveTowardsAngle(current, 0, Time.deltaTime * .5F);
    //    return newMoveAngle;
    //}

#if UNITY_EDITOR
    //Added UI to show centre of screen but I can't place it in the right place because Im dumb
    private void OnDrawGizmos()
    {
 
    }
#endif
}
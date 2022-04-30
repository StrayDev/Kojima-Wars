using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControlScript : MonoBehaviour
{
    [SerializeField] private GameObject FpCam;
    [SerializeField] private GameObject TpCam;

    private AudioListener FpCamAudioLis;
    private AudioListener TpCamAudioLis;

    public InputActionManager inputActionManager;

    private void Awake()
    {

    }

    void Start()
    {

        //Get Camera Listeners
        FpCamAudioLis = FpCam.GetComponent<AudioListener>();
        TpCamAudioLis = TpCam.GetComponent<AudioListener>();

        //Camera Position Set
        CameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));
    }

    private void OnEnable()
    {
        inputActionManager.inputActions.Mech.SwitchCamera.performed += SwitchCamera;
        inputActionManager.inputActions.Mech.SwitchCamera.Enable();
    }

    void Update()
    {

    }

    //UI JoyStick Method
    public void CameraPositonM()
    {
        CameraChangeCounter();
    }

    //Change Camera Keyboard
    void SwitchCamera(InputAction.CallbackContext obj)
    {
        CameraChangeCounter();
    }

    //Camera Counter
    void CameraChangeCounter()
    {
        int cameraPositionCounter = PlayerPrefs.GetInt("CameraPosition");
        cameraPositionCounter++;
        CameraPositionChange(cameraPositionCounter);
    }

    //Cam Swap
    void CameraPositionChange(int camPosition)
    {
        if (camPosition > 1)
        {
            camPosition = 0;
        }

        //Cam pos info
        PlayerPrefs.SetInt("CameraPosition", camPosition);

        //FpCam pos
        if (camPosition == 0)
        {
            FpCam.SetActive(true);
            FpCamAudioLis.enabled = true;

            TpCamAudioLis.enabled = false;
            TpCam.SetActive(false);
        }

        //TpCam pos
        if (camPosition == 1)
        {
            TpCam.SetActive(true);
            TpCamAudioLis.enabled = true;

            FpCamAudioLis.enabled = false;
            FpCam.SetActive(false);
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_Dash : NetworkBehaviour
{
   
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private int dashCount;
    
    private Rigidbody _rb;
    private GameObject _player;
    private MechCharacterController _playerMovement;
    
    private GameObject _dashCount;
    private Vector3 _velocityNorm;
    
    private Camera _FPSCamera;
    private Transform _cameraTransform;
    
    [HideInInspector] public CS_Firepoints firepoint;
    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        
        _player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        firepoint = _player.GetComponent<CS_Firepoints>();
        _playerMovement = _player.GetComponent<MechCharacterController>();
        _rb = _player.GetComponent<Rigidbody>();
        _velocityNorm = _rb.velocity;
        _cameraTransform = _player.GetComponentInChildren<MechLookPitch>().transform;

        AudioManager.Instance.CreateFMODInstancePoolServerRpc(AudioManager.Instance.events.playerAudioEvents.dash, 4);
        AudioManager.Instance.PlayFMODOneShotServerRpc(AudioManager.Instance.events.playerAudioEvents.dash, _playerMovement._currentPos);

        //  transform.position = firepoint.dashPoint.position;
        StartCoroutine(ActiveTime());
        //  transform.position = firepoint.dashPoint.position;
        //StartCoroutine(ActiveTime());
        ServerAbilityManager.Instance.ApplyDashServerRPC(_player.GetComponent<NetworkObject>(),
            dashForce, dashDuration);
    }

    private IEnumerator ActiveTime()
    {
        //If moving apply to movement direction
        if (_rb.velocity != Vector3.zero)
        {
            _velocityNorm = _rb.velocity; 
            _rb.velocity = _playerMovement.movePosition() * dashForce;
        }
        else
        {
            //If stationary - move based on camera
            _rb.velocity = _cameraTransform.transform.forward * dashForce;
            //Todo add limiting parameters to stop vertical dashes 
        }
        
        yield return new WaitForSeconds(dashDuration);
        _rb.velocity = _velocityNorm;
        //Destroy(gameObject);
       
    }

    private void Update()
    {
        AudioManager.Instance.UpdatePosition(AudioManager.Instance.events.playerAudioEvents.dash, _playerMovement._currentPos);
    }

    private void GetFPSCamRecursively(Transform obj)
    {
        //Get HUD recursively in children
        foreach (Transform child in obj)
        {
            if (child.TryGetComponent(out Camera camera))
            {
                _FPSCamera = camera;
            }
            else
            {
                GetFPSCamRecursively(child);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VectorSwizzles;

public class MechCharacterController : NetworkBehaviour
{
    public static event Action<MechCharacterController> OnLocalMechDead;

    [SerializeField] private LayerMask groundLayerMask = default;

    private CapsuleCollider _collider = default;
    private Rigidbody _rb = default;

    private Vector3 _inputVector = default;

    public static readonly List<MechCharacterController> List = new List<MechCharacterController>();

    public Recoil recoilScript;
    public WeaponScript weapon;
    public CS_PlayerStats stats;

    [SerializeField] GameObject[] playerBodyParts;

    //For Movement/Dash Abilities 
    public Vector3 _previousPos;
    public Vector3 _currentPos;
    public Vector3 moveDirection_;

    [SerializeField] private GameTeamData m_teamData = default;
    [SerializeField] private SkinnedMeshRenderer[] m_modelRenderers;

    private bool grounded = false;

    private void Awake()
    {
        if(!TryGetComponent<CapsuleCollider>(out _collider))
        {
            Debug.LogWarning($"{gameObject.name} does not have a capsule collider", this);
        }

        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.LogWarning($"{gameObject.name} does not have a rigidbody", this);
        }

        recoilScript = GetComponentInChildren<Recoil>();
        weapon = GetComponentInChildren<WeaponScript>();
        stats = GetComponent<CS_PlayerStats>();
        _currentPos = transform.position;
    }

    public void UpdatePlayerMaterial(string teamName)
    {
        Material material = m_teamData.GetTeamData(teamName).Material;
        foreach (SkinnedMeshRenderer part in m_modelRenderers)
        {
            part.material = material;
        }
    }

    public override void OnNetworkSpawn()
    {
        List.Add(this);
        
        if (!IsOwner)
        {
            GetComponent<FMODUnity.StudioListener>().enabled = false;
            return;
        }
        InputManager.MECH.Jump.performed += OnJump;
        InputManager.MECH.Movement.performed += OnMove;
        InputManager.MECH.Movement.canceled += OnMove;
        InputManager.MECH.Reload.performed += OnReload;
        InputManager.MECH.Pause.performed += OnPauseGame;

        foreach (var bodyPart in playerBodyParts)
        {
            bodyPart.layer = LayerMask.NameToLayer("Player");
        }
        _rb.useGravity = false;
    }

    public override void OnNetworkDespawn()
    {
        List.Remove(this);

        if (!IsOwner) return;
        InputManager.MECH.Jump.performed -= OnJump;
        InputManager.MECH.Movement.performed -= OnMove;
        InputManager.MECH.Movement.canceled -= OnMove;
        InputManager.MECH.Reload.performed -= OnReload;
        InputManager.MECH.Pause.performed -= OnPauseGame;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // checked if grounded 
        if (grounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, stats.jumpForce, _rb.velocity.z);
            //GetComponent<PlayerAudioManager>().PlayJump();
        }
    }

    private void GroundCheck()
    {
        var pos = _collider.bounds.center;
        var radius = _collider.radius;
        var maxDis = _collider.height / 2 - _collider.radius + 0.1f;
        var dir = Vector3.down;

        #if UNITY_EDITOR
        Debug.DrawLine(pos, pos + dir * maxDis, grounded ? Color.green : Color.red);
        #endif

        grounded = Physics.SphereCast(pos, radius, dir, out var hit, maxDis, groundLayerMask) &&
                   Vector3.Angle(hit.normal, Vector3.up) < stats.maxRampAngle;
        // Prevent sliding
        Vector3 v = _rb.velocity.xoz();
        float surfaceAngleMultiplier = Mathf.Clamp(Vector3.Dot(v.normalized, hit.normal) + 1.0F + Mathf.Sin(stats.maxRampAngle * Mathf.Deg2Rad), 0.0F, 1.0F);
        v *= surfaceAngleMultiplier;
        _rb.velocity = new Vector3(v.x,_rb.velocity.y,v.z);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }
    
    private void OnReload(InputAction.CallbackContext context)
    {
        weapon.Reload();
    }

    private void OnPauseGame(InputAction.CallbackContext context)
    {
        GameObject.Find("PauseManager").GetComponent<PauseManager>().ChangePauseState();
    }

    private void Update()
    {
        if (!IsOwner) return;

        GroundCheck();
        
        var target = (transform.forward * _inputVector.y + transform.right * _inputVector.x) * stats.speed;

        var current = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        var acceleration = Mathf.Lerp(stats.deceleration, stats.acceleration, Vector3.Dot(current, target));
        acceleration *= grounded ? 1.0F : stats.airAccelerationMultiplier;
        Vector3 newVelocity = Vector3.MoveTowards(current, target, acceleration * stats.speed * Time.deltaTime);
        newVelocity += new Vector3(0f, _rb.velocity.y + stats.gravity * Time.deltaTime, 0f);
        _rb.velocity = newVelocity;
        
        Step();
    }

    private void Step()
    {
        var rayPos = _collider.bounds.center + Vector3.down * (_collider.bounds.extents.y - 0.1f);
        var rayDir = _rb.velocity.xoz().normalized;
        var rayDist = _collider.radius + 0.25f;

        for (int i = -45; i <= 45; i += 45)
        {
            var thisDir = Quaternion.Euler(0, i, 0) * rayDir;
            #if UNITY_EDITOR
            Debug.DrawLine(rayPos, rayPos + rayDir * rayDist);
            Debug.DrawLine(rayPos + Vector3.up * stats.maxStepSize, rayPos + Vector3.up * stats.maxStepSize + rayDir * rayDist);
            #endif
            if (Physics.Raycast(rayPos, thisDir, out var hit, rayDist, groundLayerMask) && 
                Vector3.Dot(thisDir,-hit.normal) > 0.75F &&
                !Physics.Raycast(rayPos += Vector3.up * stats.maxStepSize, rayDir, rayDist, groundLayerMask) &&
                Physics.Raycast(rayPos + rayDir * (hit.distance + 0.025f), Vector3.down, out hit, stats.maxStepSize, groundLayerMask))
            {
                float stepHeight = stats.maxStepSize - hit.distance;
                if (stats.stepSmooth > 0.001F) stepHeight *= Time.deltaTime * 1.0F / stats.stepSmooth;
                _rb.position += Vector3.up * stepHeight;
                grounded = true;
                break;
            }
        }
    }

    //Dash Ability
    private void FixedUpdate()
    {
        _previousPos = _currentPos; 
        _currentPos = transform.position;
    }

    public  Vector3 movePosition()
    {
        moveDirection_ = (_currentPos - _previousPos).normalized;
        
        return moveDirection_;
    }

    public void PositionMech(Vector3 position, Quaternion rotation) => PositionServerRpc(position, rotation);
   
    [ServerRpc(RequireOwnership =false)]
    private void PositionServerRpc(Vector3 position, Quaternion rotation) => PositionClientRpc(position, rotation);
    
    [ClientRpc]
    private void PositionClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void KillMech() => HandleMechDeadServerRpc();

    [ServerRpc(RequireOwnership =false)]
    private void HandleMechDeadServerRpc() => HandleMechDeadClientRpc();

    [ClientRpc]
    private void HandleMechDeadClientRpc()
    {
        Debug.Log("MechCharacterController : OnDeath", this);

        // why is it not just this
        //gameObject.SetActive(false);

        // why is it like this?? ^^^
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

        if (OwnerClientId == NetworkManager.LocalClient.ClientId)
        {
            OnLocalMechDead?.Invoke(this);
        }
    }

}
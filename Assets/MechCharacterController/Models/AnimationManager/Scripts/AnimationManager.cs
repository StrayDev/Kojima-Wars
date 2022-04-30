using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class AnimationManager : NetworkBehaviour
{
    [SerializeField] Rigidbody _rb = default;
    [SerializeField] Transform _transform = default;

    private Animator animator;
    private MechCharacterController mechController;
    [SerializeField] Rig aimLayer;

   // [SerializeField] private PlayerClassSO playerClassSO = default;
    [SerializeField] private WeaponScript weaponScript;
    // private readonly int horizontal = Animator.StringToHash("Horizontal");
    //  private readonly int vertical = Animator.StringToHash("Vertical");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        mechController = GetComponent<MechCharacterController>();

    }

    private void Update()
    {
        UpdateClientVisuals();
    }

    public void UpdateClientVisuals()
    {
        if (!IsOwner) return;

        var move = InputManager.MECH.Movement.ReadValue<Vector2>();
        SetAnimatorValues(move);
        UpdateAnimatorValuesServerRpc(move);
        AimAnimation();
    }

    [ServerRpc]
    public void UpdateAnimatorValuesServerRpc(Vector2 move)
    {
        UpdateAnimatorValuesClientRpc(move);
        //animator.SetBool("Grounded", mechController.grounded);
    }

    [ClientRpc]
    public void UpdateAnimatorValuesClientRpc(Vector2 move)
    {
        if (IsOwner) return;
        SetAnimatorValues(move);
    }

    private void SetAnimatorValues(Vector3 move)
    { 
        animator.SetFloat("Horizontal", move.x);
        animator.SetFloat("Vertical", move.y);
        //animator.SetBool("Grounded", mechController.grounded);
    }

    public void AimAnimation()
    {
        if (InputManager.MECH.ADS.IsPressed())
        {
            aimLayer.weight += Time.deltaTime / weaponScript.ADSAmount;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / weaponScript.ADSAmount;
        }

     }
    
}

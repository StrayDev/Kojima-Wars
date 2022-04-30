using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseDefenceLocation : MonoBehaviour
{
    public static Action<BaseDefenceLocation> OnInteracted;
    public static Action<Entity, BaseDefenceLocation> OnLocalPlayerEnteredRange;
    public static Action<Entity, BaseDefenceLocation> OnLocalPlayerExitedRange;

    public event Action<EBaseDefenceState> OnChangeState;
    public event Action OnDefenceDestroyed;

    public BaseController BaseController => m_baseController;

    public EBaseDefenceState CurrentState { get; private set; } = EBaseDefenceState.EMPTY;
    
    public int RepairCost => m_repairCost * (int)(DefenceObject.Health.MaxHealth - DefenceObject.Health.CurrentHealth);

    public BaseDefence DefenceObject { get; private set; }

    public PlayerInformation PlayerInRange => m_playerResources;

    [SerializeField] private BaseController m_baseController = default;

    [Tooltip("The input action that should trigger an interact with the base defence.")]
    [SerializeField] private InputActionReference m_interactAction = default;

    [Tooltip("The amount of currency needed to repair one health point.")]
    [SerializeField] private int m_repairCost = 1;

    [Tooltip("Possible Defences Types for this location. (empty means all are possible)")]
    [SerializeField] private List<EBaseDefenceTypes> m_possibleDefenceTypes = new List<EBaseDefenceTypes>();

    private Entity m_entityInRange = null;
    private PlayerInformation m_playerResources = null;

    public List<EBaseDefenceTypes> GetPossibleDefences()
    {
        return m_possibleDefenceTypes;
    }

    private bool m_playerInRange = false;

    public void RepairDefenceObject()
    {
        if (DefenceObject != null)
        {
            DefenceObject.Health.ResetHealth();
            ChangeState(EBaseDefenceState.WORKING);
        }
    }

    public void DestroyDefenceObject()
    {

        if (DefenceObject != null)
        {
            Destroy(DefenceObject.gameObject);
            DefenceObject.Health.OnDeath -= DestroyDefenceObject;
            DefenceObject.Health.OnDamaged -= OnDamaged;
            DefenceObject = null;
        }

        CurrentState = EBaseDefenceState.EMPTY;
        OnDefenceDestroyed?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && entity.EntityType == Entity.EEntityType.PLAYER)
        {
            NetworkObject networkObject = entity.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.OwnerClientId == networkObject.NetworkManager.LocalClientId)
            {
                m_playerInRange = true;
                m_entityInRange = entity;
                m_playerResources = entity.GetComponent<PlayerInformation>();
                OnLocalPlayerEnteredRange?.Invoke(m_entityInRange, this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && entity.EntityType == Entity.EEntityType.PLAYER)
        {
            NetworkObject networkObject = entity.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.OwnerClientId == networkObject.NetworkManager.LocalClientId)
            {
                m_playerInRange = false;
                m_entityInRange = null;
                OnLocalPlayerExitedRange?.Invoke(entity, this);
            }
        }
    }

    private bool IsEntityInRangeOfCurrentTeam()
    {
        return m_entityInRange != null && 
               m_entityInRange.EntityType == Entity.EEntityType.PLAYER && 
               m_entityInRange.TeamName == m_baseController.TeamOwner;
    }

    private void OnEnable()
    {
        m_interactAction.action.performed += OnInteractPressed;
        m_interactAction.action.Enable();
    }

    private void OnDisable()
    {
        m_interactAction.action.performed -= OnInteractPressed;
    }

    private void OnInteractPressed(InputAction.CallbackContext callbackContext)
    {
        if (m_playerInRange && IsEntityInRangeOfCurrentTeam())
        {
            OnInteracted?.Invoke(this);
        }
    }

    private void ChangeState(EBaseDefenceState newState)
    {
        CurrentState = newState;
        OnChangeState?.Invoke(CurrentState);
    }
    
    public void DefenceBuilt(BaseDefence defence)
    {
        ChangeState(EBaseDefenceState.WORKING);

        defence.SetBaseController(m_baseController);

        DefenceObject = defence;
        DefenceObject.Health.OnDeath += DestroyDefenceObject;
        DefenceObject.Health.OnDamaged += OnDamaged;
    }

    private void OnDamaged(float damage)
    {
        ChangeState(EBaseDefenceState.DAMAGED);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;

public class BaseUnitGeneration : NetworkBehaviour
{
    public static event Action<BaseUnitGeneration> OnInteracted;
    public static event Action<Entity, BaseUnitGeneration> OnLocalPlayerEnteredRange;
    public static event Action<Entity, BaseUnitGeneration> OnLocalPlayerExitedRange;

    public static event Action<AIUnitTypesData.UnitTypeInfo> OnQueueAdded;
    public static event Action<BaseUnitGeneration> OnQueueEnded;
    public static event Action<float> OnQueueProgressUpdate;

    public BaseController BaseController => m_baseController;

    private EBaseRank m_baseRank;
    [SerializeField] private BaseController m_baseController = default;
    [SerializeField] private GenerationRates generationRate = default;
    [SerializeField] public List<EUnitTypes> spawnableUnits = new List<EUnitTypes>();
    [SerializeField] private InputActionReference m_interactAction = default;
    [SerializeField] private int m_maxQueueAmount = default;
    [SerializeField] private AIUnitTypesData m_unitTypesData;
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private GameTeamData gameTeamData;
    [SerializeField] private AIUnitManager aiUnitManager;

    private List<AIUnitTypesData.UnitTypeInfo> m_unitQueue = new List<AIUnitTypesData.UnitTypeInfo>();
    private bool m_isQueueRunning;
    private bool m_playerInRange = false;
    private Entity m_entityInRange;
    private PlayerInformation m_playerResources;
    public PlayerInformation PlayerInRange => m_playerResources;

    private int maxWidth = 5, width, height;
    public Vector3 m_unitSpawnOffset;

    private float rankMultiplyer = 1;

    public GameObject player;
    public SelectGroup group;

    public ulong playerId;
    public TextMeshProUGUI idText;

    public List<GameObject> units = new List<GameObject>();
    public bool set = false;

    private Color teamColor;

    public DefenceLocationManager defenceLocationManager;
    public BaseController controller;
    public GameObject unitMover;

    private int layerMask = 1 << 9;

    public List<AIUnitTypesData.UnitTypeInfo> GetUnitQueue()
    {
        return m_unitQueue;
    }

    private void Start()
    {
        m_baseRank = m_baseController.Rank;
        rankMultiplyer = generationRate.GetTimeMultiplyerForRank(m_baseRank);
    }

    private void OnEnable()
    {
        m_interactAction.action.performed += OnInteractPressed;
        m_interactAction.action.Enable();
    }

    private void OnDisable()
    {
        m_interactAction.action.performed -= OnInteractPressed;
        m_interactAction.action.Disable();
    }

    private void OnInteractPressed(InputAction.CallbackContext callbackContext)
    {
        if (m_playerInRange && IsEntityInRangeOfCurrentTeam())
        {
            OnInteracted?.Invoke(this);
        }
    }

    private bool IsEntityInRangeOfCurrentTeam()
    {
        return m_entityInRange != null &&
               m_entityInRange.EntityType == Entity.EEntityType.PLAYER &&
               m_entityInRange.TeamName == m_baseController.TeamOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && entity.EntityType == Entity.EEntityType.PLAYER && entity.TeamName == m_baseController.TeamOwner)
        {
            NetworkObject networkObject = entity.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                //playerId = networkObject.NetworkObjectId;
                if (playerId == 0)
                {
                    //bool isRedTeam = playerData.GetPlayerTeam(networkObject.OwnerClientId);
                    //teamColor = gameTeamData.GetTeamData((isRedTeam ? gameTeamData.GetTeamDataAtIndex(0).TeamName : gameTeamData.GetTeamDataAtIndex(1).TeamName)).Colour;
                    teamColor = Color.white;
                    SetPlayerInMenuServerRpc(networkObject.NetworkObjectId);
                }
            }
            //&& networkObject.IsOwner
            if (networkObject != null && networkObject.OwnerClientId == networkObject.NetworkManager.LocalClient.ClientId)
            {
                BaseUnitOptionsObject.OnTrainUnit += AddUnitToSpawnQueue;
                BaseUnitOptionsObject.OnInstantSpawnUnit += InstantSpawnUnits;

                m_playerInRange = true;
                m_entityInRange = entity;
                m_playerResources = entity.GetComponent<PlayerInformation>();
                //player = other.gameObject.transform.parent.gameObject;
                OnLocalPlayerEnteredRange?.Invoke(entity, this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && entity.EntityType == Entity.EEntityType.PLAYER && m_entityInRange == entity)
        {
            NetworkObject networkObject = entity.GetComponent<NetworkObject>();
            
            if (networkObject != null)
            {
                //playerId = networkObject.NetworkObjectId;
                if (playerId != 0)
                {
                    SetPlayerInMenuServerRpc(0);
                }
            }
            
            if (networkObject != null && networkObject.IsOwner)
            {
                BaseUnitOptionsObject.OnTrainUnit -= AddUnitToSpawnQueue;
                BaseUnitOptionsObject.OnInstantSpawnUnit -= InstantSpawnUnits;

                m_playerInRange = false;
                m_entityInRange = null;
                OnLocalPlayerExitedRange?.Invoke(entity, this);
            }
        }
    }

    public void AddUnitToSpawnQueue(AIUnitTypesData.UnitTypeInfo unit)
    {
        int amount = 0;
        foreach (var units in group.units)
        {
            if (units != null)
            {
                amount++;
            }
            else
            {
                group.units.Remove(units);
            }
        }
        int spawnAmount = 5 - amount;
        Debug.Log(spawnAmount);

        if (m_unitQueue.Count < m_maxQueueAmount)
        {
            if (m_playerResources)
            {
                if (m_playerResources.Currency >= unit.unitCost)
                {
                    m_playerResources.DecreasePlayerCurrency(unit.unitCost);

                    m_unitQueue.Add(unit);

                    OnQueueAdded?.Invoke(unit);

                    if (!m_isQueueRunning)
                    {
                        StartCoroutine(StartSpawnQueue(spawnAmount));
                    }
                }
            }
            
        }
    }

    public bool AreUnitsInBase()
    {
        if(group.units.Count == 0)
        {
            return true;
        }

        foreach(var locations in defenceLocationManager.unitDefenceLocations)
        {
            foreach(var info in locations.defenceLocationInfo)
            {
                if (info.groupId == group.groupId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void InstantSpawnUnits(AIUnitTypesData.UnitTypeInfo unit, ulong localPlayerId)
    {
        if(AreUnitsInBase())
        {
            int amount = 0;
            foreach (var units in group.units)
            {
                if (units != null)
                {
                    amount++;
                }
                else
                {
                    group.units.Remove(units);
                }
            }

            int spawnAmount = 5 - amount;

            int instantSpawnCost = unit.unitCost;
            if (m_playerResources.Currency >= instantSpawnCost)
            {
                m_playerResources.DecreasePlayerCurrency(instantSpawnCost);
                Spawn(unit, spawnAmount, localPlayerId);
            }
        }
        else
        {
            Debug.Log("Other units in group not at defence base");
        }

    }

    private IEnumerator StartSpawnQueue(int amount)
    {
        m_isQueueRunning = true;

        while (m_unitQueue.Count > 0)
        {
            foreach (var units in m_unitQueue.ToArray())
            {
                float time = 0.0f;
                while (time < units.unitSpawnTime * rankMultiplyer)
                {
                    float progressAmount = time / Mathf.Max(units.unitSpawnTime, 0);

                    if (m_entityInRange)
                    {
                        OnQueueProgressUpdate?.Invoke(progressAmount);
                    }
                    time += Time.deltaTime;
                    
                    yield return new WaitForEndOfFrame();
                }
                
                OnQueueEnded?.Invoke(this);
                //Spawn(units, amount);
                m_unitQueue.Remove(units);
            }
        }
        m_isQueueRunning = false;
    }

    private void Update()
    {
        if (player == null)
        {
            foreach (var controller in MechCharacterController.List)
            {
                if (controller.OwnerClientId != NetworkManager.LocalClient.ClientId) continue;
                player = controller.gameObject;
            }
        }
        if(unitMover == null)
        {
            unitMover = GameObject.Find("UnitHandler");
        }
    }




    private void Spawn(AIUnitTypesData.UnitTypeInfo unit, int amount, ulong localPlayerId)
    {
        // spawn the spawn amount all at once of the specific type
        // loop through and use spawn amount 
        Debug.Log("Spawn - " + unit.unitType + " - Amount - " + unit.unitSpawnAmount);

        units = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            if (width < maxWidth - 1)
            {
                width += 1;
            }
            else
            {
                height += 1;
                width = 0;
            }

            Vector3 pos = transform.position + (i * Vector3.left * 2);

            // Get the team name of the player spawning the AI units
            FixedString32Bytes aiTeamName = "";
            foreach (MechCharacterController mcc in MechCharacterController.List)
            {
                if (localPlayerId == mcc.OwnerClientId)
                {
                    Entity mechEntity = mcc.GetComponent<Entity>();
                    aiTeamName = mechEntity.TeamName;
                    break;
                }
            }

            int baseId = controller.GetBaseId();
            aiUnitManager.SpawnAgentServerRpc(pos, transform.rotation, unit.unitType, localPlayerId, group.id, baseId, aiTeamName, unit.damageStrength, unit.teamColorMaterialIndex);
        }

        set = true;

        width = 0;
        height = 0;
    }

    
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerInMenuServerRpc(ulong id)
    {
        SetPlayerInMenuClientRpc(id);
    }

    [ClientRpc]
    private void SetPlayerInMenuClientRpc(ulong id)
    {
        playerId = id;
    }
}
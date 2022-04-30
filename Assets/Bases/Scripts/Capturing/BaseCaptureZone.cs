using System;
using System.Collections.Generic;
using FMOD;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BaseCaptureZone : NetworkBehaviour
{
    public event Action<Entity, BaseCaptureZone> OnEntityEntered;
    public event Action<Entity> OnEntityExited;
    public event Action<float> OnCaptureProgressUpdated;
    public event Action<string, Color> OnCaptureStatusUpdated;
    public event Action<string> OnBaseCaptured;

    public string ContestingTeam
    {
        get; private set;
    }

    public BaseController BaseController => m_baseController;

    [SerializeField] private BaseController m_baseController;
    [SerializeField] private GameTeamData m_teamData;

    [Header("Capture Zone Values")]
    [Tooltip("The rate the capture percentage increases or coolsdown with each player.")]
    [SerializeField] private float m_perPlayerCaptureRate = 0.5f;

    [Tooltip("The rate the capture percentage increases or coolsdown with each unit.")]
    [SerializeField] private float m_perUnitCaptureRate = 0.01f;

    [Tooltip("The base rate the capture percentage cooldowns with no contesting entities in the zone.")]
    [SerializeField] private float m_contestingCooldownRate = 0.05f;

    [Header("Pleayer Healing Values")]
    [SerializeField] private float m_delayInHealing = 3.0f;
    [SerializeField] private int m_amountToHeal = 10;

    private Dictionary<string, List<ulong>> m_playersInZone = new Dictionary<string, List<ulong>>();
    private Dictionary<string, List<ulong>> m_unitsInZone = new Dictionary<string, List<ulong>>();

    private float m_captureProgress = 0;
    private bool contestCheck;
    public event Action<Color> OnPlayerColorEntered;

    private float m_playerHealingTimer = 0;

    public bool IsLocalPlayerInZone()
    {
        foreach (KeyValuePair<string, List<ulong>> player in m_playersInZone)
        {
            foreach (ulong id in player.Value)
            {
                if (id == NetworkManager.Singleton.LocalClientId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Start()
    {
        if(!IsHost)
        {
            GetComponent<Collider>().enabled = false;
        }

        m_baseController.OnStateChanged += OnStateChanged;
    }

    private void Update()
    {
        if (IsHost)
        {
            float currentProgress = m_captureProgress;

            UpdateCaptureProgress();
            UpdateCooldown();

            if (currentProgress != m_captureProgress) UpdateProgressServerRpc(m_captureProgress);

            UpdateHealingPlayers();
        }
    }
    
    [ServerRpc]
    void UpdateBaseStateServerRpc(EBaseState newState)
    {
        UpdateBaseStateClientRpc(newState);
    }

    [ClientRpc]
    void UpdateBaseStateClientRpc(EBaseState newState)
    {
        m_baseController.ChangeState(newState);
    }

    [ServerRpc]
    void UpdateBaseOwnerServerRpc(string newTeamOwner)
    {
        UpdateBaseOwnerClientRpc(newTeamOwner);
    }

    [ClientRpc]
    void UpdateBaseOwnerClientRpc(string newTeamOwner)
    {
        m_baseController.ChangeTeamOwner(newTeamOwner);
        OnBaseCaptured?.Invoke(newTeamOwner);
    }

    [ServerRpc]
    void UpdateProgressServerRpc(float newProgress)
    {
        UpdateProgressClientRpc(newProgress);
    }

    [ClientRpc]
    void UpdateProgressClientRpc(float newProgress)
    {
        m_captureProgress = newProgress;
        OnCaptureProgressUpdated?.Invoke(newProgress);
    }

    [ServerRpc]
    void UpdateTeamColorServerRpc(Color teamColor)
    {
        UpdateTeamColorClientRpc(teamColor);
    }

    [ClientRpc]
    void UpdateTeamColorClientRpc(Color teamColor)
    {
        OnPlayerColorEntered?.Invoke(teamColor);
    }

    [ServerRpc]
    void UpdateContestingTeamServerRpc(string TeamName)
    {
        UpdateContestingTeamClientRpc(TeamName);
    }

    [ClientRpc]
    void UpdateContestingTeamClientRpc(string TeamName)
    {
        ContestingTeam = TeamName;
    }

    [ServerRpc]
    void UpdateOnEntityEnteredServerRpc(NetworkObjectReference entityRef)
    {
        UpdateOnEntityEnteredClientRpc(entityRef);
    }

    [ClientRpc]
    void UpdateOnEntityEnteredClientRpc(NetworkObjectReference entityRef)
    {
        NetworkObject networkObject;
        entityRef.TryGet(out networkObject);
        if (networkObject != null)
        {
            Entity entity = networkObject.GetComponent<Entity>();
            if (entity != null)
            {
                OnEntityEntered?.Invoke(entity, this);
            }
        }
    }

    [ServerRpc]
    void UpdateOnEntityExitedServerRpc(NetworkObjectReference entityRef)
    {
        UpdateOnEntityExitedClientRpc(entityRef);
    }

    [ClientRpc]
    void UpdateOnEntityExitedClientRpc(NetworkObjectReference entityRef)
    {
        NetworkObject networkObject;
        entityRef.TryGet(out networkObject);
        if (networkObject != null)
        {
            Entity entity = networkObject.GetComponent<Entity>();
            if (entity != null)
            {
                OnEntityExited?.Invoke(entity);
            }
        }
    }

    private void UpdateCaptureProgress()
    {
        if (m_baseController.State == EBaseState.CONTESTED && !HasFriendlyEntitiesInZone() && IsOnlyOneTeamContesting(out string contestingTeamName))
        {
            CheckAndAddTeamInDictionaries(m_baseController.TeamOwner);

            m_captureProgress += m_playersInZone[contestingTeamName].Count * m_perPlayerCaptureRate * Time.deltaTime * BasesCaptureController.Instance.CaptureMultiplyer;
            m_captureProgress += m_unitsInZone[contestingTeamName].Count * m_perUnitCaptureRate * Time.deltaTime * BasesCaptureController.Instance.CaptureMultiplyer;

            if (m_captureProgress >= 1.0f)
            {
                UpdateContestingTeamServerRpc("");
                UpdateBaseOwnerServerRpc(contestingTeamName);
                UpdateBaseStateServerRpc(EBaseState.IDLE);
                m_captureProgress = 0;
            }
        }
    }

    private void UpdateCooldown()
    {
        if (m_baseController.State == EBaseState.COOLDOWN)
        {
            CheckAndAddTeamInDictionaries(m_baseController.TeamOwner);

            m_captureProgress -= m_contestingCooldownRate * Time.deltaTime;
            m_captureProgress -= m_playersInZone[m_baseController.TeamOwner].Count * m_perPlayerCaptureRate * Time.deltaTime;
            m_captureProgress -= m_unitsInZone[m_baseController.TeamOwner].Count * m_perUnitCaptureRate * Time.deltaTime;

            if (m_captureProgress <= 0)
            {
                UpdateContestingTeamServerRpc("");
                UpdateBaseStateServerRpc(EBaseState.IDLE);
                m_captureProgress = 0;
            }
        }
    }

    private void UpdateHealingPlayers()
    {
        if (m_baseController.State != EBaseState.IDLE)
        {
            return;
        }

        CheckAndAddTeamInDictionaries(m_baseController.TeamOwner);

        m_playerHealingTimer += Time.deltaTime;
        if (m_playerHealingTimer >= m_delayInHealing)
        {
            m_playerHealingTimer = 0;
            foreach (ulong id in m_playersInZone[m_baseController.TeamOwner])
            {
                Entity entity = TransformManager.GetActiveEntityFromClientID(id);
                entity.GetComponent<CombatComponent>().HealDamageServerRpc(m_amountToHeal);
            }
        }
    }

    private bool IsEntityInZone(Entity entity)
    {
        foreach (KeyValuePair<string, List<ulong>> player in m_playersInZone)
        {
            foreach (ulong id in player.Value)
            {
                if (entity.clientId == id)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity == null)
        {
            // This will detect if the playeer switches to vtol while inside a base
            Networking.NetworkTransformComponent vtol = other.gameObject.GetComponent<Networking.NetworkTransformComponent>();
            if (vtol != null)
            {
                entity = vtol.mechSwitchScript.gameObject.GetComponent<Entity>();
                if (entity != null)
                {
                    HandleEntityExited(entity);
                }
            }
            return;
        }

        CheckAndAddTeamInDictionaries(entity.TeamName);

        if (entity.EntityType == Entity.EEntityType.PLAYER || entity.EntityType == Entity.EEntityType.UNIT)
        {
            HandleEntityEntered(entity);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Entity entity = other.gameObject.GetComponentInParent<Entity>();

        if (entity == null)
        {
            return;
        }

        CheckAndAddTeamInDictionaries(entity.TeamName);

        if (entity.EntityType == Entity.EEntityType.PLAYER)
        {
            CombatComponent player = entity.GetComponent<CombatComponent>();
            if (!player.IsAlive())
            {
                HandleEntityExited(entity);
                return;
            }
            else if (!m_unitsInZone.ContainsKey(entity.TeamName) || !m_unitsInZone[entity.TeamName].Contains(entity.clientId))
            {
                HandleEntityEntered(entity);
            }
        }
        else if (entity.EntityType == Entity.EEntityType.UNIT)
        {
            HandleEntityEntered(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();

        if (entity == null)
        {
            return;
        }

        if (entity.EntityType == Entity.EEntityType.PLAYER || entity.EntityType == Entity.EEntityType.UNIT)
        {
            HandleEntityExited(entity);
        }
    }

    private void HandleEntityEntered(Entity entity)
    {
        if (!m_unitsInZone[entity.TeamName].Contains(entity.clientId))
        {
            m_unitsInZone[entity.TeamName].Add(entity.clientId);
        }

        if (m_baseController.TeamOwner != entity.TeamName)
        {
            if (m_baseController.State == EBaseState.COOLDOWN)
            {
                if (ContestingTeam == entity.TeamName)
                {
                    UpdateContestingTeamClientRpc(entity.TeamName);
                    UpdateBaseStateClientRpc(EBaseState.CONTESTED);
                }
            }
            else if (m_baseController.State == EBaseState.IDLE)
            {
                UpdateContestingTeamClientRpc(entity.TeamName);
                UpdateBaseStateClientRpc(EBaseState.CONTESTED);
                OnCaptureStatusUpdated?.Invoke("CAPTURING", m_teamData.GetTeamData(entity.TeamName).Colour);
                OnPlayerColorEntered?.Invoke(m_teamData.GetTeamData(entity.TeamName).Colour);
                UpdateTeamColorClientRpc(m_teamData.GetTeamData(entity.TeamName).Colour);
            }
        }

        if (m_baseController.State != EBaseState.IDLE)
        {
            UpdateOnEntityEnteredClientRpc(new NetworkObjectReference(entity.GetComponent<NetworkObject>()));
            OnCaptureStatusUpdated?.Invoke("CAPTURING", m_teamData.GetTeamData(entity.TeamName).Colour);
            OnPlayerColorEntered?.Invoke(m_teamData.GetTeamData(entity.TeamName).Colour);
            UpdateTeamColorClientRpc(m_teamData.GetTeamData(entity.TeamName).Colour);
        }
    }

    private void HandleEntityExited(Entity entity)
    {
        if (m_unitsInZone.ContainsKey(entity.TeamName) && m_unitsInZone[entity.TeamName].Contains(entity.clientId))
        {
            m_unitsInZone[entity.TeamName].Remove(entity.clientId);
        }

        UpdateOnEntityExitedClientRpc(new NetworkObjectReference(entity.GetComponent<NetworkObject>()));

        if (m_baseController.TeamOwner != entity.TeamName)
        {
            if (!HasEnemyEntitiesInZone(out List<string> teamsInZone))
            {
                UpdateBaseStateServerRpc(EBaseState.COOLDOWN);
            }
        }
    }

    private void OnStateChanged(EBaseState state)
    {
        if (state == EBaseState.IDLE && HasEnemyEntitiesInZone(out List<string> teamsInZone))
        {
            string newContestingTeam = "";
            float numEntities = -Mathf.Infinity;
            foreach (string team in teamsInZone)
            {
                if (m_playersInZone[team].Count + m_unitsInZone[team].Count > numEntities)
                {
                    newContestingTeam = team;
                    numEntities = m_playersInZone[team].Count + m_unitsInZone[team].Count;
                }
            }

            UpdateContestingTeamServerRpc(newContestingTeam);
            m_baseController.ChangeState(EBaseState.CONTESTED);
        }
    }

    private bool HasEnemyEntitiesInZone(out List<string> teamsInZone)
    {
        teamsInZone = new List<string>();

        foreach (KeyValuePair<string, List<ulong>> team in m_playersInZone)
        {
            if (m_baseController.TeamOwner != team.Key && team.Value.Count != 0)
            {
                teamsInZone.Add(team.Key);
            }
        }

        foreach (KeyValuePair<string, List<ulong>> team in m_unitsInZone)
        {
            if (m_baseController.TeamOwner != team.Key && team.Value.Count != 0)
            {
                teamsInZone.Add(team.Key);
            }
        }

        return teamsInZone.Count != 0;
    }

    private bool HasFriendlyEntitiesInZone()
    {
        CheckAndAddTeamInDictionaries(m_baseController.TeamOwner);

        return m_playersInZone[m_baseController.TeamOwner].Count != 0 || m_unitsInZone[m_baseController.TeamOwner].Count != 0;
    }

    private void CheckAndAddTeamInDictionaries(string teamName)
    {
        if (!m_playersInZone.ContainsKey(teamName))
        {
            m_playersInZone.Add(teamName, new List<ulong>());
        }

        if (!m_unitsInZone.ContainsKey(teamName))
        {
            m_unitsInZone.Add(teamName, new List<ulong>());
        }
    }

    private bool IsOnlyOneTeamContesting(out string contestingTeamName)
    {
        bool oneTeamDetected = false;
        contestingTeamName = "";
        foreach (KeyValuePair<string, List<ulong>> team in m_playersInZone)
        {
            if (m_baseController.TeamOwner != team.Key && team.Value.Count != 0)
            {
                if (oneTeamDetected)
                {
                    return false;
                }
                else
                {
                    oneTeamDetected = true;
                    contestingTeamName = team.Key;
                }
            }
        }

        foreach (KeyValuePair<string, List<ulong>> team in m_unitsInZone)
        {
            if (m_baseController.TeamOwner != team.Key && team.Value.Count != 0)
            {
                if (oneTeamDetected && contestingTeamName != team.Key)
                {
                    return false;
                }
                else
                {
                    oneTeamDetected = true;
                    contestingTeamName = team.Key;
                }
            }
        }

        return true;
    }

    public void SetCaptureProgress(float progress)
    {
        UpdateProgressServerRpc(progress);
    }
}

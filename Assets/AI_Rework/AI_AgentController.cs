using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.Collections;

public enum AgentState
{
    ATTACKING,
    DEFENDING
}

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Unity.Netcode.Components.NetworkTransform))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Entity))]
public class AI_AgentController : NetworkBehaviour
{
    public EUnitTypes UnitType => unitType;

    [SerializeField] private GameTeamData teamData;

    public DefenceLocationManager defenceLocationManager;
    public AgentState agentState = AgentState.DEFENDING;
    public bool move;

    private NavMeshAgent navMeshAgent;
    private EUnitTypes unitType;
    private Entity entityComponent;
    private int damageStrength;
    private DefenceLocationManager lastDefenceLocationManager;
    private DefenceLocationManager resetDefenceLocationManager;

    public override void OnNetworkSpawn()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        entityComponent = GetComponent<Entity>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNavMeshAgentDestinationServerRpc(Vector3 position)
    {
        SetNavMeshDestination(position);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopNavMeshAgentMovementServerRpc()
    {
        StopNavMeshAgentMovement();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResumeNavMeshAgentMovementServerRpc()
    {
        ResumeNavMeshAgentMovement();
    }

    public void SetDefenceLocationManager(int baseId)
    {
        if(defenceLocationManager != null)
        {
            lastDefenceLocationManager = defenceLocationManager;
        }
        foreach(BaseController baseController in FindObjectOfType<GameController>().m_allBases)
        { 
            if(baseId == baseController.GetBaseId())
            {
                defenceLocationManager = baseController.m_defenceLocationManager;
                break;
            }
        }
    }

    public void MoveToAttackPosition(int groupIndex, int baseId)
    {
        agentState = AgentState.ATTACKING;
        move = true;
        SetDefenceLocationManager(baseId);

        for (int x = 0; x < 5; x++)
        {
            for (int i = 0; i < 5; i++)
            {
                DefenceLocationInfo locationInfo = defenceLocationManager.unitDefenceLocations[i].defenceLocationInfo[x];

                if(!locationInfo.enemyOccupied && move)
                {
                    SetNavMeshDestination(locationInfo.location);
                    RemoveDefenceLocationInfo(groupIndex);

                    locationInfo.enemyOccupied = true;
                    locationInfo.enemyGroupId = groupIndex;
                    locationInfo.enemyUnit = gameObject;
                    move = false;
                    break;
                }
            }
        }
    }
    
    public void MoveToDefendPosition(int groupIndex, int baseId)
    {
        agentState = AgentState.DEFENDING;
        move = true;
        SetDefenceLocationManager(baseId);

        for (int x = 0; x < 5; x++)
        {
            for (int i = 0; i < 5; i++)
            {
                DefenceLocationInfo locationInfo = defenceLocationManager.unitDefenceLocations[i].defenceLocationInfo[x];
                if (!locationInfo.occupied && move)
                {
                    SetNavMeshDestination(locationInfo.location);
                    RemoveDefenceLocationInfo(groupIndex);

                    locationInfo.occupied = true;
                    locationInfo.groupId = groupIndex;
                    locationInfo.unit = gameObject;
                    move = false;
                    break;
                }
            }
        }
    }

    public void SetNewTeamDefenceLocationInfo(int groupIndex, int baseId)
    {
        foreach (BaseController baseController in FindObjectOfType<GameController>().m_allBases)
        {
            if (baseId == baseController.GetBaseId())
            {
                resetDefenceLocationManager = baseController.m_defenceLocationManager;
            }
        }

        for (int x = 0; x < 5; x++)
        {
            for (int i = 0; i < 5; i++)
            {
                DefenceLocationInfo locationInfo = resetDefenceLocationManager.unitDefenceLocations[i].defenceLocationInfo[x];

                if (locationInfo.enemyOccupied && locationInfo.enemyGroupId == groupIndex)
                {
                    locationInfo.occupied = true;
                    locationInfo.unit = locationInfo.enemyUnit;
                    locationInfo.groupId = locationInfo.enemyGroupId;
                    locationInfo.enemyOccupied = false;
                    locationInfo.enemyUnit = null;
                    locationInfo.enemyGroupId = -1;
                }
                else
                {
                    locationInfo.occupied = false;
                    locationInfo.groupId = -1;
                    locationInfo.unit = null;
                }
            }
        }
    }

    public void RemoveDefenceLocationInfo(int groupIndex)
    {
        if(lastDefenceLocationManager != null)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int i = 0; i < 5; i++)
                {
                    DefenceLocationInfo locationInfo = lastDefenceLocationManager.unitDefenceLocations[i].defenceLocationInfo[x];

                    if (locationInfo.groupId == groupIndex)
                    {
                        locationInfo.occupied = false;
                        locationInfo.groupId = -1;
                        locationInfo.unit = null;
                    }
                }
            }
        }
    }


    public void SetUnitType(EUnitTypes type)
    {
        if (!IsServer) return;

        unitType = type;
    }

    public EUnitTypes GetUnitType()
    {
        return unitType;
    }

    public void SetEntityTeamName(FixedString32Bytes teamName)
    {
        if (!IsServer) return;

        entityComponent.ChangeTeamClientRpc(teamName);

        // Set the AI agents to the correct layer mask for their team
        // NOTE Red team must come first in the team data scriptable object
        // 25 - AI_RED
        // 26 - AI_BLUE
        int redLayer = 25;
        int blueLayer = 26;

        // If the team name being set to is the same as the first team's in the team data SO. Assuming that this will be red team
        if(teamName.ToString() == teamData.GetTeamDataAtIndex(0).TeamName)
        {
            gameObject.layer = redLayer;
        }
        else
        {
            gameObject.layer = blueLayer;
        }
    }

    public void SetDamageStrength(int strength)
    {
        if (!IsServer) return;

        damageStrength = strength;
    }

    public int GetDamageStrength()
    {
        return damageStrength;
    }

    // NOTE Only call this function from the server
    private void SetNavMeshDestination(Vector3 position)
    {
        //Debug.Log("Setting nav mesh destination: " + position + (IsServer ? " [Server]" : " [Client]"));
        navMeshAgent.SetDestination(position);
        navMeshAgent.speed = 20;
    }

    // NOTE Only call this function from the server
    private void StopNavMeshAgentMovement()
    {
        navMeshAgent.isStopped = true;
    }

    // NOTE Only call this function from the server
    private void ResumeNavMeshAgentMovement()
    {
        navMeshAgent.isStopped = false;
    }
}

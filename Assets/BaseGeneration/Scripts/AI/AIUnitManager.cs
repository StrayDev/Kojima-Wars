using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

[RequireComponent(typeof(NetworkObject))]
public class AIUnitManager : NetworkBehaviour
{
    [SerializeField] GameObject aiAgentPrefab;
    [SerializeField] private AIUnitTypesData unitTypesData;
    [SerializeField] private PlayerDataSO playerDataSO;
    [SerializeField] private GameTeamData teamDataSO;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnAgentServerRpc(Vector3 position, Quaternion rotation, EUnitTypes type, ulong localPlayerID, int groupIndex, int baseId, FixedString32Bytes teamName, int damageStrength,
        int teamColorMaterialIndex)
    {
        //Debug.Log("Spawn AI agent [position: " + position + "] [rotation: " + rotation + "]");

        // Spawn agent
        GameObject agent = Instantiate(aiAgentPrefab, position, rotation);
        // Spawn agent on all client machines
        agent.GetComponent<NetworkObject>().Spawn();

        // Setup the agent
        AI_AgentController agentController = agent.GetComponent<AI_AgentController>();
        agentController.SetUnitType(type);
        agentController.SetEntityTeamName(teamName);
        agentController.SetDamageStrength(damageStrength);

        // Spawn model prefab for the unit type parented to the agent
        AIUnitTypesData.UnitTypeInfo info = unitTypesData.GetUnitInfo(type);
        GameObject model = Instantiate(info.modelPrefab);
        model.transform.position = position;
        model.GetComponent<NetworkObject>().Spawn(); // Owned by the server 
        model.transform.SetParent(agent.transform);

        // Swap the model material to the team material
        // Assuming team 0 in the team data SO is the red team
        int teamIndex = (teamName.ConvertToString() == teamDataSO.GetTeamDataAtIndex(0).TeamName) ? 0 : 1;
        SetAgentTeamColorClientRpc(model, teamIndex, teamColorMaterialIndex);

        // Add units to group in player data
        playerDataSO.AddAgentToGroup(localPlayerID, groupIndex, agent);

        BaseController selectedBaseController = null;
        BaseController[] baseControllers = FindObjectsOfType<BaseController>();
        foreach (BaseController baseController in baseControllers)
        {
            if (baseId == baseController.GetBaseId())
            {
                selectedBaseController = baseController;
                break;
            }
        }

        playerDataSO.SetBaseControllerForGroup(localPlayerID, groupIndex, selectedBaseController);

        agent.GetComponent<AI_AgentController>().MoveToDefendPosition(groupIndex, baseId);

        //Debug.Log("Agents in group " + groupIndex + ": " + playerDataSO.GetAIGroup(localPlayerID, groupIndex).agents.Count);
    }

    [ClientRpc]
    public void SetAgentTeamColorClientRpc(NetworkObjectReference agentModelRef, int teamIndex, int materialIndex)
    {
        NetworkObject agentModelNetworkObject = null;
        agentModelRef.TryGet(out agentModelNetworkObject);

        Color teamColor = teamDataSO.GetTeamDataAtIndex(teamIndex).Colour;
        agentModelNetworkObject.GetComponent<Renderer>().materials[materialIndex].color = teamColor;
    }
}

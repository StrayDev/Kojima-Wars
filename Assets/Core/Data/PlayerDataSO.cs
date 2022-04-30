using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGroup
{
    public static int MaxGroupsPerPlayer = 10;

    public BaseController BaseController = null;
    public List<GameObject> agents = new List<GameObject>();
}

public class PlayerData
{
    public PlayerData()
    {
        for(int i = 0; i < AIGroup.MaxGroupsPerPlayer; ++i)
        {
            aiGroups.Add(new AIGroup());
        }
    }

    public string Name { get; private set; } = "";
    public void SetName(string n) { Name = n; }

    public bool IsRedTeam { get; private set; } = false;
    public void SetTeamRed(bool isRed) { IsRedTeam = isRed; }

    // AI
    public List<AIGroup> aiGroups = new List<AIGroup>(AIGroup.MaxGroupsPerPlayer);
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public Dictionary<ulong, PlayerData> List { get; private set; }

    private PlayerDataSO()
    {
        // WARNING : you cannot do this inline with the declaration
        List = new Dictionary<ulong, PlayerData>();
    }

    public PlayerData GetPlayerData(ulong id)
    {
        return List[id];
    }

    public bool GetPlayerTeam(ulong id)
    {
        return List[id].IsRedTeam;
    }   

    public string GetPlayerName(ulong id)
    {
        return List[id].Name;
    }

    public AIGroup GetAIGroup(ulong id, int groupIndex)
    {
        return List[id].aiGroups[groupIndex];
    }

    public List<AIGroup> GetAIGroups(ulong id)
    {
        return List[id].aiGroups;
    }

    public void SetAIGroup(ulong id, int groupIndex, AIGroup aiGroup)
    {
        List[id].aiGroups[groupIndex] = aiGroup;
    }

    // Helper function to add units to a player's ai group
    public void AddAgentToGroup(ulong id, int groupIndex, GameObject agent)
    {
        List[id].aiGroups[groupIndex].agents.Add(agent);
    }

    // Helper function to remove units from a player's ai group
    public void RemoveAgentFromGroup(ulong id, int groupIndex, GameObject agent)
    {
        List[id].aiGroups[groupIndex].agents.Remove(agent);
        // NOTE Remember to reset the base controller to null if the group is empty
    }

    // Helper function to set base controller in a player's ai group
    public void SetBaseControllerForGroup(ulong id, int groupIndex, BaseController controller)
    {
        List[id].aiGroups[groupIndex].BaseController = controller;
    }
}

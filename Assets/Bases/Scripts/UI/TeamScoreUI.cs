using System.Collections.Generic;
using UnityEngine;

public class TeamScoreUI : MonoBehaviour
{
    [SerializeField] private Transform m_scoresHolder = default;
    [SerializeField] private GameTeamData m_gameTeamData = default;
    [SerializeField] private GameStateDataSO m_gameStateData = default;
    [SerializeField] private List<TeamScoreObject> m_teamScoreObjects = new List<TeamScoreObject>();

    private Dictionary<string, TeamScoreObject> m_teamUIMap = new Dictionary<string, TeamScoreObject>();

    private void Awake()
    {
        var index = 0;
        foreach (var objects in m_teamScoreObjects)
        {
            m_teamUIMap.Add(m_gameTeamData.GetTeamDataAtIndex(index).TeamName, objects);
            objects.SetTeamColour(m_gameTeamData.GetTeamData(m_gameTeamData.GetTeamDataAtIndex(index).TeamName).Colour);
            objects.SetTeamScoreText(0, m_gameStateData.scoreThreshold);
            index++;
        }
    }

    public void OnTeamScoreUpdated(string teamName, int score)
    {
        if (m_teamUIMap.ContainsKey(teamName))
        {
            m_teamUIMap[teamName].SetTeamScoreText(score, m_gameStateData.scoreThreshold);
        }
        else
        {
            Debug.LogWarning($"Team {teamName} added after initialisation of score UI, adding new UI display for new team.");
        }
    }
}
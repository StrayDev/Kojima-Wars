using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game Team Data")]
public class GameTeamData : ScriptableObject
{
    [Serializable]
    public struct TeamData
    {
        public string TeamName;
        public Color Colour;
        public Material Material;
        public int Score;
    }

    public int TeamCount => m_teamDatas.Length;

    public Color DefaultTeamColour => m_defaultColour;

    [SerializeField] private Color m_defaultColour = new Color(0.2f, 0.2f, 0.2f);

    [SerializeField] private TeamData[] m_teamDatas = new TeamData[] { };

    private Dictionary<string, TeamData> m_teamDataDictionary = new Dictionary<string, TeamData>();

    public TeamData GetTeamData(string teamName)
    {
        return m_teamDataDictionary[teamName];
    }

    public void SetTeamData(string teamName, TeamData teamData)
    {
        m_teamDataDictionary[teamName] = teamData;
    }

    public ref Dictionary<string, TeamData> GetTeamDataDictionary()
    {
        return ref m_teamDataDictionary;
    }

    public TeamData GetTeamDataAtIndex(int index)
    {
        return m_teamDatas[index];
    }

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        m_teamDataDictionary = new Dictionary<string, TeamData>();
        foreach (TeamData data in m_teamDatas)
        {
            m_teamDataDictionary.Add(data.TeamName, data);
        }
    }
}

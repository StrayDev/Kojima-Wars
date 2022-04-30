using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeamUI : MonoBehaviour
{
    [SerializeField] private GameTeamData m_gameTeamData;
    [SerializeField] private Image m_teamIconImage;
    [SerializeField]private Image m_teamColourImage;

    private void OnEnable()
    {
        PlayerInformation.OnPlayerTeamSet += TeamNameSet;
    }

    private void OnDisable()
    {
        PlayerInformation.OnPlayerTeamSet -= TeamNameSet;
    }
    
    private void TeamNameSet(string teamName)
    {
        var teamData = m_gameTeamData.GetTeamData(teamName);
        m_teamColourImage.color = teamData.Colour;
    }
}

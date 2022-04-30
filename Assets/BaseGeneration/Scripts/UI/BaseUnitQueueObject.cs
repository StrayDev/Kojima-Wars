using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitQueueObject : MonoBehaviour
{
    [SerializeField] private Image m_unitIcon = default;
    [SerializeField] private Image m_progressFill = default;
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private GameTeamData gameTeamData;

    public List<Image> images = new List<Image>();

    private AIUnitTypesData.UnitTypeInfo m_unitdata;
    
    [SerializeField] private Color m_playerColour = Color.white;
    
    public void SetUnitData(AIUnitTypesData.UnitTypeInfo unitData, BaseUnitGeneration unitGenerationBase)
    {
        m_unitdata = unitData;
        
        m_progressFill.fillAmount = 0;
        m_unitIcon.sprite = unitData.unitImage;
        transform.localScale = Vector3.one;
    }

    public void UpdateProgress(float amount)
    {
        m_progressFill.fillAmount = amount;
    }
    

    // Update is called once per frame
    void Update()
    {
        /*
        if (m_playerColour == Color.white)
        {
            foreach (var player in FindObjectsOfType<MechCharacterController>())
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                if (networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    bool isRedTeam = playerData.GetPlayerTeam(networkObject.OwnerClientId);
                    m_playerColour = gameTeamData.GetTeamData((isRedTeam ? gameTeamData.GetTeamDataAtIndex(0).TeamName : gameTeamData.GetTeamDataAtIndex(1).TeamName)).Colour;
                    Debug.Log(m_playerColour);
                    foreach (var image in images)
                    {
                        image.color = m_playerColour;
                    }
                }
            }
        }
        */
    }
}

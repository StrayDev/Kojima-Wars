using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BaseUnitOptionsObject : MonoBehaviour
{
    public static event Action<AIUnitTypesData.UnitTypeInfo> OnTrainUnit;
    public static event Action<AIUnitTypesData.UnitTypeInfo, ulong> OnInstantSpawnUnit;
    public static event Action<BaseUnitOptionsObject> OnSelected; 
    
    private AIUnitTypesData.UnitTypeInfo m_unitTypeInfo;
    private BaseUnitGeneration m_interactingBase;
    private int instantSpawnAmount;

    [Header("General Information")] 
    [SerializeField] private bool isInstantObject;
    public BaseController controller;
    
    [Header("Display References")]
    [SerializeField] private Image m_unitIcon = default;
    //[SerializeField] private Image m_keyIcon = default;
    [SerializeField] private Image m_costBackgroundImage = default;
    [SerializeField] private TextMeshProUGUI m_costText = default;
    [SerializeField] private List<Image> images = new List<Image>();


    [Header("Selection References")]
    [SerializeField] private Key m_keyPress;

    [Header("Color References")]
    [SerializeField] private Color m_defaultColour = Color.black;
    [SerializeField] private Color m_playerColour = Color.white;
    [SerializeField] private Color m_cannotAffordColour = Color.red;
    [SerializeField] private Color m_teamRed;
    [SerializeField] private Color m_teamBlue;

    private void OnDisable()
    {
        m_interactingBase = null;
    }

    public void SetOptionsData(AIUnitTypesData.UnitTypeInfo unitInfo, BaseUnitGeneration unitGenerationBase)
    {
        m_unitTypeInfo = unitInfo;
        m_interactingBase = unitGenerationBase;
        //m_unitIcon.sprite = m_unitTypeInfo.unitImage;
        instantSpawnAmount = m_unitTypeInfo.unitCost;
        m_costText.text = isInstantObject ? instantSpawnAmount.ToString() : m_unitTypeInfo.unitCost.ToString();
    }
    
    void Update()
    {
        if (controller.TeamOwner == "red")
        {
            m_playerColour = m_teamRed;
        }
        else
        {
            m_playerColour = m_teamBlue;
        }

        foreach (var image in images)
        {
            image.color = m_playerColour;
        }
        
        if (m_unitTypeInfo != null)
        {
            if (Keyboard.current[m_keyPress].wasPressedThisFrame && isInstantObject)
            {
                OnInstantSpawnUnit?.Invoke(m_unitTypeInfo, NetworkManager.Singleton.LocalClientId);
                OnSelected?.Invoke(this);
            }
            else if (Keyboard.current[m_keyPress].wasPressedThisFrame)
            {
                Debug.Log("Train Units");
                OnTrainUnit?.Invoke(m_unitTypeInfo);
                OnSelected?.Invoke(this);
            }
        }
        
        if (m_interactingBase != null)
        {
            UpdateDisplay(m_interactingBase.PlayerInRange.Currency >= m_unitTypeInfo.unitCost);
        }
    }

    public void SpawnUnits()
    {
        if (m_unitTypeInfo != null)
        {
            if (isInstantObject)
            {
                OnInstantSpawnUnit?.Invoke(m_unitTypeInfo, NetworkManager.Singleton.LocalClientId);
                OnSelected?.Invoke(this);
            }
            else
            {
                Debug.Log("Train Units");
                OnTrainUnit?.Invoke(m_unitTypeInfo);
                OnSelected?.Invoke(this);
            }
        }
    }
    
    private void UpdateDisplay(bool canAfford)
    {
        //m_keyIcon.enabled = canAfford;
        //m_costBackgroundImage.color = canAfford ? m_defaultColour : m_cannotAffordColour;
    }
}

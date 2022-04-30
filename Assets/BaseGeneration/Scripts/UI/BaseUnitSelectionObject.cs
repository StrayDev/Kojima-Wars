using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitSelectionObject : MonoBehaviour
{
    public static event Action<AIUnitTypesData.UnitTypeInfo, BaseUnitGeneration> OnUnitSelected;

    [SerializeField] private GameTeamData m_gameTeamData = default;

    [Header("Display References")]
    [SerializeField] private Button m_buyButton = default;
    [SerializeField] private Image m_unitIcon = default;
    [SerializeField] private Image m_iconBackgroundImage = default;
    [SerializeField] private TextMeshProUGUI m_costText = default;
    [SerializeField] private TextMeshProUGUI m_nameText = default;
    [SerializeField] private TextMeshProUGUI m_descriptionText = default;

    private AIUnitTypesData.UnitTypeInfo m_unitTypeInfo;
    private BaseUnitGeneration m_interactingBase;
    
    public void SetUnitData(AIUnitTypesData.UnitTypeInfo unitData, BaseUnitGeneration unitGenerationBase)
    {
        m_interactingBase = unitGenerationBase;
        m_unitTypeInfo = unitData;
        m_unitIcon.sprite = m_unitTypeInfo.unitImage;
        m_nameText.text = m_unitTypeInfo.unitTitle;
        m_descriptionText.text = m_unitTypeInfo.unitDesc;
        m_costText.text = m_unitTypeInfo.unitCost.ToString();
    }
    public void SelectUnit()
    {
        OnUnitSelected?.Invoke(m_unitTypeInfo, m_interactingBase);
    }

    private void OnDisable()
    {
        m_interactingBase = null;
    }

    private void Update()
    {
        if (m_interactingBase != null)
        {
            m_iconBackgroundImage.color = m_gameTeamData.GetTeamData(m_interactingBase.controller.TeamOwner).Colour;
            m_buyButton.interactable = m_interactingBase.PlayerInRange.Currency >= m_unitTypeInfo.unitCost;
        }
    }
}
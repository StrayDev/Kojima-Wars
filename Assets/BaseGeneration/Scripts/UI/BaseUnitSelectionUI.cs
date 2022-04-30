using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject m_uiContent = default;
    [SerializeField] private GameObject m_groupButtonsHolder = default;
    [SerializeField] private GameObject m_optionsContent;
    [SerializeField] private List<BaseUnitSelectionObject> m_buttons = new List<BaseUnitSelectionObject>();
    [SerializeField] private List<BaseUnitOptionsObject> m_optionButtons = new List<BaseUnitOptionsObject>();
    [SerializeField] private AIUnitTypesData m_unitData = default;

    [SerializeField] private List<SelectGroup> m_unitGroupButtons = new List<SelectGroup>();

    private BaseUnitGeneration m_interactingBase = null;

    private void Awake()
    {
        m_uiContent.SetActive(false);
        m_optionsContent.SetActive(false);
        m_groupButtonsHolder.SetActive(false);
    }

    private void OnEnable()
    {
        BaseUnitSelectionObject.OnUnitSelected += OnUnitSelected;
        BaseUnitOptionsObject.OnSelected += OnOptionsSelected;
        
        BaseUnitGeneration.OnInteracted += OnInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange += OnLocalPlayerExited;
    }

    private void OnDisable()
    {
        BaseUnitSelectionObject.OnUnitSelected -= OnUnitSelected;
        BaseUnitOptionsObject.OnSelected -= OnOptionsSelected;
        
        BaseUnitGeneration.OnInteracted -= OnInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange -= OnLocalPlayerExited;
    }

    private void OnInteracted(BaseUnitGeneration unitGenerationBase)
    {
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager.paused)
        {
            pauseManager.Unpause();
        }
        else
        {
            pauseManager.Pause(false);
        }

        SetUnitButtons(unitGenerationBase);
        m_uiContent.SetActive(!m_uiContent.activeSelf);
        m_interactingBase = unitGenerationBase;

        foreach (var button in m_optionButtons)
        {
            button.controller = m_interactingBase.controller;
        }
    }
    
    private void OnLocalPlayerExited(Entity player, BaseUnitGeneration unitGenerationBase)
    {
        m_uiContent.SetActive(false);
        m_optionsContent.SetActive(false);
        m_groupButtonsHolder.SetActive(false);

        foreach (SelectGroup groupButton in m_unitGroupButtons)
        {
            groupButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    
    private void OnUnitSelected(AIUnitTypesData.UnitTypeInfo unitInfo, BaseUnitGeneration unitGenerationBase)
    {
        m_uiContent.SetActive(false);
        m_groupButtonsHolder.SetActive(true);

        foreach (SelectGroup groupButton in m_unitGroupButtons)
        {
            groupButton.GetComponent<Button>().onClick.AddListener(() => OnGroupSelected(unitInfo, unitGenerationBase, groupButton));
        }
    }

    private void OnGroupSelected(AIUnitTypesData.UnitTypeInfo unitInfo, BaseUnitGeneration unitGenerationBase, SelectGroup selectGroup_)
    {
        m_interactingBase.group = selectGroup_;
        m_groupButtonsHolder.SetActive(false);

        foreach (SelectGroup groupButton in m_unitGroupButtons)
        {
            groupButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        FindObjectOfType<PauseManager>().Unpause();
        m_interactingBase.InstantSpawnUnits(unitInfo, NetworkManager.Singleton.LocalClientId);
        //m_optionsContent.SetActive(true);

        //for (int i = 0; i < m_optionButtons.Count; i++)
        //{
        //    m_optionButtons[i].SetOptionsData(unitInfo, unitGenerationBase);
        //    m_optionButtons[i].gameObject.SetActive(true);
        //}
    }
    
    private void OnOptionsSelected(BaseUnitOptionsObject optionsObject)
    {
        m_optionsContent.SetActive(false);
        FindObjectOfType<PauseManager>().Unpause();
    }

    private void SetUnitButtons(BaseUnitGeneration unitGenerationBase)
    {
        List<EUnitTypes> possibleUnitTypes = unitGenerationBase.spawnableUnits;
        if (possibleUnitTypes.Count > m_buttons.Count)
        {
            Debug.LogError("There are more defences to display on the build defences UI than there is build buttons, either remove some buildable defences or add more build buttons.");
        }

        for (int i = 0; i < m_buttons.Count; i++)
        {
            if (i < possibleUnitTypes.Count)
            {
                AIUnitTypesData.UnitTypeInfo info = m_unitData.GetUnitInfo(possibleUnitTypes[i]);
                m_buttons[i].SetUnitData(info, m_interactingBase);
                m_buttons[i].gameObject.SetActive(true);
            }
            else
            {
                m_buttons[i].gameObject.SetActive(false);
            }
        }
    }

}

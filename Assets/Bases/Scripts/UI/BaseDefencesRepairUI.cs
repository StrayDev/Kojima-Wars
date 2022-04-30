using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BaseDefencesRepairUI : MonoBehaviour
{
    [Tooltip("The gameobject that holds all the build UI, to be enabled/disabled on interaction")]
    [SerializeField] GameObject m_uiHolder = default;

    [SerializeField] private TextMeshProUGUI m_buyText = default;
    [SerializeField] private Image m_keyIconImage = default;
    [SerializeField] private Image m_costBackgroundImage = default;

    [SerializeField] private Key m_keyPress = Key.None;

    [SerializeField] private List<Image> images = new List<Image>();
    [SerializeField] private BaseController controller;

    [Header("Color References")]
    [SerializeField] private Color m_defaultColour = Color.black;
    [SerializeField] private Color m_playerColour = Color.white;
    [SerializeField] private Color m_cannotAffordColour = Color.red;
    [SerializeField] private Color m_teamRed;
    [SerializeField] private Color m_teamBlue;


    private BaseDefenceLocation m_baseDefenceLocation = default;
    
    private void Awake()
    {
        m_uiHolder.SetActive(false);
    }

    private void OnEnable()
    {
        BaseDefenceLocation.OnInteracted += OnInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange += OnLocalPlayerExitedDefence;
    }

    private void Update()
    {
        if (controller != null)
        {
            if (controller.TeamOwner == "red")
            {
                m_playerColour = m_teamRed;
            }
            else
            {
                m_playerColour = m_teamBlue;
            }
        }

        foreach (var image in images)
        {
            image.color = m_playerColour;
        }

        if (m_uiHolder.activeSelf && Keyboard.current[m_keyPress].wasPressedThisFrame)
        {
            Repair();
        }

        if (m_uiHolder.activeSelf && m_baseDefenceLocation != null)
        {
            UpdateDisplay(m_baseDefenceLocation.PlayerInRange.Currency >= m_baseDefenceLocation.RepairCost);
            
        }
    }

    private void OnDisable()
    {
        BaseDefenceLocation.OnInteracted -= OnInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange -= OnLocalPlayerExitedDefence;
    }

    private void Repair()
    {
        if (m_baseDefenceLocation.PlayerInRange != null)
        {
            if (m_baseDefenceLocation.PlayerInRange.Currency >= m_baseDefenceLocation.RepairCost)
            {
                m_baseDefenceLocation.PlayerInRange.Currency -= m_baseDefenceLocation.RepairCost;
                m_baseDefenceLocation.RepairDefenceObject();
                m_uiHolder.SetActive(false);
            }
        }
    }

    private void OnInteracted(BaseDefenceLocation defenceLocation)
    {
        if (defenceLocation.CurrentState == EBaseDefenceState.DAMAGED)
        {
            FindObjectOfType<PauseManager>().Pause(false);
            defenceLocation.OnDefenceDestroyed += OnDefenceDestroyed;
            m_baseDefenceLocation = defenceLocation;
            m_buyText.text = defenceLocation.RepairCost.ToString();
            m_uiHolder.SetActive(!m_uiHolder.activeSelf);

            controller = m_baseDefenceLocation.BaseController;
        }
    }

    private void OnLocalPlayerExitedDefence(Entity player, BaseDefenceLocation defenceLocation)
    {
        if (m_baseDefenceLocation != null)
        {
            m_baseDefenceLocation.OnDefenceDestroyed -= OnDefenceDestroyed;
            m_baseDefenceLocation = null;
        }

        m_uiHolder.SetActive(false);
    }

    private void UpdateDisplay(bool canAfford)
    {
        //m_keyIconImage.enabled = canAfford;
        //m_costBackgroundImage.color = canAfford ? m_defaultColour : m_cannotAffordColour;
        m_buyText.text = m_baseDefenceLocation.RepairCost.ToString();
    }

    private void OnDefenceDestroyed()
    {
        m_uiHolder.SetActive(false);
    }
}
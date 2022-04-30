using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseDefenceBuildObject : MonoBehaviour
{
    public static event Action<EBaseDefenceTypes> OnBuildDefence;

    public EBaseDefenceTypes DefenceType => m_defenceInfo.DefencesType;

    [SerializeField] private GameTeamData m_gameTeamData = default;

    [Header("Display References")]
    [SerializeField] private Button m_selectButton = default;
    [SerializeField] private Image m_defenceIcon = default;
    [SerializeField] private Image m_iconBackgroundImage = default;
    [SerializeField] private TextMeshProUGUI m_costText = default;
    [SerializeField] private TextMeshProUGUI m_nameText = default;
    [SerializeField] private TextMeshProUGUI m_descriptionText = default;
    
    private DefenceTypesData.DefenceTypeInfo m_defenceInfo;
    private BaseDefenceLocation m_baseDefenceLocation = null;

    public void EnableButton(bool enable = true)
    {
        gameObject.SetActive(enable);
    }

    public void SetData(BaseDefenceLocation defenceLocation, DefenceTypesData.DefenceTypeInfo defenceInfo)
    {
        m_baseDefenceLocation = defenceLocation;
        m_defenceInfo = defenceInfo;

        m_defenceIcon.sprite = defenceInfo.Icon;
        m_nameText.text = defenceInfo.Name;
        m_descriptionText.text = defenceInfo.Description;
        m_costText.text = defenceInfo.Cost.ToString();
    }

    private void OnEnable()
    {
        m_selectButton.onClick.AddListener(SelectPressed);
    }

    private void Update()
    {
        if (m_baseDefenceLocation != null)
        {
            m_iconBackgroundImage.color = m_gameTeamData.GetTeamData(m_baseDefenceLocation.BaseController.TeamOwner).Colour;
            m_selectButton.interactable = m_baseDefenceLocation.PlayerInRange.Currency >= m_defenceInfo.Cost;
        }
    }

    private void OnDisable()
    {
        m_selectButton.onClick.RemoveListener(SelectPressed);
    }

    private void SelectPressed()
    {
        OnBuildDefence?.Invoke(m_defenceInfo.DefencesType);
    }
}
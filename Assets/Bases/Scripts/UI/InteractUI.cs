using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject m_gameObjectHolder = default;

    [SerializeField] private TextMeshProUGUI m_detailsText = default;

    private void Awake()
    {
        m_gameObjectHolder.SetActive(false);
    }

    private void OnEnable()
    {
        BaseDefenceLocation.OnLocalPlayerEnteredRange += OnEnteredBaseLocation;
        BaseDefenceLocation.OnInteracted += OnDefenceLocationInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange += OnExitedBaseLocation;
        BaseUnitGeneration.OnLocalPlayerEnteredRange += OnEnteredUnitGeneration;
        BaseUnitGeneration.OnInteracted += OnUnitGenerationInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange += OnExitedUnitGeneration;
    }

    private void OnDisable()
    {
        BaseDefenceLocation.OnLocalPlayerEnteredRange -= OnEnteredBaseLocation;
        BaseDefenceLocation.OnInteracted -= OnDefenceLocationInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange -= OnExitedBaseLocation;
        BaseUnitGeneration.OnLocalPlayerEnteredRange -= OnEnteredUnitGeneration;
        BaseUnitGeneration.OnInteracted -= OnUnitGenerationInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange -= OnExitedUnitGeneration;
    }

    private void OnEnteredBaseLocation(Entity player, BaseDefenceLocation location)
    {
        if (player.TeamName != location.BaseController.TeamOwner)
        {
            return;
        }

        if (location.CurrentState == EBaseDefenceState.EMPTY)
        {
            m_detailsText.text = "Build Defence";
            m_gameObjectHolder.SetActive(true);
        }
        else if (location.CurrentState == EBaseDefenceState.DAMAGED)
        {
            m_detailsText.text = "Repair Defence";
            m_gameObjectHolder.SetActive(true);
        }
    }

    private void OnDefenceLocationInteracted(BaseDefenceLocation location)
    {
        m_gameObjectHolder.SetActive(false);
    }

    private void OnExitedBaseLocation(Entity player, BaseDefenceLocation location)
    {
        m_gameObjectHolder.SetActive(false);
    }

    private void OnEnteredUnitGeneration(Entity player, BaseUnitGeneration unitGeneration)
    {
        if (player.TeamName != unitGeneration.BaseController.TeamOwner)
        {
            return;
        }

        m_detailsText.text = "Generate Units";
        m_gameObjectHolder.SetActive(true);
    }

    private void OnUnitGenerationInteracted(BaseUnitGeneration unitGeneration)
    {
        m_gameObjectHolder.SetActive(false);
    }

    private void OnExitedUnitGeneration(Entity player, BaseUnitGeneration unitGeneration)
    {
        m_gameObjectHolder.SetActive(false);
    }
}
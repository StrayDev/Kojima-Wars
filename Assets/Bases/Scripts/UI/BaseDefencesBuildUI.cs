using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseDefencesBuildUI : NetworkBehaviour
{
    [Tooltip("The gameobject that holds all the build UI, to be enabled/disabled on interaction")]
    [SerializeField] private GameObject m_uiHolder = default;

    [SerializeField] private List<BaseDefenceBuildObject> m_buttons = new List<BaseDefenceBuildObject>();

    [SerializeField] private DefenceTypesData m_defencesInfo = default;
    
    private BaseDefenceLocation m_currentLocation;
    
    private void UpdateButtons(BaseDefenceLocation defenceLocation)
    {
        List<EBaseDefenceTypes> possibleDefenceTypes = defenceLocation.GetPossibleDefences();
        if (possibleDefenceTypes.Count > m_buttons.Count)
        {
            Debug.LogError("There are more defences to display on the build defences UI than there is build buttons, either remove some buildable defences or add more build buttons.");
        }

        for (int i = 0; i < m_buttons.Count; i++)
        {
            if (i < possibleDefenceTypes.Count)
            {
                DefenceTypesData.DefenceTypeInfo info = m_defencesInfo.GetDefenceInfo(possibleDefenceTypes[i]);
                m_buttons[i].SetData(defenceLocation, info);
                m_buttons[i].EnableButton();
            }
            else
            {
                m_buttons[i].EnableButton(false);
            }
        }
    }

    private void Awake()
    {
        m_uiHolder.SetActive(false);
    }

    private void OnEnable()
    {
        BaseDefenceLocation.OnInteracted += OnInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange += OnLocalPlayerExitedDefence;
        BaseDefenceBuildObject.OnBuildDefence += OnBuildDefence;
    }

    private void OnDisable()
    {
        BaseDefenceLocation.OnInteracted -= OnInteracted;
        BaseDefenceLocation.OnLocalPlayerExitedRange -= OnLocalPlayerExitedDefence;
        BaseDefenceBuildObject.OnBuildDefence -= OnBuildDefence;
    }

    private void OnInteracted(BaseDefenceLocation defenceLocation)
    {
        if (defenceLocation.CurrentState == EBaseDefenceState.EMPTY)
        {
            FindObjectOfType<PauseManager>().Pause(false);
            UpdateButtons(defenceLocation);
            m_uiHolder.SetActive(!m_uiHolder.activeSelf);
            m_currentLocation = defenceLocation;
        }
    }

    private void OnLocalPlayerExitedDefence(Entity player, BaseDefenceLocation defenceLocation)
    {
        m_uiHolder.SetActive(false);
    }

    private void OnBuildDefence(EBaseDefenceTypes defenceType)
    {
        if (m_currentLocation.PlayerInRange != null)
        {
            DefenceTypesData.DefenceTypeInfo defenceInfo = m_defencesInfo.GetDefenceInfo(defenceType);
            if (m_currentLocation.PlayerInRange.Currency >= defenceInfo.Cost)
            {
                m_currentLocation.PlayerInRange.DecreasePlayerCurrency(defenceInfo.Cost);

                var networkObject = new NetworkObjectReference(m_currentLocation.BaseController.gameObject);
                CreateNewDefenceServerRpc(defenceType, networkObject, m_currentLocation.BaseController.GetComponent<BaseDefencesController>().GetDefenceIndex(m_currentLocation));

                m_currentLocation = null;
                m_uiHolder.SetActive(false);
            }

            FindObjectOfType<PauseManager>().Unpause();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateNewDefenceServerRpc(EBaseDefenceTypes defenceType, NetworkObjectReference parent, int locationIndex)
    {
        parent.TryGet(out NetworkObject parentNetworkObject);
        if (parentNetworkObject == null)
        {
            Debug.LogError("Could not find network object for provided turret location. NetworkObjectReference: " + parent.ToString());
        }

        BaseDefence defence = m_defencesInfo.BuildDefenceOfType(defenceType);

        defence.GetComponent<NetworkObject>().Spawn();
        var defenceNetworkObject = new NetworkObjectReference(defence.gameObject);

        SetupDefenceClientRpc(parent, locationIndex, defenceNetworkObject);
    }

    [ClientRpc]
    private void SetupDefenceClientRpc(NetworkObjectReference baseDefenceControllerReference, int locationIndex, NetworkObjectReference defenceReference)
    {
        baseDefenceControllerReference.TryGet(out NetworkObject baseDefencesController);
        if (baseDefencesController == null)
        {
            Debug.LogError("Could not find network object for provided turret location. NetworkObjectReference: " + baseDefenceControllerReference.ToString());
        }

        defenceReference.TryGet(out NetworkObject defenceObject);
        if (defenceObject == null)
        {
            Debug.LogError("Could not find network object for provided turret location. NetworkObjectReference: " + defenceReference.ToString());
        }

        defenceObject.transform.SetParent(baseDefencesController.GetComponent<BaseDefencesController>().DefenceLocations[locationIndex].transform);
        defenceObject.transform.localPosition = Vector3.zero;
        defenceObject.transform.localScale = Vector3.one;
        defenceObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        baseDefencesController.GetComponent<BaseDefencesController>().DefenceLocations[locationIndex].DefenceBuilt(defenceObject.GetComponent<BaseDefence>());
    }
}
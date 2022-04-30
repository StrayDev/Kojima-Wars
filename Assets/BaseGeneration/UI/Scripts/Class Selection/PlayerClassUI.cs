using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassUI : MonoBehaviour
{
    
    //use this (need the actual scriptable object) to let the player know its confirmed class choice)
    public static event Action<int> OnClassAssigned = default;
    
    [Header("UI Containers")]
    [SerializeField] private List<GameObject> m_classContainers = new List<GameObject>();
    
    private PlayerClassSO m_selectedScriptable = default;

    private void Start()
    {
        // set assault as our default class
        SwitchClassContainer(0);

        // Register on spawn callback
        Core.NetworkPlayerSetup nps = FindObjectOfType<Core.NetworkPlayerSetup>();
        nps.OnSpawnEvents += OnSpawn;

        InputManager.SetInputType(ControlType.NONE);
        CursorManager.EnableCursor("class-ui");
    }

    public void SwitchClassContainer(int i)
    {
        // hide all of our containers before setting 1 to visible
        HideClassContainers();
        
        // set the selected scriptable object dependent on the selection
        
        switch (i)
        {
            // assault
            case 0:
                m_classContainers[0].SetActive(true);
                m_selectedScriptable = m_classContainers[0].GetComponent<ClassSelectionUI>().GetClassScriptable();
                break;
            
            // defence
            case 1:
                m_classContainers[1].SetActive(true);
                m_selectedScriptable = m_classContainers[1].GetComponent<ClassSelectionUI>().GetClassScriptable();
                break;
            
            // movement
            case 2:
                m_classContainers[2].SetActive(true);
                m_selectedScriptable = m_classContainers[2].GetComponent<ClassSelectionUI>().GetClassScriptable();
                break;
            
            // recon
            case 3:
                m_classContainers[3].SetActive(true);
                m_selectedScriptable = m_classContainers[3].GetComponent<ClassSelectionUI>().GetClassScriptable();
                break;
        }
    }

    private void HideClassContainers()
    {
        foreach (var classContainer in m_classContainers)
        {
            classContainer.SetActive(false);
        }
    }

    public void ShowUI(bool show)
    {
        // hide this game object??
        this.gameObject.SetActive(show);
    }

    private void OnSpawn(ulong id)
    {
        InputManager.SetInputType(ControlType.NONE);
        CursorManager.EnableCursor("class-ui");
        ShowUI(true);
    }

    public void ClassConfirmed()
    {
        ShowUI(false);

        // send an event to whoever is subscribed?
        // pass through the scriptable object of the type that we have confirmed.
        OnClassAssigned?.Invoke(m_selectedScriptable.GetUniqueID());

        // Set the input back to mech. Assuming the player is always a mech when they are spawned
        InputManager.SetInputType(ControlType.MECH);
        CursorManager.DisableCursor("class-ui");
    }
}

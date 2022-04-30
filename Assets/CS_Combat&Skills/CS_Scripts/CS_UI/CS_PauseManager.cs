using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_PauseManager : MonoBehaviour
{
    public static event Action<CS_UseAbilities> OnDisplaySkillTree;
    public static event Action<CS_UseAbilities> OnHideSkillTree; 
    
    private CS_UseAbilities m_abilities;
    private bool m_isPaused = false;
    private bool m_doOnce = false;
    
    private void Update()
    {
        if (!m_doOnce)
        {
            // try and find the skill tree
            m_abilities = GetComponent<CS_UseAbilities>();
            if (m_abilities)
            {
                OnHideSkillTree?.Invoke(m_abilities);
            }

            m_doOnce = true;
        }
       
    }

    ///TODO remove isPaused and use the UI function again once the mech state is not constantly being applied 
    private void PauseGame(InputAction.CallbackContext context)
    {
        if (!m_isPaused)
        {
             CursorManager.EnableCursor("cs-pause-ui");
            
            OnDisplaySkillTree?.Invoke(m_abilities);
        }
        else
        {
             CursorManager.DisableCursor("cs-pause-ui");
            
            OnHideSkillTree?.Invoke(m_abilities);
        }
    }
}

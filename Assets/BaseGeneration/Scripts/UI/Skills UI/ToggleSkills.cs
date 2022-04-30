using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSkills : MonoBehaviour
{
    public static event Action<CS_UseAbilities> OnDisplaySkillTree;
    public static event Action<CS_UseAbilities> OnHideSkillTree; 
    
    public CS_UseAbilities m_abilities;


    void Start()
    {
        m_abilities = GetComponent<CS_UseAbilities>();
        CursorManager.EnableCursor("Skill-Ui");
    }
    

    public void ToggleOn()
    {
         OnDisplaySkillTree?.Invoke(m_abilities);
    }

    public void ToggleOff()
    {
        OnHideSkillTree?.Invoke(m_abilities);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

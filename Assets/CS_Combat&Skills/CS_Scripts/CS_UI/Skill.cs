/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static SkillTree;

public class Skill : MonoBehaviour
{
    public Image backgroundImage;

    public GameObject[] connectedSkills;
    public bool isActive = false;
    public bool isChosen = false;

    public int skillTier = 0;

    public CS_DamageAbilities damageAbility;
    public CS_MovementAbilities movementAbility;
    public CS_DefenceAbilities defenceAbility;
    public CS_ReconAbilities reconAbility;

    public string title;
    public string shortDesc;

    private void Awake()
    {
        //skillTree.skillList.Add(this.gameObject);
    }

    public void Deactivate()
    {
        DisableSkill();
        skillTree.UpdateUI();
    }

    public void SelectSkill()
    {
        if (isActive && !isChosen && skillTree.skillPoints >= 0)
        {
            if (damageAbility != null && skillTree.currentPlayerClass != CS_UseAbilities.Class.Damage)
            {
                skillTree.currentPlayerClass = CS_UseAbilities.Class.Damage;
                skillTree.ClearSelections();
                skillTree.AssignAbility(damageAbility);
                skillTree.classText.text = "Damage Class";
            }
            else if (movementAbility != null && skillTree.currentPlayerClass != CS_UseAbilities.Class.Movement)
            {
                skillTree.ClearSelections();
                skillTree.currentPlayerClass = CS_UseAbilities.Class.Movement;
                skillTree.AssignAbility(movementAbility);
            skillTree.classText.text = "Movement Class";
            }
            else if (defenceAbility != null && skillTree.currentPlayerClass != CS_UseAbilities.Class.Defence)
            {
                skillTree.ClearSelections();
                skillTree.currentPlayerClass = CS_UseAbilities.Class.Defence;
                skillTree.AssignAbility(defenceAbility);
                skillTree.classText.text = "Defence Class";
            }
            else if (reconAbility != null && skillTree.currentPlayerClass != CS_UseAbilities.Class.Recon)
            {
                skillTree.ClearSelections();
                skillTree.currentPlayerClass = CS_UseAbilities.Class.Recon;
                skillTree.AssignAbility(reconAbility);
                skillTree.classText.text = "Recon Class";
            }
            else if (skillTree.skillPoints > 0)
            {
                Assign();
            }
            if (skillTree.skillPoints > 0)
            {
                isChosen = true;
                skillTree.skillPoints--;
                skillTree.AddToPlayer(backgroundImage.sprite, title, shortDesc);
                foreach (GameObject skill in connectedSkills)
                {
                    skill.GetComponent<Skill>().isActive = true;
                }
            }
            
        }
        else if (isActive && isChosen)
        {
            DisableSkill();
        }
        else if (!isActive)
        {
            return;
        }

        skillTree.UpdateUI();
    }

    private void DisableSkill()
    {
        if (isChosen)
        {
            isChosen = false;
            skillTree.skillPoints++;
            skillTree.RemoveFromPlayer(backgroundImage.sprite);
            Remove(damageAbility, movementAbility, defenceAbility, reconAbility);
        }
        
        foreach (GameObject skill in connectedSkills)
        {
            if (skill.GetComponent<Skill>().isChosen)
            {
                skillTree.skillPoints++;
            }
            skill.GetComponent<Skill>().isActive = false;
            skill.GetComponent<Skill>().isChosen = false;
            skillTree.RemoveFromPlayer(skill.GetComponent<Skill>().backgroundImage.sprite);
            Remove(skill.GetComponent<Skill>().damageAbility, skill.GetComponent<Skill>().movementAbility, skill.GetComponent<Skill>().defenceAbility, skill.GetComponent<Skill>().reconAbility);

            foreach (GameObject skill2 in skill.GetComponent<Skill>().connectedSkills)
            {
                if (skill2.GetComponent<Skill>().isChosen)
                {
                    skillTree.skillPoints++;
                }
                skill2.GetComponent<Skill>().isActive = false;
                skill2.GetComponent<Skill>().isChosen = false;
                skillTree.RemoveFromPlayer(skill2.GetComponent<Skill>().backgroundImage.sprite);
                Remove(skill2.GetComponent<Skill>().damageAbility, skill2.GetComponent<Skill>().movementAbility, skill2.GetComponent<Skill>().defenceAbility, skill2.GetComponent<Skill>().reconAbility);
            }
        }
        skillTree.UpdateUI();
    }

    private void Assign()
    {
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Damage) skillTree.AssignAbility(damageAbility);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Movement) skillTree.AssignAbility(movementAbility);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Defence) skillTree.AssignAbility(defenceAbility);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Recon) skillTree.AssignAbility(reconAbility);
    }
    private void Remove(CS_DamageAbilities dmg, CS_MovementAbilities mvm, CS_DefenceAbilities def, CS_ReconAbilities rec)
    {
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Damage) skillTree.RemoveAbility(dmg);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Movement) skillTree.RemoveAbility(mvm);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Defence) skillTree.RemoveAbility(def);
        if (skillTree.currentPlayerClass == CS_UseAbilities.Class.Recon) skillTree.RemoveAbility(rec);

        skillTree.ReorganiseSkills();
    }
}
*/
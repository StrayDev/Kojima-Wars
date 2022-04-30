/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class SkillTree : MonoBehaviour
{
    public static bool EnableDeathOnClose = false;
    public static SkillTree skillTree;
    
    // hacky but will do for now
    public HealthBars abilityUIManager;
    public List<GameObject> skillList;
    public List<GameObject> passiveList;
    public int skillPoints = 3;

    public Image skillOne;
    public TMP_Text skillOneTitle;
    public TMP_Text skillOneDesc;
    public Image skillTwo;
    public TMP_Text skillTwoTitle;
    public TMP_Text skillTwoDesc;
    public Image skillThree;
    public TMP_Text skillThreeTitle;
    public TMP_Text skillThreeDesc;
    public Image passiveSkill;
    public TMP_Text passiveDesc;

    public TMP_Text classText;
    public List<Image> playerSkills;

    public GameObject assaultSkills;
    public GameObject movementSkills;
    public GameObject defenceSkills;
    public GameObject reconSkills;
    public List<GameObject> skillMenus;

    public Sprite passivePlaceholder;

    public List<CS_DamageAbilities> damageAbilities;
    public List<CS_DamageAbilities> movementAbilities;
    public List<CS_DamageAbilities> defenceAbilities;
    public List<CS_DamageAbilities> reconAbilities;
    
    // i've set this to public for now till theres is a better way of communicated with this "Healthbars" //
    public CS_UseAbilities abilitiesScript;
    private CS_UseAbilities.Class playerClass;
    public CS_UseAbilities.Class currentPlayerClass;

    public int maxTier = 0;

    public bool pressedOnce = false;
    
    private void OnHideSkillTree(CS_UseAbilities obj)
    {
        CursorManager.DisableCursor("Skill-Ui");
        abilitiesScript = null;
        abilityUIManager.abilities = null;
    }

    private void OnDisplaySkillTree(CS_UseAbilities obj)
    {
        abilitiesScript = obj;
        abilityUIManager.abilities = obj;
        CursorManager.EnableCursor("Skill-Ui", true);
    }
    
    private void Awake()
    {
        skillList = new List<GameObject>();
        skillTree = this;

        ToggleSkills.OnDisplaySkillTree += OnDisplaySkillTree;
        ToggleSkills.OnHideSkillTree += OnHideSkillTree;

        skillMenus.Add(assaultSkills);
        skillMenus.Add(movementSkills);
        skillMenus.Add(defenceSkills);
        skillMenus.Add(reconSkills);

        foreach (GameObject menu in skillMenus)
        {
            menu.SetActive(false);
        }
        assaultSkills.SetActive(true);

        UpdateUI();
    }

    public void SkillButton()
    {
        gameObject.SetActive(false);

        if (EnableDeathOnClose)
        {
            EnableDeathOnClose = false;
            FindObjectOfType<DeathUI>().EnableDisplay();
        }
        
        if (!pressedOnce)
        {
            CursorManager.DisableCursor("Skill-Ui", true);
            pressedOnce = true;
        }
        else
        {
            CursorManager.EnableCursor("Skill-Ui", true);
        }
    }

    private void Update()
    {
        
        if (abilitiesScript != null)
        {
            if (currentPlayerClass == CS_UseAbilities.Class.Damage) abilitiesScript.playerClass = CS_UseAbilities.Class.Damage;
            if (currentPlayerClass == CS_UseAbilities.Class.Movement) abilitiesScript.playerClass = CS_UseAbilities.Class.Movement;
            if (currentPlayerClass == CS_UseAbilities.Class.Defence) abilitiesScript.playerClass = CS_UseAbilities.Class.Defence;
            if (currentPlayerClass == CS_UseAbilities.Class.Recon) abilitiesScript.playerClass = CS_UseAbilities.Class.Recon;
        }
        else
        {
            foreach (var player in FindObjectsOfType<MechCharacterController>())
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                if (networkObject.OwnerClientId == networkObject.NetworkManager.LocalClient.ClientId)
                {
                    abilitiesScript = player.GetComponent<CS_UseAbilities>();
                }
            }
        }
    }

    public void UpdateUI()
    {
        maxTier = 1;
        foreach (GameObject skill in skillList)
        {
            if (playerClass == CS_UseAbilities.Class.Damage)
            {
                if (skill.GetComponent<Skill>().damageAbility != null)
                {
                    if (skill.GetComponent<Skill>().isChosen && skill.GetComponent<Skill>().skillTier > maxTier)
                    {
                        maxTier = skill.GetComponent<Skill>().skillTier;
                    }
                }
            }
            if (playerClass == CS_UseAbilities.Class.Movement)
            {
                if (skill.GetComponent<Skill>().movementAbility != null)
                {
                    if (skill.GetComponent<Skill>().isChosen && skill.GetComponent<Skill>().skillTier > maxTier)
                    {
                        maxTier = skill.GetComponent<Skill>().skillTier;
                    }
                }
            }
            if (playerClass == CS_UseAbilities.Class.Defence)
            {
                if (skill.GetComponent<Skill>().defenceAbility != null)
                {
                    if (skill.GetComponent<Skill>().isChosen && skill.GetComponent<Skill>().skillTier > maxTier)
                    {
                        maxTier = skill.GetComponent<Skill>().skillTier;
                    }
                }
            }
            if (playerClass == CS_UseAbilities.Class.Recon)
            {
                if (skill.GetComponent<Skill>().reconAbility != null)
                {
                    if (skill.GetComponent<Skill>().isChosen && skill.GetComponent<Skill>().skillTier > maxTier)
                    {
                        maxTier = skill.GetComponent<Skill>().skillTier;
                    }
                }
            }



            if (skill.GetComponent<Skill>().isChosen)
            {
                skill.GetComponent<Skill>().backgroundImage.color = Color.yellow;
                Color temp = skill.GetComponent<Skill>().backgroundImage.color;
                temp.a = 1f;
                skill.GetComponent<Skill>().backgroundImage.color = temp;
            }
            else if (skill.GetComponent<Skill>().isActive)
            {
                skill.GetComponent<Skill>().backgroundImage.color = Color.white;
                Color temp = skill.GetComponent<Skill>().backgroundImage.color;
                temp.a = 1f;
                skill.GetComponent<Skill>().backgroundImage.color = temp;
            }
            else
            {
                skill.GetComponent<Skill>().backgroundImage.color = Color.white;
                Color temp = skill.GetComponent<Skill>().backgroundImage.color;
                temp.a = 0.25f;
                skill.GetComponent<Skill>().backgroundImage.color = temp;
            }    

            foreach (Image img in playerSkills)
            {
                if (img.sprite == null)
                {
                    Color temp = img.color;
                    temp.a = 0f;
                    img.color = temp;
                }
                else
                {
                    Color temp = img.color;
                    temp.a = 1f;
                    img.color = temp;
                }
            }
        }
        
        /*
        if (maxTier == 1)
        {
            GetPassiveByTier(1).isActive = true;
            GetPassiveByTier(2).isActive = false;
            GetPassiveByTier(3).isActive = false;
        }
        else if (maxTier == 2)
        {
            GetPassiveByTier(1).isActive = true;
            GetPassiveByTier(2).isActive = true;
            GetPassiveByTier(3).isActive = false;
        }
        else if (maxTier == 3)
        {
            GetPassiveByTier(1).isActive = true;
            GetPassiveByTier(2).isActive = true;
            GetPassiveByTier(3).isActive = true;
        }

        foreach (GameObject passive in passiveList)
        {
            if (passive.GetComponent<Passive>().isChosen)
            {
                passive.GetComponent<Passive>().backgroundImage.color = Color.yellow;
                Color temp = passive.GetComponent<Passive>().backgroundImage.color;
                temp.a = 1f;
                passive.GetComponent<Passive>().backgroundImage.color = temp;
            }
            else if (passive.GetComponent<Passive>().isActive)
            {
                passive.GetComponent<Passive>().backgroundImage.color = Color.white;
                Color temp = passive.GetComponent<Passive>().backgroundImage.color;
                temp.a = 1f;
                passive.GetComponent<Passive>().backgroundImage.color = temp;
            }
            else
            {
                passive.GetComponent<Passive>().backgroundImage.color = Color.white;
                Color temp = passive.GetComponent<Passive>().backgroundImage.color;
                temp.a = 0.25f;
                passive.GetComponent<Passive>().backgroundImage.color = temp;
            }

            if (passive.GetComponent<Passive>().isChosen && passive.GetComponent<Passive>().skillLevel > maxTier)
            {
                passive.GetComponent<Passive>().isChosen = false;
                RemoveFromPlayerPassive();
            }
        }
        
    }

    private Passive GetPassiveByTier(int tier)
    {
        Passive temp = null;
        foreach (GameObject passive in passiveList)
        {
            if (passive.GetComponent<Passive>().skillLevel == tier && passive.activeInHierarchy)
            {
                temp = passive.GetComponent<Passive>();
            }
        }
        return temp;
    }

    public void AddToPlayer(Sprite sprite, string title, string desc)
    {
        foreach (Image img in playerSkills)
        {
            if (!img.sprite)
            {
                img.sprite = sprite;
                Color temp = img.color;
                temp.a = 1f;
                img.color = temp;
                img.preserveAspect = true;

                img.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = title;
                img.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = desc;

                return;
            }
        }
    }

    public void RemoveFromPlayer(Sprite sprite)
    {
        foreach (Image img in playerSkills)
        {
            if (img.sprite == sprite)
            {
                img.sprite = null;
                Color temp = img.color;
                temp.a = 0.5f;
                img.color = temp;

                img.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                img.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
        }
    }    

    public void AddToPlayerPassive(Sprite sprite, string desc)
    {
        passiveSkill.sprite = sprite;
        passiveDesc.text = desc;
    }

    public void RemoveFromPlayerPassive()
    {
        passiveSkill.sprite = passivePlaceholder;
        passiveDesc.text = "";
    }

    public void SelectMenu(GameObject menu)
    {
        if (menu == assaultSkills) playerClass = CS_UseAbilities.Class.Damage;
        if (menu == movementSkills) playerClass = CS_UseAbilities.Class.Movement;
        if (menu == defenceSkills) playerClass = CS_UseAbilities.Class.Defence;
        if (menu == reconSkills) playerClass = CS_UseAbilities.Class.Recon;

        if (menu.gameObject.activeInHierarchy)
        {
            return;
        }

        foreach (GameObject classmenu in skillMenus)
        {
            if (classmenu == menu)
            {
                classmenu.SetActive(true);
            }
            else
            {
                
                classmenu.SetActive(false);
            }
        }
        UpdateUI();
    }

    public void ClearSelections()
    {
        foreach (GameObject classmenu in skillMenus)
        {
            foreach (Transform child in classmenu.transform)
            {
                if (child.GetComponent<Skill>())
                {
                    child.GetComponent<Skill>().Deactivate();
                }
            }
        }
        foreach (GameObject passive in passiveList)
        {
            passive.GetComponent<Passive>().Deactivate();
        }
        foreach (Image img in playerSkills)
        {
            img.sprite = null;
        }
        RemoveFromPlayerPassive();
        ClearAbilities();
        UpdateUI();
    }

    public void ReorganiseSkills()
    {
        if (currentPlayerClass == CS_UseAbilities.Class.Damage)
        {
            if (abilitiesScript.damageAbilityOne == null)
            {
                if (abilitiesScript.damageAbilityTwo != null)
                {
                    abilitiesScript.damageAbilityOne = abilitiesScript.damageAbilityTwo;
                    abilitiesScript.damageAbilityTwo = null;
                    skillOne.sprite = skillTwo.sprite;
                    skillTwo.sprite = null;

                    Color temp = skillTwo.color;
                    temp.a = 0.5f;
                    skillTwo.color = temp;

                    skillOne.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillOne.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }
                
            }
            if (abilitiesScript.damageAbilityTwo == null)
            {
                if (abilitiesScript.damageAbilityThree != null)
                {
                    abilitiesScript.damageAbilityTwo = abilitiesScript.damageAbilityThree;
                    abilitiesScript.damageAbilityThree = null;
                    skillTwo.sprite = skillThree.sprite;
                    skillThree.sprite = null;

                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }
            }
        }
        //Do movement one
        if (currentPlayerClass == CS_UseAbilities.Class.Defence)
        {
            if (abilitiesScript.defenceAbilityOne == null)
            {
                if (abilitiesScript.defenceAbilityTwo != null)
                {
                    abilitiesScript.defenceAbilityOne = abilitiesScript.defenceAbilityTwo;
                    abilitiesScript.defenceAbilityTwo = null;
                    skillOne.sprite = skillTwo.sprite;
                    skillTwo.sprite = null;

                    skillOne.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillOne.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }

            }
            if (abilitiesScript.defenceAbilityTwo == null)
            {
                if (abilitiesScript.defenceAbilityThree != null)
                {
                    abilitiesScript.defenceAbilityTwo = abilitiesScript.defenceAbilityThree;
                    abilitiesScript.defenceAbilityThree = null;
                    skillTwo.sprite = skillThree.sprite;
                    skillThree.sprite = null;

                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }
            }
        }

        if (currentPlayerClass == CS_UseAbilities.Class.Movement)
        {
            if (abilitiesScript.movementAbilityOne == null)
            {
                if (abilitiesScript.movementAbilityTwo != null)
                {
                    abilitiesScript.movementAbilityOne = abilitiesScript.movementAbilityTwo;
                    abilitiesScript.movementAbilityTwo = null;
                    skillOne.sprite = skillTwo.sprite;
                    skillTwo.sprite = null;

                    skillOne.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillOne.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }

            }
            if (abilitiesScript.movementAbilityTwo == null)
            {
                if (abilitiesScript.movementAbilityThree != null)
                {
                    abilitiesScript.movementAbilityTwo = abilitiesScript.movementAbilityThree;
                    abilitiesScript.movementAbilityThree = null;
                    skillTwo.sprite = skillThree.sprite;
                    skillThree.sprite = null;

                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }
            }
        }
        if (currentPlayerClass == CS_UseAbilities.Class.Recon)
        {
            if (abilitiesScript.reconAbilityOne == null)
            {
                if (abilitiesScript.reconAbilityTwo != null)
                {
                    abilitiesScript.reconAbilityOne = abilitiesScript.reconAbilityTwo;
                    abilitiesScript.reconAbilityTwo = null;
                    skillOne.sprite = skillTwo.sprite;
                    skillTwo.sprite = null;

                    skillOne.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillOne.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }

            }
            if (abilitiesScript.reconAbilityTwo == null)
            {
                if (abilitiesScript.reconAbilityThree != null)
                {
                    abilitiesScript.reconAbilityTwo = abilitiesScript.reconAbilityThree;
                    abilitiesScript.reconAbilityThree = null;
                    skillTwo.sprite = skillThree.sprite;
                    skillThree.sprite = null;

                    skillTwo.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    skillTwo.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text;
                    skillThree.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                    skillThree.gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                }
            }
        }
    }

    public void ClearAbilities()
    {
        abilitiesScript.damageAbilityOne = null;
        abilitiesScript.damageAbilityTwo = null;
        abilitiesScript.damageAbilityThree = null;
        abilitiesScript.movementAbilityOne = null;
        abilitiesScript.movementAbilityTwo = null;
        abilitiesScript.movementAbilityThree = null;
        abilitiesScript.defenceAbilityOne = null;
        abilitiesScript.defenceAbilityTwo = null;
        abilitiesScript.defenceAbilityThree = null;
        abilitiesScript.reconAbilityOne = null;
        abilitiesScript.reconAbilityTwo = null;
        abilitiesScript.reconAbilityThree = null;
    }

    public void AssignAbility(CS_DamageAbilities ability)
    {
        if (abilitiesScript.damageAbilityOne == null) abilitiesScript.damageAbilityOne = ability;
        else if (abilitiesScript.damageAbilityTwo == null) abilitiesScript.damageAbilityTwo = ability;
        else if (abilitiesScript.damageAbilityThree == null) abilitiesScript.damageAbilityThree = ability;
    }
    public void RemoveAbility(CS_DamageAbilities ability)
    {
        if (abilitiesScript.damageAbilityOne == ability) abilitiesScript.damageAbilityOne = null;
        else if (abilitiesScript.damageAbilityTwo == ability) abilitiesScript.damageAbilityTwo = null;
        else if (abilitiesScript.damageAbilityThree == ability) abilitiesScript.damageAbilityThree = null;
    }

    public void AssignAbility(CS_MovementAbilities ability)
    {
        if (abilitiesScript.movementAbilityOne == null) abilitiesScript.movementAbilityOne = ability;
        else if (abilitiesScript.movementAbilityTwo == null) abilitiesScript.movementAbilityTwo = ability;
        else if (abilitiesScript.movementAbilityThree == null) abilitiesScript.movementAbilityThree = ability;
    }
    public void RemoveAbility(CS_MovementAbilities ability)
    {
        if (abilitiesScript.movementAbilityOne == ability) abilitiesScript.movementAbilityOne = null;
        else if (abilitiesScript.movementAbilityTwo == ability) abilitiesScript.movementAbilityTwo = null;
        else if (abilitiesScript.movementAbilityThree == ability) abilitiesScript.movementAbilityThree = null;
    }

    public void AssignAbility(CS_DefenceAbilities ability)
    {
        if (abilitiesScript.defenceAbilityOne == null) abilitiesScript.defenceAbilityOne = ability;
        else if (abilitiesScript.defenceAbilityTwo == null) abilitiesScript.defenceAbilityTwo = ability;
        else if (abilitiesScript.defenceAbilityThree == null) abilitiesScript.defenceAbilityThree = ability;
    }
    public void RemoveAbility(CS_DefenceAbilities ability)
    {
        if (abilitiesScript.defenceAbilityOne == ability) abilitiesScript.defenceAbilityOne = null;
        else if (abilitiesScript.defenceAbilityTwo == ability) abilitiesScript.defenceAbilityTwo = null;
        else if (abilitiesScript.defenceAbilityThree == ability) abilitiesScript.defenceAbilityThree = null;
    }

    public void AssignAbility(CS_ReconAbilities ability)
    {
        if (abilitiesScript.reconAbilityOne == null) abilitiesScript.reconAbilityOne = ability;
        else if (abilitiesScript.reconAbilityTwo == null) abilitiesScript.reconAbilityTwo = ability;
        else if (abilitiesScript.reconAbilityThree == null) abilitiesScript.reconAbilityThree = ability;
    }
    public void RemoveAbility(CS_ReconAbilities ability)
    {
        if (abilitiesScript.reconAbilityOne == ability) abilitiesScript.reconAbilityOne = null;
        else if (abilitiesScript.reconAbilityTwo == ability) abilitiesScript.reconAbilityTwo = null;
        else if (abilitiesScript.reconAbilityThree == ability) abilitiesScript.reconAbilityThree = null;
    }
}
*/
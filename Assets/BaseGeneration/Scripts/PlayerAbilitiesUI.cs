using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitiesUI : MonoBehaviour
{
    [Header("Ability Slots")]
    [SerializeField] private List<AbilitySlotUI> slots = new List<AbilitySlotUI>(3);

    private void OnEnable()
    {
        PlayerClassUI.OnClassAssigned += OnClassAssigned;
    }
    
    private void OnDisable()
    {
        PlayerClassUI.OnClassAssigned -= OnClassAssigned;
    }
    
    private void OnClassAssigned(int classID)
    {
        // just setting the background image for now (will need to pass in the ability so in future)
        // (for the cooldown of each ability)

        var so = PlayerClassSO.GetClassFromID(classID);

        for (var i = 0; i < 3; i++)
        {
            var slot = slots[i];
            var ability = so.GetAbility(i);
            
            slot.SetAbilityImage(ability.GetIconSprite());
            slot.SetIconColour(so.GetClassColour());
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class HealthBars : MonoBehaviour
{
    public AbilitySlotUI abilitySlotOne;
    public AbilitySlotUI abilitySlotTwo;
    public AbilitySlotUI abilitySlotThree;
    public AbilitySlotUI[] abilitySlots;

    /*
    public Slider shield;
    private GameObject health_obj;
    private GameObject shield_obj;
    public TMP_Text shield_text;
    public TMP_Text health_text;
    public Slider health;
    public Image a1;
    public Image a2;
    public Image a3;
    */

    //public GameObject player;
    //private CS_PlayerStats playerStats;
    public CS_UseAbilities abilities;

    private void Start()
    {
        abilitySlotOne.SetAbilityImage(null);
        abilitySlotTwo.SetAbilityImage(null);
        abilitySlotThree.SetAbilityImage(null);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameObject.name);
        if (abilities == null)
        {
            foreach (var player in FindObjectsOfType<MechCharacterController>())
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                if (networkObject.OwnerClientId == networkObject.NetworkManager.LocalClient.ClientId)
                {
                    abilities = player.GetComponent<CS_UseAbilities>();
                }
            }
        }
        if (abilities != null)
        {
            for (int i = 0; i < 3; i++)
            {
                AbilitySO ability = abilities.GetPlayerClass().GetAbility(i);
                abilitySlots[i].SetAbilityImage(ability.GetIconSprite());
            }
        }
    }
}
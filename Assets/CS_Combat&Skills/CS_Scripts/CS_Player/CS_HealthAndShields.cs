using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_HealthAndShields : MonoBehaviour/*, IDamageable*/
{
    /*[SerializeField] public float maxShields;
    [SerializeField] public float maxHealth;
    
    [SerializeField] public float currentShields;
    [SerializeField] public float currentHealth;*/
    
    [SerializeField] private float regenAmountPerSecond;
    public int healAmount;
    public bool regen;

    private CS_PlayerStats playerStats;

    [SerializeField] private TextMeshProUGUI ShieldsText;
    [SerializeField] private TextMeshProUGUI HealthText;

    private GameObject player;
    /*private void Awake()
    {
        playerStats = GetComponent<CS_PlayerStats>();

        playerStats.currentShields = playerStats.maxShields;
        playerStats.currentHealth = playerStats.maxHealth;
        ShieldsText.text = playerStats.currentShields.ToString();
        HealthText.text = playerStats.currentHealth.ToString();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //Rotate text towards player 
        if (tag.Equals("Enemy"))
        {
           ShieldsText.transform.parent.LookAt(player.transform.position);
           ShieldsText.transform.parent.Rotate(0,180,0);
        }
        regenerateHealth();
    }

    public void TakeDamage(float damage, string weaponname)
    {
        Debug.Log(gameObject.name + " Took Damage enemy: " + damage + " with: " + weaponname);


        float damageCarryOver = 0;
        float shieldsBeforeDamage = playerStats.currentShields;

        //Damage the shields
        if (playerStats.currentShields > 0 && shieldsBeforeDamage - damage >= 0)
        {
            playerStats.currentShields -= damage;
            ShieldsText.text = playerStats.currentShields.ToString();
        }
        //If the damage finishes the shield and needs to damage the health too
        else if (playerStats.currentShields > 0 && shieldsBeforeDamage - damage < 0)
        {
            playerStats.currentShields -= damage;
            damageCarryOver = playerStats.currentShields * -1;
            Debug.Log("carry over to health by: " + damageCarryOver);
            playerStats.currentHealth -= damageCarryOver;
            HealthText.text = Mathf.Round(playerStats.currentHealth).ToString();
        }
        //Damage the health
        else
        {
            playerStats.currentHealth -= damage;
            HealthText.text = Mathf.Round(playerStats.currentHealth).ToString();
        }
        
        
        if (playerStats.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else if (playerStats.currentShields <= 0)
        {
            //stop sheilds going below zero
            playerStats.currentShields = 0;
            ShieldsText.text = playerStats.currentShields.ToString();
        }
        
    }

    //Testing only
    public void activateRegen()
    {
        if (regen)
        {
            regen = false;
        }
        else
        {
            regen = true;
        }
    }

    public void regenerateHealth()
    {
        if (regen)
        {
            //Heal health first
            if (playerStats.currentHealth < playerStats.maxHealth && playerStats.currentShields <= 0)
            {
                playerStats.currentHealth += regenAmountPerSecond * Time.deltaTime;
                HealthText.text = Mathf.Round(playerStats.currentHealth).ToString();
            }
            //Replenish sheild if health is full
            else if (playerStats.currentShields < playerStats.maxShields && Mathf.Round(playerStats.currentHealth) == playerStats.maxHealth)
            {
                playerStats.currentShields += regenAmountPerSecond * Time.deltaTime;
                ShieldsText.text = Mathf.Round(playerStats.currentShields).ToString();
            }
        }
        
    }

    public void Heal(int healAmount)
    {
        
        float healCarryOver = 0;
        float healthBeforeHeal = playerStats.currentHealth;
        //Heal health first
        if (playerStats.currentHealth + healAmount < playerStats.maxHealth && playerStats.currentShields <= 0)
        {
            playerStats.currentHealth += healAmount;
            HealthText.text = Mathf.Round(playerStats.currentHealth).ToString(); 
        }
        else if (playerStats.currentHealth + healAmount >= playerStats.maxHealth && playerStats.currentShields <= 0)
        {
            healCarryOver = playerStats.maxHealth - playerStats.currentHealth;
            playerStats.currentHealth += healAmount;
            playerStats.currentHealth -= healCarryOver;
            playerStats.currentShields += healCarryOver;
            HealthText.text = Mathf.Round(playerStats.currentHealth).ToString(); 
            ShieldsText.text = Mathf.Round(playerStats.currentShields).ToString();

        }
        //Replenish sheild if health is full
        else if (playerStats.currentShields < playerStats.maxShields && Mathf.Round(playerStats.currentHealth) == playerStats.maxHealth)
        {
            playerStats.currentShields += healAmount;
            ShieldsText.text = Mathf.Round(playerStats.currentShields).ToString();
        }

        if (playerStats.currentShields > playerStats.maxShields)
        {
            playerStats.currentShields = playerStats.maxShields;
            ShieldsText.text = Mathf.Round(playerStats.currentShields).ToString();
        }
    }*/
}

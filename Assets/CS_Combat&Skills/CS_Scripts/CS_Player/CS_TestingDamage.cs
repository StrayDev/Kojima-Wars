using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_TestingDamage : MonoBehaviour, IDamageable
{
    [SerializeField] public float maxShields;
    [SerializeField] public float maxHealth;
    
    [SerializeField] public float currentShields;
    [SerializeField] public float currentHealth;
    
    [SerializeField] private float regenAmountPerSecond;
    public int healAmount;
    public bool regen;


    [SerializeField] private TextMeshProUGUI ShieldsText;
    [SerializeField] private TextMeshProUGUI HealthText;

    private GameObject player;
    private void Awake()
    {
        currentShields = maxShields;
        currentHealth = maxHealth;
        ShieldsText.text = currentShields.ToString();
        HealthText.text = currentHealth.ToString();

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

    public bool IsAlive()
    {
        return currentHealth > 0.0f;
    }

    public void TakeDamageServerRpc(int damage)
    {       
        float damageCarryOver = 0;
        float shieldsBeforeDamage = currentShields;

        //Damage the shields
        if (currentShields > 0 && shieldsBeforeDamage - damage >= 0)
        {
            currentShields -= damage;
            ShieldsText.text = currentShields.ToString();
        }
        //If the damage finishes the shield and needs to damage the health too
        else if (currentShields > 0 && shieldsBeforeDamage - damage < 0)
        {
            currentShields -= damage;
            damageCarryOver = currentShields * -1;
            Debug.Log("carry over to health by: " + damageCarryOver);
            currentHealth -= damageCarryOver;
            HealthText.text = Mathf.Round(currentHealth).ToString();
        }
        //Damage the health
        else
        {
            currentHealth -= damage;
            HealthText.text = Mathf.Round(currentHealth).ToString();
        }
        
        
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else if (currentShields <= 0)
        {
            //stop shields going below zero
            currentShields = 0;
            ShieldsText.text = currentShields.ToString();
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
            if (currentHealth < maxHealth && currentShields <= 0)
            {
                currentHealth += regenAmountPerSecond * Time.deltaTime;
                HealthText.text = Mathf.Round(currentHealth).ToString();
            }
            //Replenish sheild if health is full
            else if (currentShields < maxShields && Mathf.Round(currentHealth) == maxHealth)
            {
                currentShields += regenAmountPerSecond * Time.deltaTime;
                ShieldsText.text = Mathf.Round(currentShields).ToString();
            }
        }
    }

    public void HealDamageServerRpc(int healAmount)
    {
        float healCarryOver = 0;
        float healthBeforeHeal = currentHealth;
        //Heal health first
        if (currentHealth + healAmount < maxHealth && currentShields <= 0)
        {
            currentHealth += healAmount;
            HealthText.text = Mathf.Round(currentHealth).ToString(); 
        }
        else if (currentHealth + healAmount >= maxHealth && currentShields <= 0)
        {
            healCarryOver = maxHealth - currentHealth;
            currentHealth += healAmount;
            currentHealth -= healCarryOver;
            currentShields += healCarryOver;
            HealthText.text = Mathf.Round(currentHealth).ToString(); 
            ShieldsText.text = Mathf.Round(currentShields).ToString();

        }
        //Replenish sheild if health is full
        else if (currentShields < maxShields && Mathf.Round(currentHealth) == maxHealth)
        {
            currentShields += healAmount;
            ShieldsText.text = Mathf.Round(currentShields).ToString();
        }

        if (currentShields > maxShields)
        {
            currentShields = maxShields;
            ShieldsText.text = Mathf.Round(currentShields).ToString();
        }
    }
}

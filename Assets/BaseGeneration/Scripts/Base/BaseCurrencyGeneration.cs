using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCurrencyGeneration : MonoBehaviour
{
    public static Action<int, string> OnCurrencyUpdated;

    [SerializeField] private BaseController m_baseController = default;
    [SerializeField] private GenerationRates generationRate = default;

    private float timer = 0;
    private float perTickTimer = 0;
    private int cashPerTick = 20;
    
    private bool contested = false;
    private bool active = false;
    

    
    private void SetUpGenerationRates()
    {
        GenerationRates.GenerationValues rates = generationRate.GetCurrencyGenerationRatesOfRank(m_baseController.Rank);
        timer = rates.time;
        perTickTimer = rates.time;
        cashPerTick = rates.amount;
    }

    private void Start()
    {
        SetUpGenerationRates();
    }

    private void Update()
    {
        
        if (m_baseController.TeamOwner == "red" || m_baseController.TeamOwner == "blue")
        {
            active = true;
        }
        else
        {
            active = false;
        }


        if (active)
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                FindObjectOfType<CurrencyManager>().UpdatePlayerCurrencyServerRpc(cashPerTick, m_baseController.TeamOwner);
                timer = perTickTimer;
            }
        }
        
    }
}

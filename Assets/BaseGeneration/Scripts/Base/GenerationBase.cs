using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseRank
{
    LOW,
    MEDIUM,
    HIGH
}
public enum UpdateStages
{
    CONTESTED,
    COUNTDOWN,
    ADDREASOURCE
}
public interface IGeneration
{
    void SetTimer(float timer);
    void OnTickGoal();
    void SetActive(bool active);
    void OnTick();
    void SetContested(bool contested);
}

public class GenerationBase : MonoBehaviour, IGeneration
{
    private float  perTickTime = 10f;
    private float Timer;
    private bool Active;
    private bool Contested;
    private UpdateStages UpdateStages;
    private UpdateStages previousStage;
    
    
    protected void Awake()
    {
        UpdateStages = UpdateStages.COUNTDOWN;
        previousStage = UpdateStages;
        Active = true;
    }
    private void Update()
    {
        OnTick();
    }
    private void ContesedCheck()
    {
        if (Contested)
        {
            print("Contested");
            previousStage = UpdateStages;
            UpdateStages = UpdateStages.CONTESTED;
        }
    }

    public virtual void SetTimer(float timer)
    {
        perTickTime = timer;
    }
    public virtual void SetContested(bool contested)
    {
        Contested = contested;
    }
    public virtual void SetActive(bool active)
    {
        Active = active;
    }
    
    public virtual void OnTick()
    {
        if (Active)
        {
            switch (UpdateStages)
            {
                case UpdateStages.COUNTDOWN:
                {
                    ContesedCheck();
                    Timer -= Time.deltaTime;
                    if (Timer <= 0)
                    {
                        UpdateStages = UpdateStages.ADDREASOURCE;
                    }
                    break;
                }

                case UpdateStages.ADDREASOURCE:
                {
                    OnTickGoal();
                    Timer = perTickTime;
                    UpdateStages = UpdateStages.COUNTDOWN;
                    break;
                }

                case UpdateStages.CONTESTED:
                {
                    if (!Contested)
                    {
                        UpdateStages = previousStage;
                    }
                    break;
                }
            }
        }
    }
    public virtual void OnTickGoal()
    {
        
    }
    
    
}

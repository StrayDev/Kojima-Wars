using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action OnDeath;
    public event Action<float> OnDamaged;
    public event Action<float> OnHealed;

    public float MaxHealth => m_maxHealth;

    public float CurrentHealth { get; private set; } = 0;

    public bool IsDead => CurrentHealth <= 0;

    [SerializeField] private float m_maxHealth = 100;

    public void Damage(float amount)
    {
        CurrentHealth -= amount;

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke(amount);
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        OnHealed?.Invoke(amount);

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    private void Awake()
    {
        CurrentHealth = m_maxHealth;
    }
}
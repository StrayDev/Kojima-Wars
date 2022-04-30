using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using Core;

public class CombatComponent : NetworkBehaviour, IDamageable
{
    public int MaxHealthValue => MaxHealth;
    public int MaxShieldsValue => MaxShields;

    [SerializeField] private int MaxShields = 200;
    [SerializeField] private int MaxHealth = 100;

    [SerializeField] private UnityEvent OnSpawnEvent = default;
    [SerializeField] private UnityEvent OnDeathEvent = default;

    private const int MinValue = 0;

    public void HealDamage(int heal) => TakeDamage(-heal); // this might need to be updated for both health and shield
    public void TakeDamage(int damage) => TakeDamageServerRpc(damage);


    public void Start()
    {
        var nps = NetworkPlayerSetup.Get();
        nps.OnSpawnEvents += OnSpawn;

        if (!IsServer) return;

        HealthComponent.SetHealth(OwnerClientId, MaxHealth);
        ShieldComponent.SetShield(OwnerClientId, MaxShields);
    }

    /// <summary> 
    /// Called When the player is spawned
    /// </summary>
    public void OnSpawn(ulong id) => OnSpawnServerRpc(id);

    [ServerRpc(RequireOwnership = false)]
    public void OnSpawnServerRpc(ulong id)
    {
        OnSpawnClientRpc(id);

        HealthComponent.SetHealth(id, MaxHealth);
        ShieldComponent.SetShield(id, MaxShields);
    }

    [ClientRpc]
    public void OnSpawnClientRpc(ulong id)
    {
        OnSpawnEvent.Invoke();
    }

    /// <summary> 
    /// Called When the player is killed
    /// </summary>
    public void OnDeath() => OnDeathServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void OnDeathServerRpc()
    {
        OnDeathClientRpc();
    }

    [ClientRpc]
    private void OnDeathClientRpc()
    {
        /// use this for visual effects and sounds
        OnDeathEvent.Invoke();
    }


    private int DamageShield( int damage)
    {
        // dont do anything on ZERO, minus is ok
        if (damage == 0) return damage;

        // work out current health and clamp between values
        var shields = ShieldComponent.GetShield(OwnerClientId) - damage;

        // work out and set the overflow damage 
        damage = shields < 1 ? -shields : 0;

        // clamp values
        shields = Mathf.Clamp(shields, MinValue, MaxShields);

        // update the shield component
        ShieldComponent.SetShield(OwnerClientId, shields);
        return damage;
    }

    private int DamageHealth(int damage)
    {
        // dont do anything on ZERO, minus is ok
        if (damage == 0) return damage;

        // work out current health and clamp between values
        var health = HealthComponent.GetHealth(OwnerClientId) - damage;

        // work out and set the overflow damage 
        damage = health < 1 ? -health : 0;

        // clamp values
        health = Mathf.Clamp(health, MinValue, MaxShields);

        // update the health component
        HealthComponent.SetHealth(OwnerClientId, health);
        return damage;
    }

    /// <summary>
    /// interface for IDamageable
    /// </summary>
    /// 
    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int heal) => TakeDamageServerRpc(-heal);

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        Debug.Log("took damage");
        // damage the shields first 
        var remainingAfterShield = DamageShield(damage);
        // carry remaining damage to health
        var remainingAfterHealth = DamageHealth(remainingAfterShield);

        // damage is remaining damage
        if (remainingAfterHealth > 0)
        {
            OnDeath();
        }
    }

    public bool IsAlive() => HealthComponent.GetHealth(OwnerClientId) > 0;

}



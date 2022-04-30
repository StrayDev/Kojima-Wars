using Unity.Netcode;
using UnityEngine;

public abstract class BaseDefence : NetworkBehaviour, IDamageable
{
    public EBaseDefenceTypes Type => m_type;

    public Health Health => m_health;

    [SerializeField] protected EBaseDefenceTypes m_type;
    [SerializeField] protected Health m_health;

    protected BaseController m_baseController = default;

    public void SetBaseController(BaseController controller)
    {
        m_baseController = controller;
    }

    public bool IsAlive()
    {
        return m_health.CurrentHealth > 0.0f;
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        TakeDamageServerRpc(damage);
    }

    [ServerRpc]
    public void HealDamageServerRpc(int heal)
    {
        TakeDamageClientRpc(heal);
    }


    [ClientRpc]
    private void TakeDamageClientRpc(float damage)
    {
        m_health.Damage(damage);
    }

    [ServerRpc]
    public void HealServerRpc(float healAmount, bool onlyShields, bool onlyHealth)
    {
        m_health.Heal(healAmount);
    }
}
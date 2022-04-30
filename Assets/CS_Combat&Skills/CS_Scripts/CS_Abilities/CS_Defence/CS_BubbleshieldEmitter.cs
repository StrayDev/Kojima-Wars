using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_BubbleshieldEmitter : NetworkBehaviour, IDamageable
{
    [SerializeField] private int health;
    
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject.transform.root.gameObject);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int heal)
    {

    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }

 
}

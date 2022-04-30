using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_AOEStun : CS_NewGrenade
{
    [SerializeField] private float stunStrength;
    [SerializeField] private float stunTime;

    void Update()
    {
        if (!IsServer) return;

        Fuse();

        if (exploded)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius);
            GameObject stunObj = Instantiate(explosion, transform.position, Quaternion.identity);
            stunObj.GetComponent<NetworkObject>().Spawn();
            ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(stunObj), player);
            foreach (var obj in objectsInRange)
            {
                if (obj.GetComponent<IDamageable>() != null)
                {
                    Entity entityHit = obj.GetComponent<Entity>();
                    if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                    {
                        ServerAbilityManager.Instance.PlayerStatusServerRPC(obj.GetComponent<NetworkObject>(),
                            "Slow", stunTime);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_RepulsionBlast : CS_NewGrenade
{
    [Header("Blast Parameters")] public float effectRadius;
    public float explosionForce;

    private void Update()
    {
        if (!IsServer) return;

        Fuse();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        
        if (other.gameObject != player && !other.isTrigger)
        {
            GameObject explosionObj = Instantiate(explosion, transform.position, Quaternion.identity);
            explosionObj.GetComponent<NetworkObject>().Spawn();

            Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius);

            foreach (Collider nearbyObject in colliders)
            {
                Entity entityHit = nearbyObject.GetComponent<Entity>();
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if (entityHit != null && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
       
                    rb.AddExplosionForce(explosionForce, transform.position, effectRadius);

                    Destroy(gameObject);
                }
            }
        }
    }
}
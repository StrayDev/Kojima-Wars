using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_Missile : NetworkBehaviour
{
    [SerializeField] private float downForce;
    [SerializeField] private GameObject explosion;
    [SerializeField] private float damageRadius;
    [SerializeField] private int damagePerMissile;
    
    public LayerMask damageMask;
    private Rigidbody rb;
    
    
    void Start()
    {
        if (!IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        
        rb.AddForce(-transform.up * downForce, ForceMode.Acceleration);

        if (transform.position.y <= -100)
        {
            Destroy(gameObject);
        }

        
    }

    private void OnCollisionEnter(Collision other)
    {
        //if (!IsServer) return;
        
        GameObject explosionObj = Instantiate(explosion, transform.position, Quaternion.identity);
        explosionObj.GetComponent<NetworkObject>().Spawn();
        
        Collider[] EnemiesToDamage = Physics.OverlapSphere(transform.position, damageRadius/*, damageMask*/);

        foreach (var obj in EnemiesToDamage)
        {
            //Damage enemy
            if(obj.GetComponent<IDamageable>() != null)
            {
                Entity entityHit = obj.GetComponent<Entity>();
                Debug.Log("Entity hit by missile: " + entityHit.name);
                if(ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    obj.GetComponent<IDamageable>().TakeDamageServerRpc(damagePerMissile);
                }
            }
        }

        Destroy(gameObject);
        
    }
    
}

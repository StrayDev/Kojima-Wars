using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Unity.Netcode;

public class CS_Ordnance : CS_NewGrenade
{
    //private HealthComponent _healthComponent;
    public GameObject healthPack;
    [SerializeField] private float activeTime;
    [SerializeField] private int healthIncrease;
    private bool doSetUp = false;
    
    private void Update()
    {
        if (!IsServer) return;

        if (!doSetUp)
        {
            fuse = activeTime;
            StartCoroutine(ActiveTime());
            doSetUp = true;
        }

        Fuse();
    }

    private IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //downForce = 0;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            Debug.Log("Stage 1");
        }

        if (other.gameObject.GetComponent<MechCharacterController>())
        {
            Physics.IgnoreCollision(other.collider, GetComponent<Collider>());

            Entity entityHit = other.gameObject.GetComponent<Entity>();
            if (entityHit != null)
            {
                if (!ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    if (other.gameObject.GetComponent<IDamageable>() != null)
                    {
                        other.gameObject.GetComponent<IDamageable>()?.HealDamageServerRpc(healthIncrease);
                    }
                }
            }
        }
    }
}
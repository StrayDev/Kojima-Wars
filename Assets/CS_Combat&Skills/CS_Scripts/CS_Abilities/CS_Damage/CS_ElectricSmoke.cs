using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_ElectricSmoke : CS_NewGrenade
{
    [SerializeField] private float smokeTime;
    [SerializeField] private GameObject smoke;
    private float elecDamTimer;
    private bool activatedOnce;


    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        Fuse();
        
        if (elecDamTimer > 0)
        {
            elecDamTimer -= Time.deltaTime;
        }

        if (exploded)
        {
            if (!activatedOnce)
            {
                GameObject smokeObj = Instantiate(smoke, transform.position, Quaternion.identity);
                smokeObj.GetComponent<NetworkObject>().Spawn();
                ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(smokeObj), player);
                activatedOnce = true;
            }
            
            smokeTime -= Time.deltaTime;
            if (smokeTime <= 0)
            {
                //Destroy(gameObject);
            }
            
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius, enemyMask);

            //Loop through each enemy in range
            foreach (var obj in objectsInRange)
            {
                if(obj.tag == "Enemy")
                {
                    if (elecDamTimer <= 0)
                    {
                        elecDamTimer = 1;
                        obj.GetComponent<IDamageable>()?.TakeDamageServerRpc(damage);
                    }
                }
            }
        }
    }
}

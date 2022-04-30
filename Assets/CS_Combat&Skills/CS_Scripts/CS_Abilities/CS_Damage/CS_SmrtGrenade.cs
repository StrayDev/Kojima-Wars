using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class CS_SmrtGrenade : CS_NewGrenade
{
    [SerializeField] private int lockOnDistance;
    [SerializeField] private float lockOnTravelSpeed;
    [SerializeField] private float lockOnFreezeTime;
    [SerializeField] private Material lockOnMaterial;
    [SerializeField] private GameObject pingEffect;
    private Vector3 lockOnDireciton;
    
    private bool inRange;
    private bool lockedOn;
    private Vector3 targetPosition;
    private bool targetFound;
    
    void Update()
    {
        if (!IsServer) return;
        
        Fuse();
        
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, lockOnDistance/*, enemyMask*/);

        //if there is an object in range
        if (objectsInRange.Length >= 1 && !targetFound)
        {
            //Stop moving grenade 
            foreach (var obj in objectsInRange)
            {
                //Damage enemy
                if(obj.GetComponent<IDamageable>() != null)
                {
                    Entity entityHit = obj.GetComponent<Entity>();
                    if(ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                    {
                        ScreenLog.Instance.Print("ENEMY FOUND, THIS PLAYER: " + gameObject.name , Color.red);

                        targetFound = true;
                        targetPosition = entityHit.transform.position;
                        RemoveForce(targetPosition);
                    }

                }
            }
           
        }
        
        if (lockedOn)
        {
            //Add force towards the enemy

            rb.AddForce(lockOnDireciton * lockOnTravelSpeed, ForceMode.Impulse);
        }

        if (exploded)
        {

            GameObject explosionObject = Instantiate(explosion, transform.position, Quaternion.identity);
            explosionObject.GetComponent<NetworkObject>().Spawn();
            //explosionObject.GetComponent<NetworkObject>().TrySetParent(transform);
            explosionObject.transform.SetParent(transform);
            
            Collider[] EnemiesToDamage = Physics.OverlapSphere(transform.position, damageRadius/*, enemyMask*/);

            foreach (var obj in EnemiesToDamage)
            {
                
                //Damage enemy
                if(obj.GetComponent<IDamageable>() != null)
                {
                    Entity entityHit = obj.GetComponent<Entity>();
                    if(ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                    {
                        obj.GetComponent<IDamageable>().TakeDamageServerRpc(damage);
                    }

                }
            }
            Destroy(gameObject);
        }
        
    }
    
    private void RemoveForce(Vector3 enemyPosition)
    {
        //Remove current trajectory of grenade
        if (!inRange)
        {
            inRange = true;

            GameObject pingObj = Instantiate(pingEffect, transform.position, transform.rotation);
            pingObj.GetComponent<NetworkObject>().Spawn();
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
            //rb.isKinematic = true;
            //GetComponent<Renderer>().material = lockOnMaterial;
            lockOnDireciton = enemyPosition - transform.position;
            lockOnDireciton = lockOnDireciton.normalized;
            StartCoroutine(lockOnDelay());
        }
    }
    
    private IEnumerator lockOnDelay()
    {
        yield return new WaitForSeconds(lockOnFreezeTime);
        rb.constraints = RigidbodyConstraints.None;
       //rb.isKinematic = false;

        lockedOn = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Explode when it hits something after locking on
        if (lockedOn)
        { 
            fuse = 0;
        }
    }
}

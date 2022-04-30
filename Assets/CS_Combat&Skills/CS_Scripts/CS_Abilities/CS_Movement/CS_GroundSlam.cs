using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class CS_GroundSlam : NetworkBehaviour
{
    [SerializeField] private float speedModifier;
    [SerializeField] private float stunTime;

    public float radius;
    [Range(0, 360)] public float viewAngle;

    //TODO TO be replaced when isEnemy is implemented 
    //public GameObject enemy;
    private GameObject player;

    private CS_Firepoints groundSlamPoint;
   // public GameObject explosion;
    
    //Layer of enemies
   // public LayerMask enemyMask;

    //Layer for walls etc
    public LayerMask obstructionMask;
    
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        groundSlamPoint = player.GetComponent<CS_Firepoints>();
        transform.position = groundSlamPoint.groundSlamPoint.position;
        transform.rotation = groundSlamPoint.groundSlamPoint.rotation;
 
       
   
        var position = groundSlamPoint.groundSlamPoint.position;

        var rotation = Quaternion.LookRotation
            (player.GetComponentInChildren<MechLookPitch>().transform.forward);
        
      //  GameObject explosionObj = Instantiate(explosion, position, rotation);
     //   explosionObj.GetComponent<NetworkObject>().Spawn();
        
        
        GroundSlam();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GroundSlam()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, radius);

        foreach (var obj in enemiesInRange)
        {
            if (obj.GetComponent<IDamageable>() != null && enemiesInRange.Length != 0)
            {
                Transform targets = obj.transform;
                Vector3 directionFromTarget = obj.transform.position - player.transform.position.normalized;
                
                //Entity entityHit = obj.GetComponent<Entity>();
                //Debug.Log("Entity hit by fire: " + entityHit.name);
              
                if (Vector3.Angle(player.transform.forward, directionFromTarget) < viewAngle / 2)
                {
                    float distanceFromTarget = Vector3.Distance(obj.transform.position, player.transform.position);

                    //Check to see if enemy in frustum of player, obstruction mask to stop it working through walls 
                    if (!Physics.Raycast(player.transform.position, directionFromTarget, distanceFromTarget,
                        obstructionMask))
                    {
                        //EnemyInRange of ground slam = true;
                        
                  
                        //Damage enemy
                        
                        if (obj.GetComponent<IDamageable>() != null)
                        {
                            DamageEnemies(obj);
                            StunPlayers(obj);
                        }
                    }
                    else
                    {
                       Destroy(gameObject,5);
                    }
                }
                else
                {
                    Destroy(gameObject, 5);
                }
            }
        }
    }

    private void DamageEnemies(Collider targets)
    {
        Entity entityHit = targets.GetComponent<Entity>();
        if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
        {
            
            entityHit.GetComponent<IDamageable>().TakeDamageServerRpc(damage);
            
        }
    }

    private void StunPlayers(Collider targets)
    {
        Entity entityHit = targets.GetComponent<Entity>();
        if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
        {
            //ServerAbilityManager.Instance.SlowEnemyServerRPC(new NetworkObjectReference(targets.gameObject), player.GetComponent<CS_PlayerStats>().speed / speedModifier);
            entityHit.GetComponent<IDamageable>().TakeDamageServerRpc(damage);
            StartCoroutine(ResetSpeedValue());
        }
        
        IEnumerator ResetSpeedValue()
        {
            yield return  new WaitForSeconds(stunTime); 
            ServerAbilityManager.Instance.SlowEnemyServerRPC(new NetworkObjectReference(targets.gameObject), player.GetComponent<CS_PlayerStats>().speed * speedModifier );
            Destroy(gameObject);
        }
        //Apply Stun to enemy in range
        //Movement speed decrease?
        //UI/Particles indicator 
    }
    
}
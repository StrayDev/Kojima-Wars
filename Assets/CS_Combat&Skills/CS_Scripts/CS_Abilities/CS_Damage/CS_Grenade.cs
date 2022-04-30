using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CS_Grenade : MonoBehaviour
{
    //Customisation
    [Header("General Customisation")]
    [SerializeField] grenadeTypes grenadeType;
    [SerializeField] private float force;
    [SerializeField] private float fuse;
    [SerializeField] private float damageRadius;
    [SerializeField] private int damage;
    [SerializeField] private GameObject explosion;
    
    [Header("Air Strike specific")]
    [SerializeField] private GameObject smoke;
    
    [Header("Lock on specific")]
    [SerializeField] private int lockOnDistance;
    [SerializeField] private float lockOnTravelSpeed;
    [SerializeField] private float lockOnFreezeTime;
    [SerializeField] private Material lockOnMaterial;
    
    [Header("Stun on specific")]
    [SerializeField] private float stunStrength;
    [SerializeField] private float stunTime;

    [Header("Electric Smoke specific")]
    [SerializeField] private float smokeTime;

    [Header("Ring of Fire specific")]
    [SerializeField] private GameObject ringOfFire;

    [Header("Recon specific")]
    [SerializeField] float reveal_time;
    [SerializeField] Material mat_1;
    [SerializeField] Material mat_2;

    public LayerMask enemyMask;

    //references
    private Rigidbody rb;
    private GameObject player;
    private CS_Firepoints firepoint;
    private CS_PlayerController playerCont;
    private CS_PlayerStats playerStats;

    private bool exploded;
    private bool inRange;
    private bool lockedOn;
    private Vector3 lockOnDireciton;
    private float elecDamTimer;

    //Types of grenade
    private enum grenadeTypes
    {
        Normal,
        LockOn,
        AirstrikeMarker,
        AOEStun,
        ElecSmoke,
        RoF,
        FlashBang,
        Smoke,
        Recon
    }

    void Awake()
    {
        //Get references
        player = GameObject.FindGameObjectWithTag("Player");
        firepoint = player.GetComponent<CS_Firepoints>();
        rb = GetComponent<Rigidbody>();
        playerCont = player.GetComponent<CS_PlayerController>();
        playerStats = player.GetComponent<CS_PlayerStats>();

        //Spawn at grenade firepoint transform
        transform.position = firepoint.grenadeFirepoint.position;
        rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);
    }

    void Update()
    {
        //Count down fuse
        fuse -= Time.deltaTime;
        if (fuse <= 0 && !playerCont.playerStunned)
        {
            effectEnemiesInRange();
        }

        if (elecDamTimer > 0)
        {
            elecDamTimer -= Time.deltaTime;
        }

        //Lock on grenade travels towards enemies
        LockOnGrenade();
        
        //Destroy stun after period
        destroyStun();
    }

    private void destroyStun()
    {
        if (exploded && playerCont.playerStunned)
        {
            //Debug.Log(stunTime);
            stunTime -= Time.deltaTime;
            this.GetComponent<MeshRenderer>().enabled = false;
        }

        if (stunTime <= 0 && playerCont.playerStunned)
        {
            stunPlayer(playerCont.playerStunned);
            playerCont.playerStunned = false;
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        //Explode when it hits something after locking on
        if (lockedOn)
        {
            fuse = 0;
        }
        //stop rolling 
        rb.drag = 1;
    }

    private void effectEnemiesInRange()
    {
        if (grenadeType == grenadeTypes.Recon)
        {
            if (!playerCont.playerStunned)
            {
                stunPlayer(playerCont.playerStunned);
                playerCont.playerStunned = true;
                exploded = true;
            }
            Instantiate(explosion, transform.position, Quaternion.identity);
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius, enemyMask);
            StartCoroutine(ReconReveal(objectsInRange));
        }
        else if (grenadeType != grenadeTypes.AirstrikeMarker && grenadeType != grenadeTypes.ElecSmoke)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius, enemyMask);
            if(grenadeType == grenadeTypes.RoF)
            {
                Instantiate(ringOfFire, transform.position, Quaternion.identity);
            }
            //Loop through each enemy in range
            foreach (var obj in objectsInRange)
            {
                if (grenadeType == grenadeTypes.AOEStun)
                {
                    Debug.Log("Grenade slowed" + obj.name);
                    if (obj.tag == "Player")
                    {
                        if (!playerCont.playerStunned)
                        {
                            stunPlayer(playerCont.playerStunned);
                            playerCont.playerStunned = true;
                            exploded = true;
                        }
                        //playerCont.speed = playerCont.jumpForce * 2;
                    }

                }
                else 
                {
                    //Damage enemy
                    obj.GetComponent<IDamageable>()?.TakeDamageServerRpc(damage);

                }
            }
            if (grenadeType != grenadeTypes.AOEStun && grenadeType != grenadeTypes.ElecSmoke)
            {
                Debug.Log("Grenade destroyed");
                Destroy(gameObject);
            }
        }
        else
        {
            if (!exploded && grenadeType != grenadeTypes.AOEStun)
            {
                exploded = true;
                Instantiate(smoke, transform.position, Quaternion.Euler(-90,0,0),transform);
                
            }
            else if (grenadeType == grenadeTypes.ElecSmoke)
            {
                ElecSmokeGrenade();
            }
        }
    }

    private void LockOnGrenade()
    {
        if (grenadeType == grenadeTypes.LockOn)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, lockOnDistance, enemyMask);

            //if there is an object in range
            if (objectsInRange.Length >= 1)
            {
                Debug.Log("Enemy in range");
                //Stop moving grenade 
                RemoveForce(objectsInRange[0].transform.position);
            }
            
            if (lockedOn)
            {
                //Add force towards the enemy
                rb.AddForce(lockOnDireciton * lockOnTravelSpeed, ForceMode.Impulse);
            }
            
        }
    }

    private void ElecSmokeGrenade()
    {
        smokeTime -= Time.deltaTime;
        if (smokeTime <= 0)
        {
            Destroy(gameObject);
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
            //deal damage to players in range
        }
    }

        private void RemoveForce(Vector3 enemyPosition)
    {
        //Remove current trajectory of grenade
        if (!inRange)
        {
            inRange = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
            GetComponent<Renderer>().material = lockOnMaterial;
            lockOnDireciton = enemyPosition - transform.position;
            lockOnDireciton = lockOnDireciton.normalized;
            StartCoroutine(lockOnDelay());

        }
    }

    private IEnumerator lockOnDelay()
    {
        yield return new WaitForSeconds(lockOnFreezeTime);
        rb.constraints = RigidbodyConstraints.None;
        lockedOn = true;
    }

    private IEnumerator ReconReveal(Collider[] objectsInRange) 
    {
        foreach (Collider obj in objectsInRange) 
        {
            //obj.gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Universal Rendering Pipeline/ThroughWalls");
            obj.gameObject.GetComponent<MeshRenderer>().material = mat_2;
        }
        //float time = 0f;
        /*while (time < reveal_time) 
        {
            time += Time.deltaTime;        
        }*/
        yield return new WaitForSeconds(reveal_time);
        foreach (Collider obj in objectsInRange)
        {
            //obj.gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Universal Rendering Pipeline/lit");
            obj.gameObject.GetComponent<MeshRenderer>().material = mat_1;
        }
    }

    

    private void stunPlayer(bool stunned)
    {
        if (!stunned)
        {
            playerStats.speed = playerStats.speed / stunStrength;
        }
        else if (stunned)
        {
            playerStats.speed = playerStats.speed * stunStrength;
        }
    }

}


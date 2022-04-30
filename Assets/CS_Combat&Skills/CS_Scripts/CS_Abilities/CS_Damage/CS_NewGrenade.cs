using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CS_NewGrenade : NetworkBehaviour
{
    //Customisation
    [Header("Customisation")]
    public float force;
    public float downForce;
    public float fuse;
    public float damageRadius;
    public int damage; 
    public GameObject explosion;
    public bool impactGrenade;
    public bool bioGren = false;
    
    //references
    [HideInInspector] public Rigidbody rb;
     public GameObject player;
    [HideInInspector] public CS_Firepoints firepoint;
    [HideInInspector] public CS_PlayerController playerCont;
    [HideInInspector] public CS_PlayerStats playerStats;
    public Transform cameraTransform;

    
    public LayerMask enemyMask;
    public bool exploded;

    void Start()
    {
        if (!IsServer)
        {
            
            GetComponent<Rigidbody>().isKinematic = true;
            enabled = false;
            return;
        }
        //Get references
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        if (player == null)
        {
            Debug.LogError("NO PLAYER FOUND");
        }

        firepoint = player.GetComponent<CS_Firepoints>();
        rb = GetComponent<Rigidbody>();
        playerStats = player.GetComponent<CS_PlayerStats>();
        cameraTransform = player.GetComponentInChildren<MechLookPitch>().transform;
        rb.useGravity = false;

        //Spawn at grenade firepoint transform
        transform.position = firepoint.grenadeFirepoint.position;
      
        if (!GetComponent<CS_BubbleShield>())
        {
          transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        }
        rb.AddForce(cameraTransform.forward * force, ForceMode.Impulse);
        Vector3 spin = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)); 
        if (!GetComponent<CS_BubbleShield>())
        {
            rb.AddForceAtPosition(spin, transform.position);
        }
    }


    void FixedUpdate()
    {
        rb.AddForce(Vector3.down*rb.mass*downForce);  
    }
    public void Fuse()
    {
 
        if (fuse <= 0 && !impactGrenade)
        {
            Explode();
        }
        else
        {
            //Count down fuse
            fuse -= Time.deltaTime;
        }
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!enabled) return;
        //stop rolling 
        rb.drag = 1;

        if (impactGrenade)
        {
            Explode();
        }
    }
}

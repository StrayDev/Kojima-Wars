using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Unity.Netcode;

public class CS_BubbleShield : CS_NewGrenade, IDamageable
{
    //Customisation
    [SerializeField] private float health;
    [SerializeField] private float activeTime;

    //References
    //private GameObject player;
    public Collider deviceHitBox;
    //Animations
    private Animator anim;
    public AnimationEvent _animationEvent;
    private bool doSetUp = false;


    private void Update()
    {
        if (!IsServer)return;
        
        if (!doSetUp)
        {
            fuse = activeTime; 
            //player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
            StartCoroutine(ActiveTime());
            doSetUp = true;
        }
        
        Fuse();

        if (exploded)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision other)
    {
        //All things that the bubble shield cant collide with - Need a rework 
        
        if (other.gameObject.GetComponent<MechCharacterController>())
        {
            Physics.IgnoreCollision(other.collider, GetComponentInChildren<Collider>());
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Physics.IgnoreCollision(other.collider, GetComponentInChildren<Collider>());
        }

        if (other.gameObject.tag == "BubbleHolder")
        {
            Physics.IgnoreCollision(other.collider, GetComponentInChildren<Collider>());
        }

        if (other.gameObject.tag == "BubbleShield")
        {
            Physics.IgnoreCollision(other.collider, GetComponentInChildren<Collider>());
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int healAmount)
    {
        //To stop erroring
    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }
}
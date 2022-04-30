using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_RingDamage : NetworkBehaviour
{
    public bool playerInRange;
    public int damageRadius;
    public int damagePerSec = 10;
    private float RoFTimer = 1;

    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        if (RoFTimer > 0)
        {
            RoFTimer -= Time.deltaTime;
        }
        if (playerInRange && RoFTimer <=0)
        {
            Collider[] EnemiesToDamage = Physics.OverlapSphere(transform.position, damageRadius/*, damageMask*/);
            foreach (var obj in EnemiesToDamage)
            {
                //Damage enemy
                if (obj.GetComponent<IDamageable>() != null)
                {
                    Entity entityHit = obj.GetComponent<Entity>();
                    //Debug.Log("Entity hit by fire: " + entityHit.name);
                    RoFTimer = 1;
                    if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                    {
                        obj.GetComponent<IDamageable>().TakeDamageServerRpc(damagePerSec);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            playerInRange = true;
            //Debug.Log(other.gameObject.name + " ENTER");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            playerInRange = false;
            //Debug.Log(other.gameObject.name + " EXIT");
        }
    }
}

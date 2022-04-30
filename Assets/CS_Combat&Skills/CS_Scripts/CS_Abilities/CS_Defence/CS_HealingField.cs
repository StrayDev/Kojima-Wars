using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_HealingField : NetworkBehaviour
{
    public int healingRadius;
    public int healing;
    public int totalHealthHealed;
    public float totalTimeHealing = 7;
    private float healingTimer;
    public GameObject player;



    private void Start()
    {
        if (!IsServer)
        {

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponentInChildren<BoxCollider>().enabled = false;
            enabled = false;
            return;
        }
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        if (player == null)
        {
            Debug.LogError("NO PLAYER FOUND");
        }
       
    }

    void Update()
    {
        
        if (healingTimer > 0)
        {
            healingTimer -= Time.deltaTime;
        }
        totalTimeHealing -= Time.deltaTime;

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, healingRadius);
        foreach (var obj in objectsInRange)
        {
            if (healingTimer <= 0 && totalHealthHealed > 0 && obj.GetComponent<IDamageable>() != null)
            {
                Entity entityHit = obj.GetComponent<Entity>();
                if (!ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    healingTimer = 1;
                    totalHealthHealed -= healing;
                    obj.GetComponent<IDamageable>().HealDamageServerRpc(healing);
                }
            }
            else if(totalHealthHealed == 0 || totalTimeHealing <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}

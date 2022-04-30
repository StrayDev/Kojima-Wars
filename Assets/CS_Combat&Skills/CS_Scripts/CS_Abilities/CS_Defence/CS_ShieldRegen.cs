using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_ShieldRegen : NetworkBehaviour, IDamageable
{
    private HealthComponent _healthComponent;

    //Customisation
    [SerializeField] private int duration;
    [SerializeField] private int placeRange;
    [SerializeField] private int health;
    [SerializeField] private int regenAmount;
    public LayerMask placeableMask;

    //References
    private GameObject player;
    private CS_PlayerController playerController;
    private Transform cameraTransform;
    private float yOffset;
    float yPos;



    private void Start()
    {
        if (!IsServer)
        {
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }

        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        cameraTransform = player.GetComponentInChildren<MechLookPitch>().transform;
        yOffset = transform.localScale.y / 2;

        this.NetworkObject.ChangeOwnership(player.GetComponent<NetworkObject>().OwnerClientId);

        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, placeRange, placeableMask))
        {
            targetPoint = hit.point;
            yPos = hit.point.y;
        }
        else
        {
            RaycastHit floorHit;
            Physics.Raycast(cameraTransform.position, Vector3.down, out floorHit, Mathf.Infinity);
            targetPoint = floorHit.point;
        }
        transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);


        StartCoroutine(DestroyObject());
    }

    
    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);


        Destroy(gameObject);
    }
    
    private void OnTriggerStay(Collider other)
    {
        Entity entityHit = other.gameObject.GetComponent<Entity>();
        if (entityHit != null)
        {
            if (!ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
            {
                if (other.gameObject.GetComponent<IDamageable>() != null)
                {
                    var healthValue = HealthComponent.GetHealth(OwnerClientId);
                    var shieldValue = ShieldComponent.GetShield(OwnerClientId);
                    
                    if (shieldValue < 200 && healthValue == 100) 
                    {
                        var value = shieldValue + regenAmount;
                        ShieldComponent.SetShield(OwnerClientId, value);
                        //TODO Check working on Develop 
                    }
                }
            }
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
    public void HealDamageServerRpc(int heal)
    {

    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }
}
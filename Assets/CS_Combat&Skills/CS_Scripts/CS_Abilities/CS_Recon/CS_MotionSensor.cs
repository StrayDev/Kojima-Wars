using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class CS_MotionSensor : NetworkBehaviour, IDamageable
{
    //Customisation
    [SerializeField] private float detectionRadius;
    [SerializeField] private int duration;
    [SerializeField] private int placeRange;
    [SerializeField] private float health;
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
        
        NetworkObject.ChangeOwnership(player.GetComponent<NetworkObject>().OwnerClientId);
        
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
        transform.position = new Vector3(targetPoint.x ,targetPoint.y + yOffset, targetPoint.z);


        StartCoroutine(DestroyObject());
    }


    void Update()
    {
        if (!IsServer) return;

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);

        //Loop through each enemy in range
        foreach (var obj in objectsInRange)
        {
            Entity entityHit = obj.GetComponent<Entity>();
            if (entityHit && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
            {
                ServerAbilityManager.Instance.RevealEnemyServerRPC(
                    ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId,
                    entityHit.gameObject, 1f);
            }

        }

    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);


        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color =new Vector4(0,1,0,0.2f);
        Gizmos.DrawSphere(transform.position /*+ (transform.forward * transform.localScale.z / 2)*/, detectionRadius);
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
        //throw new NotImplementedException();
    }


    public bool IsAlive()
    {
        return health > 0.0f;
    }


}

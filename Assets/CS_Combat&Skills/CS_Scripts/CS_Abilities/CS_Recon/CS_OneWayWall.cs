using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class CS_OneWayWall : NetworkBehaviour, IDamageable
{
    
    //Customisation
    [SerializeField] private int placeRange;
    [SerializeField] private int duration;
    [SerializeField] private float YOffsetAddition;
    [SerializeField] private float XOffsetAddition;
    [SerializeField] private int RotationOffset;
    [SerializeField] private float health;
    public LayerMask placeableMask;
    

    //References
    private GameObject player;
    private Transform cameraTransform;
    
    private float yOffset;
    private float xOffset;
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
        yOffset += YOffsetAddition;
        xOffset = transform.localScale.x / 2;
        xOffset += XOffsetAddition;
        


        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, placeRange, placeableMask))
        {
            targetPoint = hit.point;


        }
        else
        {
            RaycastHit floorHit;
            Physics.Raycast(cameraTransform.position + cameraTransform.forward * 15, Vector3.down, out floorHit, Mathf.Infinity);
            targetPoint = floorHit.point;

        }

        transform.rotation = Quaternion.LookRotation
            (player.GetComponentInChildren<MechLookPitch>().transform.forward);
        Quaternion temp = transform.rotation;
        temp.x = 0;
        temp.z = 0;
        transform.rotation = temp;
        transform.rotation *= Quaternion.Euler(0,RotationOffset,0);
        
        transform.position = new Vector3(targetPoint.x + xOffset, targetPoint.y + yOffset, targetPoint.z);
       

        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
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
        return health <= 0.0f;
    }

}

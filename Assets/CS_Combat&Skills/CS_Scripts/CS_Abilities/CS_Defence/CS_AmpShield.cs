using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class CS_AmpShield : NetworkBehaviour
{
    public int duration;
    public int placeRange;
    private bool boostActive;
    private bool shieldInSight = false;
    private float yOffset;
    public LayerMask placeableMask;
    public LayerMask boostMask;
    private GameObject player;
    Vector3 targetPoint;
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        yOffset = transform.localScale.y / 2;

        cameraTransform = player.GetComponentInChildren<MechLookPitch>().transform;
        RaycastHit hit;

        //If looking at the floor in range 
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, placeRange, placeableMask))
        {
            Debug.Log("Raycast Hit");
            targetPoint = hit.point;
            transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);
            transform.rotation = player.transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (!IsServer) return;

        StartCoroutine(DestroyObject());

        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("AmpShield"))
            {
                shieldInSight = true;
            }
            else
            {
                shieldInSight = false;
            }
        }

        if (shieldInSight && !boostActive)
        {
            boostActive = true;
            ServerAbilityManager.Instance.PlayerStatusServerRPC(player, "DamageBoost", 30);
        }
        if (!shieldInSight)
        {
            boostActive = false;
            ServerAbilityManager.Instance.PlayerStatusServerRPC(player, "DamageDecrease", 30);
        }
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);
        boostActive = false;
        shieldInSight = false;
        Destroy(gameObject);
    }
}

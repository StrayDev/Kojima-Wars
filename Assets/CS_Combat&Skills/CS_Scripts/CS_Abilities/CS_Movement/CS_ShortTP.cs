using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class CS_ShortTP : NetworkBehaviour
{
    public int placeRange;
    private float yOffset;
    public LayerMask placeableMask;
    Vector3 targetPoint;
    private GameObject player;
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Short TP");

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
            targetPoint = hit.point;
            transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);
            ServerAbilityManager.Instance.TeleportServerRPC(player.GetComponent<NetworkObject>(), gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
    }
}

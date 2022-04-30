using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientSyncTransform : NetworkBehaviour
{
    private void Start()
    {
        if(!IsLocalPlayer)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SyncTransformServerRpc(transform.position, transform.rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    void SyncTransformServerRpc(Vector3 newPosition, Quaternion newRotation)
    {
        SyncTransformClientRpc(newPosition, newRotation);
    }

    [ClientRpc]
    void SyncTransformClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        if(!IsLocalPlayer)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AIUnitMover : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SyncTransformServerRpc(Vector3 movePosition, ulong unit)
    {
        SyncTransformClientRpc(movePosition, unit);
    }

    [ClientRpc]
    void SyncTransformClientRpc(Vector3 movePosition, ulong unit)
    {
        //print("Set Move Pos");
        foreach (var object_ in FindObjectsOfType<NetworkObject>())
        {
            if (object_.NetworkObjectId == unit)
            {
                object_.GetComponent<AIMovement>().move(movePosition);
                foreach(var info in object_.GetComponent<AIMovement>().defenceLocation.defenceLocationInfo)
                {
                    if(info.unit == object_.gameObject)
                    {
                        info.groupId = -1;
                    }
                }
            }
        }
    }
}

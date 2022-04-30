
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if ( IsLocalPlayer )
        {
            Debug.Log($"On Network Spawn { OwnerClientId }");
        }
    }
}

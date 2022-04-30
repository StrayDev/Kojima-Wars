using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MapController : NetworkBehaviour
{
    public static List<MapController> List = new List<MapController>();

    public override void OnNetworkSpawn()
    {
        // keep in a static list for access
        List.Add(this);

        if (!IsOwner) return;
    }

    public override void OnNetworkDespawn()
    {
        List.Remove(this);

        if (!IsOwner) return;
    }
}

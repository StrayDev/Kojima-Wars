using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Networking;

public class CS_SpeedBoost : NetworkBehaviour
{
    public int speedBoostValue;
    public float duration;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;

        ServerAbilityManager.Instance.SpeedboostServerRPC(player.GetComponent<NetworkObject>(), 2, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
    }
}

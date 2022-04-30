using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TempPopulatePlayerData : NetworkBehaviour
{
    [SerializeField] private PlayerDataSO playerData = default;

    private void Awake()
    {
        if (!IsServer) return;

        /// this causes an error but is temporary
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var data = new PlayerData();

            data.SetName("NAME");
            data.SetTeamRed(true);

            playerData.List.Add(client.ClientId, data);
        }

    }
}

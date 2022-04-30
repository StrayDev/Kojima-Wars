using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CurrencyManager : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerCurrencyServerRpc(int cash, string team)
    {
        UpdatePlayerCurrencyClientRpc(cash, team);
    }

    [ClientRpc]
    public void UpdatePlayerCurrencyClientRpc(int cash, string team)
    {
        foreach (var player in FindObjectsOfType<PlayerInformation>())
        {
            player.IncreasePlayerCurrencyIfNameMatch(cash, team);
        }
    }
}

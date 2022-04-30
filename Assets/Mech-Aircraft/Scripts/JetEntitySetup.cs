using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class JetEntitySetup : NetworkBehaviour
{
    [SerializeField] private PlayerDataSO playerData = default;
    [SerializeField] private Entity entity = default;

    private void Start()
    {
        if (!IsServer) return;

        var team = playerData.GetPlayerTeam(OwnerClientId);
        var str = team == true ? "red" : "blue";
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeTEamServerRpc(FixedString32Bytes str)
    {
        entity.ChangeTeamClientRpc(str);
    }
}

using Unity.Netcode;
using Core;

public class AbilityPrefabSpawner : NetworkBehaviour
{
    static AbilityPrefabSpawner Singleton = default;
    private void OnEnable() => Singleton = this;


    public static void SpawnAbilityPrefab(int ability, NetworkObjectReference player)
    {
        Singleton.SpawnAbilityPrefabServerRpc(ability, player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAbilityPrefabServerRpc(int id, NetworkObjectReference player)
    {
        player.TryGet(out var playerObject);
        playerObject.TryGetComponent<CS_UseAbilities>(out var mech);

        var ability = mech.GetPlayerClass().GetAbility(id);
        var createdAbility = Instantiate(ability.GetAbilityPrefab());

        createdAbility.GetComponent<NetworkObject>().Spawn();
        ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(createdAbility), player);
    }
}

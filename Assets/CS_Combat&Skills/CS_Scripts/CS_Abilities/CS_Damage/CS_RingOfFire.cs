using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class CS_RingOfFire : CS_NewGrenade
{

    [SerializeField] private GameObject ringOfFire;

    private bool activatedOnce;
    void Update()
    {
        if (!IsServer) return;

        Fuse();

        if (exploded && !activatedOnce)
        {
            TestServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestServerRpc()
    {
        GameObject fireObj = Instantiate(ringOfFire, transform.position, Quaternion.identity);
        fireObj.GetComponent<NetworkObject>().Spawn();
        ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(fireObj), player);
        activatedOnce = true;
        Destroy(gameObject);
    }
}

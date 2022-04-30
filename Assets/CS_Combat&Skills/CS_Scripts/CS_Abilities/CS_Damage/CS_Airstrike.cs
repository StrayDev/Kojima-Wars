using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_Airstrike : CS_NewGrenade
{
    [SerializeField] private GameObject smoke;
    private bool deployedSmoke;
    
    [SerializeField] private int numOfMissiles;
    [SerializeField] private GameObject missile;
    [SerializeField] private float spawnRadius;
    [SerializeField] private int missileDelay;
    [SerializeField] private int missileHeight;

    private Vector3 pos;
    void Update()
    {
        if (!IsServer) return;

        Fuse();

        if (exploded && !deployedSmoke)
        {
            if (!deployedSmoke)
            {
                GameObject smokeObj =  Instantiate(smoke, transform.position, Quaternion.Euler(-90,0,0)/*,transform*/);
                smokeObj.GetComponent<NetworkObject>().Spawn();
                smokeObj.GetComponent<NetworkObject>().TrySetParent(transform);
                ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(smokeObj), player);

                smokeObj.transform.parent = transform.parent;
                StartCoroutine(spawnMissiles());
                StartCoroutine(destroyObjects(smokeObj));

                deployedSmoke = true;
            }
        }
    }
    
    
    private IEnumerator spawnMissiles()
    {
        yield return new WaitForSeconds(missileDelay);

        pos = transform.position;
        for (int i = 0; i < numOfMissiles; i++)
        {
            missileHeight = Random.Range(missileHeight, missileHeight+60);
            Vector3 position =
                new Vector3(Random.Range(pos.x - spawnRadius, pos.x + spawnRadius), 
                    transform.position.y + missileHeight,
                    Random.Range(pos.z - spawnRadius, pos.z + spawnRadius));
            
            
            GameObject missleObj = Instantiate(missile, position, Quaternion.identity);
            missleObj.GetComponent<NetworkObject>().Spawn();
            ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(missleObj), player);
        }
     

    }

    private IEnumerator destroyObjects(GameObject smoke)
    {
        yield return new WaitForSeconds(missileDelay + 10f);
        Destroy(smoke);
        Destroy(gameObject);
    }
    
}

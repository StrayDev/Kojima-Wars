using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_SmokeScript : CS_NewGrenade
{
    public int duration;

    void Update()
    {
        Fuse();

        if (exploded)
        {
            GameObject smokeObject = Instantiate(explosion, transform.position, Quaternion.identity);
            smokeObject.GetComponent<NetworkObject>().Spawn();
            Destroy(gameObject);
        }
    }
}

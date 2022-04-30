using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CS_SmokeDestroy : NetworkBehaviour
{
    public int duration;

    void Start()
    {
        if (!IsServer)
        {
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!IsServer) return;
    }

    private void Awake()
    {
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}

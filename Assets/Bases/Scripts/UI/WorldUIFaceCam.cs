using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WorldUIFaceCam : NetworkBehaviour
{
    private void Start()
    {
        //if (IsOwner)
        //{
        //    gameObject.SetActive(false);
        //}
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_PlaceSAM : NetworkBehaviour
{
    public GameObject turret;
    [SerializeField] bool placed = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        Instantiate(turret, this.transform.position + (transform.forward * 2), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (!placed) 
        {
            turret.transform.position = this.transform.position + (transform.forward * 2);
        }

        if (/* INPUT BUTTON TO PLACE TURRET IS PRESSED */placed) 
        {
            placed = true;
            turret.GetComponent<CS_SAMScript>().enabled = true;
        }
    }
}

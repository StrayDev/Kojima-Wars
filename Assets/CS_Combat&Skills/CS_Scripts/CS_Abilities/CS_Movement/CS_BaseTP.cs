using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_BaseTP : NetworkBehaviour
{
    private GameObject player;
    private InputActionManager playerInput;
    private int baseNumber = 0;

    private void Start()
    {
        Debug.Log("Base TP");

        if (!IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        playerInput = player.GetComponent<InputActionManager>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (!IsServer) return;
        if (playerInput.inputActions.Mech.FireWeapon.IsPressed())
        {
            Teleport();
        }
    }

    public void Teleport()
    {
        baseNumber = UnityEngine.Random.Range(0, 5);

        List<Transform> Bases = new List<Transform>(); 

        foreach (Transform child in transform)
        {
            Bases.Add(child);
        }

        //player.transform.position = Bases[baseNumber].position;
        ServerAbilityManager.Instance.UpdateTransformClientRpc(player, Bases[baseNumber].position, player.transform.rotation);
    }
}

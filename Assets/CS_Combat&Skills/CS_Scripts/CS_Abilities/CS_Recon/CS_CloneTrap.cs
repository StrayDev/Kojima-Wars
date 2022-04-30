using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_CloneTrap : NetworkBehaviour
{
    public GameObject clone;
    private Rigidbody rb;
    private GameObject player;
    private Camera FPSCamera;
    private CS_Firepoints firepoint;
    //public List<Material> mats;
    public float dissolveSpeed = 1;

    public Texture2D redMat;
    public Texture2D blueMat;

    float pos;
    bool deployed = false;
    public float speed = 3f;

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        firepoint = player.GetComponent<CS_Firepoints>();
        rb = GetComponent<Rigidbody>();

        /*transform.position = firepoint.grenadeFirepoint.position;
        rb.AddForce(firepoint.grenadeFirepoint.transform.forward * 15, ForceMode.Impulse);*/

        Vector3 tempPos = player.transform.position + player.transform.forward * 5;
        tempPos.y += 1f;
        transform.position = tempPos;

        transform.rotation = Quaternion.LookRotation
            (player.GetComponentInChildren<MechLookPitch>().transform.forward);
        Quaternion temp = transform.rotation;
        temp.z = 0;
        temp.x = 0;
        transform.rotation = temp;

        if (player.GetComponent<Entity>().TeamName == "red")
        {
            ServerAbilityManager.Instance.ChangeCloneShaderServerRPC(GetComponent<NetworkObject>(),
                "red");
        }
        else if (player.GetComponent<Entity>().TeamName == "blue")
        {
            ServerAbilityManager.Instance.ChangeCloneShaderServerRPC(GetComponent<NetworkObject>(),
                "blue");
        }

        clone.SetActive(true);
        deployed = true;
        Invoke("Death", 10f);
        ServerAbilityManager.Instance.ApplyAnimationServerRPC(GetComponent<NetworkObject>(),
            "Dissolve", true);
    }

    public void Death()
    {
        ServerAbilityManager.Instance.ApplyAnimationServerRPC(GetComponent<NetworkObject>(),
            "Dissolve", false);
        Destroy(gameObject, 1.5f);
    }

    private void Update()
    {
        if (!IsServer) return;
        if (deployed)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    IEnumerator Deploy()
    {
        yield return new WaitForSeconds(1);
        clone.SetActive(true);
        deployed = true;
        Invoke("Death", 10f);
        
    }

}

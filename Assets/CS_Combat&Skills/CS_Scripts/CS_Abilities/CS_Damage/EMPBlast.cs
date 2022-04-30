using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Networking;

public class EMPBlast : NetworkBehaviour
{
    public float blast_range;
    public float blast_speed;
    public Vector3 start_pos;
    public Vector3 blast_direction;
    public GameObject player;
    public Camera FPSCamera;
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Firing Blast");
        if (!IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        Debug.Log("Firing Blast");
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        //GetFPSCamRecursively(player.transform);
        transform.position = player.GetComponentInChildren<MechLookPitch>().transform.position;
        start_pos = player.transform.position;
        blast_direction = player.GetComponentInChildren<MechLookPitch>().transform.forward;
        transform.LookAt(blast_direction.normalized + transform.position);
        ps = this.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (Vector3.Distance(start_pos, transform.position) < blast_range)
        {
            var sh = ps.shape;
            transform.position += (blast_direction * blast_speed) * Time.deltaTime;
            transform.localScale *= 1 + (0.10f * Time.deltaTime);
            sh.scale *= 1 + (0.10f * Time.deltaTime);
            ps.startSpeed *= 1 + (0.50f * Time.deltaTime);
            ps.startSize *= 1 + (0.50f * Time.deltaTime);
            transform.localScale += new Vector3(10f, 10f, 0f) * Time.deltaTime;
            sh.scale += new Vector3(10f, 10f, 0f) * Time.deltaTime;
        }
        else 
        {
            Debug.Log("Blast ended");
            Destroy(this.gameObject);
        }
    }
    private void GetFPSCamRecursively(Transform obj)
    {
        //Get HUD recursively in children
        foreach (Transform child in obj)
        {
            if (child.TryGetComponent(out Camera camera))
            {
                FPSCamera = camera;
            }
            else
            {
                GetFPSCamRecursively(child);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter " + other.name);
        if (other.gameObject.GetComponent<IDamageable>() != null) 
        {
            if (other.gameObject.GetComponent<MechCharacterController>() == null)
            {
                /*Entity entityHit = other.gameObject.GetComponent<NetworkTransformComponent>().mechSwitchScript.gameObject.GetComponent<Entity>();
                if (entityHit != null && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit)) 
                {*/
                    Debug.Log("Hit enemy Plane: " + other.name);
                    //other.GetComponent<NetworkTransformComponent>().mode = Mode.Mech;
                    other.GetComponent<NetworkTransformComponent>().ForceSwitchMechMode();
                    ServerAbilityManager.Instance.DisableAbilitiesServerRPC(other.GetComponent<NetworkObject>(), 10);
                    //other.gameObject.GetComponent<TransformController>().mode = 0; // turn any plane hit into mech
                /*}*/

            }
            else 
            {
                Entity entityHit = other.gameObject.GetComponent<Entity>();
                if (entityHit != null && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    if (other.gameObject.GetComponent<MechCharacterController>() == null)
                    {
                        Debug.Log("Hit enemy Plane: " + other.name);
                        other.GetComponent<NetworkTransformComponent>().mode = Mode.Mech;
                        other.GetComponent<NetworkTransformComponent>().ForceSwitchMechMode();
                        ServerAbilityManager.Instance.DisableAbilitiesServerRPC(other.GetComponent<NetworkObject>(), 10);
                        //other.gameObject.GetComponent<TransformController>().mode = 0; // turn any plane hit into mech
                    }
                    else
                    {
                        Debug.Log("Hit enemy Mech: " + other.name); //hit a mech, disable abilities. Not implemented yet.
                        ServerAbilityManager.Instance.DisableAbilitiesServerRPC(other.GetComponent<NetworkObject>(), 10);
                    }

                }
            }
            
                
            //other.gameObject.GetComponent<TransformController>().mode = (other.gameObject.GetComponent<TransformController>().mode - 1) * (other.gameObject.GetComponent<TransformController>().mode - 1); //if 0, change to 1. If 1, change to 0.
        }
    }
}

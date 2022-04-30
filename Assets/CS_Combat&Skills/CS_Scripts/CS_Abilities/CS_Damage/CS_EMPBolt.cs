using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Networking;

public class CS_EMPBolt : NetworkBehaviour
{
    public float cast_time = 3f;
    public float cast_range = 200f;
    bool ready = true;
    public Material aim;
    public Material fire;
    CS_Firepoints fire_points;
    GameObject player;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        fire_points = player.GetComponent<CS_Firepoints>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        if (ready)
        {
            StartCoroutine(ShootTime());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (!ready || ready) 
        {
            Vector3[] pos = new Vector3[2];
            pos[0] = fire_points.shoulderWeaponFirepoint.position;
            RaycastHit hit;
            Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, player.GetComponentInChildren<MechLookPitch>().transform.forward, out hit, cast_range);
            if (hit.point.x != 0)
            {
                pos[1] = hit.point;
                //Debug.Log(pos[1]);
            }
            else
            {
                pos[1] = player.GetComponentInChildren<MechLookPitch>().transform.position + player.GetComponentInChildren<MechLookPitch>().transform.forward * cast_range;
                //Debug.Log(pos[1]);
            }
            lr.SetPositions(pos);
            ServerAbilityManager.Instance.LineRenderServerRPC(GetComponent<NetworkObject>(), pos[0], pos[1]);
        }
        
        
    }

    IEnumerator ShootTime() 
    {
        //Debug.Log("Aiming bolt");
        lr.startWidth = 0.1f;
        lr.material = aim;
        lr.enabled = true;
        ready = false;
        yield return new WaitForSeconds(cast_time - 0.3f);
        lr.startWidth = 1f;
        lr.material = fire;
        yield return new WaitForSeconds(0.3f);
        FireBolt();
        //Debug.Log("Bolt fired");
        lr.material = aim;
        lr.enabled = false;
        ready = true;
        Destroy(this.gameObject);
    }

    void FireBolt() 
    {
        RaycastHit hit;
        if (Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, player.GetComponentInChildren<MechLookPitch>().transform.forward, out hit, cast_range))
        {
            if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                if (hit.collider.gameObject.GetComponent<MechCharacterController>() == null)
                {
                    Debug.Log("Hit enemy Plane: " + hit.collider.name);
                    hit.collider.GetComponent<NetworkTransformComponent>().mode = Mode.Mech;
                    hit.collider.GetComponent<NetworkTransformComponent>().ForceSwitchMechMode();
                    ServerAbilityManager.Instance.DisableAbilitiesServerRPC(hit.collider.gameObject.GetComponent<NetworkObject>(), 10);
                    //other.gameObject.GetComponent<TransformController>().mode = 0; // turn any plane hit into mech
                }
                else 
                {
                    Debug.Log("Hit");
                    Entity entityHit = hit.collider.gameObject.GetComponent<Entity>();
                    if (entityHit != null && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                    {
                        ServerAbilityManager.Instance.DisableAbilitiesServerRPC(hit.collider.GetComponent<NetworkObject>(), 10);
                        Debug.Log("Hit " + entityHit.name);
                        //hit.collider.gameObject.GetComponent<TransformController>().mode = 0;   //should turn any plane hit into a mech?
                    }

                }
                
            }
            else
            {
                //Debug.Log("Missed");
            }
        }
        else 
        {
            //Debug.Log("Missed");
        }
    }
}

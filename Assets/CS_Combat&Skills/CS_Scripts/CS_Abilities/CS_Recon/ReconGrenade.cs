using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class ReconGrenade : CS_NewGrenade
{
    [SerializeField] float reveal_time;
    [SerializeField] Shader see_through;
    [SerializeField] Shader original;

    
    private bool activatedOnce;
    //private Camera FPSCamera;
    /*void Start()
    {
        if (!IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        //Get references
        //player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        this.GetComponent<NetworkObject>().Spawn();
        GetFPSCamRecursively(player.transform);
        firepoint = player.GetComponent<CS_Firepoints>();
        rb = GetComponent<Rigidbody>();
        playerCont = player.GetComponent<CS_PlayerController>();
        playerStats = player.GetComponent<CS_PlayerStats>();

        //Spawn at grenade firepoint transform
        transform.position = firepoint.grenadeFirepoint.position;

        rb.AddForce(this.cameraTransform.forward * force, ForceMode.Impulse);
    }*/

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        Fuse();

        if (exploded && !activatedOnce)
        {
            GameObject explObj = Instantiate(explosion, transform.position, Quaternion.Euler(-90, 0, 0), transform);
            explObj.GetComponent<NetworkObject>().Spawn();
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius);
            activatedOnce = true;
            StartCoroutine(ReconReveal(objectsInRange));
        }
    }

    private void GetFPSCamRecursively(Transform obj)
    {
        //Get HUD recursively in children
        foreach (Transform child in obj)
        {
            if (child.TryGetComponent(out Transform camera))
            {
                this.cameraTransform = camera;
            }
            else
            {
                GetFPSCamRecursively(child);
            }
        }
    }

    private IEnumerator ReconReveal(Collider[] objectsInRange)
    {
        foreach (Collider obj in objectsInRange)
        {
            //obj.gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Universal Rendering Pipeline/ThroughWalls");
            if (obj.GetComponent<IDamageable>() != null)
            {
                Entity entityHit = obj.GetComponent<Entity>();
                if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    //set material to see through walls
                    /*foreach (Transform child in obj.transform)
                    {
                        if (child.gameObject.GetComponent<MeshRenderer>().materials.Length > 0)
                        {
                            foreach (Material m in child.gameObject.GetComponent<MeshRenderer>().materials)
                            {
                                m.shader = see_through;
                            }
                        }
                    }*/
                    Debug.Log("Hit " + obj.name);
                    //changeMaterialRecurisvely(obj.gameObject, see_through/*, Color.red*/);
                    ServerAbilityManager.Instance.RevealEnemyServerRPC(ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId, obj.gameObject.GetComponent<NetworkObject>(), reveal_time);
                }

            }
        }
        /*float time = 0f;
        while (time < reveal_time) 
        {
            time += Time.deltaTime;        
        }*/
        yield return new WaitForSeconds(reveal_time);
        foreach (Collider obj in objectsInRange)
        {
            if (obj.GetComponent<IDamageable>() != null)
            {
                Entity entityHit = obj.GetComponent<Entity>();
                if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    //set material back to normal
                    /*foreach (Transform child in obj.transform)
                    {
                        if (child.gameObject.GetComponent<MeshRenderer>().materials.Length > 0) 
                        {
                            foreach (Material m in child.gameObject.GetComponent<MeshRenderer>().materials)
                            {
                                m.shader = original;
                            }
                        }
                    }*/
                    //changeMaterialRecurisvely(obj.gameObject, original/*, Color.clear*/);
                }

            }

            //obj.gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Universal Rendering Pipeline/lit");
        }
        Destroy(this.gameObject);
    }

    private void changeMaterialRecurisvely(GameObject gameObject, Shader shader /*Color color*/)
    {
        if (gameObject.TryGetComponent(out SkinnedMeshRenderer smr))
        {
            Debug.Log("Found smr");
            if (smr.GetComponent<Renderer>().material.shader != shader)
            {
                smr.GetComponent<Renderer>().material.shader = shader;
                //smr.GetComponent<Renderer>().material.color = color;
            }

        }
        else if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            Debug.Log("Found mr");
            if (mr.material.shader != shader)
            {
                mr.material.shader = shader;
               // mr.material.color = color;
            }
        }
        else
        {
            Debug.Log("Nothing to Change");
        }
        foreach (Transform child in gameObject.transform)
        {
            changeMaterialRecurisvely(child.gameObject, shader/*, color*/);
        }
    }
}

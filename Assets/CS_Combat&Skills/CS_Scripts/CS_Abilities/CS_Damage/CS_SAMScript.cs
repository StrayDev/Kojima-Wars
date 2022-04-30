using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Networking;

public class CS_SAMScript : NetworkBehaviour
{
    public float aggro_range = 200;
    public int damage = 75;
    public float lock_on_time = 5;
    float timer;
    public LayerMask plane_layer;
    [SerializeField] Collider[] targets_in_range;
    [SerializeField] GameObject stand;
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject lazer_point;
    [SerializeField] GameObject fire_point1;
    [SerializeField] GameObject fire_point2;
    public GameObject current_target;
    public GameObject player;
    public GameObject muzzle_blast;
    public GameObject explosion;
    LineRenderer lr;

    //private CS_PlayerInput playerInput;
    public InputActionManager playerInput;
    [SerializeField] private int placeRange;
    private bool placed = true;
    bool firing = false;
    public float yOffset;
    float yPos;
    public LayerMask placeableMask;
    Transform FPSCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            Destroy(GetComponent<Rigidbody>());
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        //this.GetComponent<NetworkObject>().Spawn();
        lr = this.GetComponent<LineRenderer>();/*
        lazer_point = transform.Find("LazerPoint").gameObject;*/
        timer = lock_on_time;
        current_target = this.gameObject;
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        this.NetworkObject.ChangeOwnership(player.GetComponent<NetworkObject>().OwnerClientId);
        FPSCamera = player.GetComponentInChildren<MechLookPitch>().transform;
        playerInput = player.GetComponent<InputActionManager>();
        this.transform.rotation = Quaternion.identity;
        //this.GetComponent<NetworkObject>().Spawn();
        RaycastHit hit;
        Vector3 targetPoint;

        //If looking at the floor in range 
        if (Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, player.GetComponentInChildren<MechLookPitch>().transform.forward, out hit, placeRange, placeableMask))
        {
            targetPoint = hit.point;
            yPos = hit.point.y;
        }
        //Otherwise place at the closest point
        else
        {
            RaycastHit floorHit;
            Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, Vector3.down, out floorHit, Mathf.Infinity);
            targetPoint = floorHit.point;
        }
        transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);
        //yOffset = transform.localScale.y / 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (placed)
        {
            if (firing)
            {
                lr.enabled = true;
            }
            else 
            {
                lr.enabled = false;            
            }
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
            CheckTargets();
            if (current_target != this.gameObject)
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = lazer_point.transform.position;
                positions[1] = current_target.transform.position;
                lr.positionCount = 2;
                lr.SetPositions(positions);
                ServerAbilityManager.Instance.LineRenderServerRPC(GetComponent<NetworkObject>(), positions[0], positions[1]);
                TrackTarget();
            }
        }
        /*else 
        {
            //Choose where to place object
            RaycastHit hit;
            Vector3 targetPoint;

            //If looking at the floor in range 
            if (Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, player.GetComponentInChildren<MechLookPitch>().transform.forward, out hit, placeRange, placeableMask))
            {
                targetPoint = hit.point;
                yPos = hit.point.y;
            }
            //Otherwise place at the closest point
            else
            {
                RaycastHit floorHit;
                Physics.Raycast(player.GetComponentInChildren<MechLookPitch>().transform.position, Vector3.down, out floorHit, Mathf.Infinity);
                targetPoint = floorHit.point;
            }
            transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);

            if (InputManager.MECH.FireWeapon.IsPressed())
            {
                Debug.Log("Place");
                placed = true;
            }
        }*/
        
    }

    

    void CheckTargets() 
    {
        targets_in_range = Physics.OverlapSphere(transform.position, aggro_range);
        if (targets_in_range.Length > 0) 
        {
            foreach (Collider target in targets_in_range) 
            {
                if (target.gameObject.GetComponent<IDamageable>() != null) 
                {
                    Entity ent = target.gameObject.GetComponent<Entity>();
                    if (ent != null && ServerAbilityManager.Instance.IsEnemy(gameObject, ent) /*&& target.gameObject.GetComponent<MechCharacterController>() == null*/)
                    {
                        //Debug.Log("New Target: " + current_target.name);
                        current_target = target.gameObject;
                        ServerAbilityManager.Instance.LineRenderServerRPC(GetComponent<NetworkObject>(), transform.position, transform.position);
                        break;

                    }
                        
                }

            }
            
        }
        
        
    }

    void TrackTarget() 
    {
        /*stand.*/transform.LookAt(new Vector3(current_target.transform.position.x, transform.position.y, current_target.transform.position.z));
        barrel.transform.LookAt(current_target.transform.position);
        if (Vector3.Distance(transform.position, current_target.transform.position) > aggro_range) 
        {
            current_target = this.gameObject;
            timer = lock_on_time;
            Debug.Log("Target exited range, no longer locking on.");
            firing = false;
        }
        if (!firing && current_target != this.gameObject)
        {
            firing = true;
            StartCoroutine(ShootPlane());
        }
    }

    IEnumerator ShootPlane() 
    {
        firing = true;
        Debug.Log("Locking onto " + current_target.name + ", " + Mathf.Round(timer));
        yield return new WaitForSeconds(lock_on_time);
        if (firing)
        {
            Debug.Log("Firing at " + current_target.name);
            //ScreenLog.Instance.Print("Firing at " + current_target, Color.red);
            Fire();
        }
    }

    private void Fire()
    {
        GameObject blast = Instantiate(explosion, current_target.transform.position, Quaternion.identity);
        blast.GetComponent<NetworkObject>().Spawn();
        blast.GetComponent<NetworkObject>().TrySetParent(transform);
        if (Random.Range(0, 100) < 50)
        {
            GameObject mb = Instantiate(muzzle_blast, fire_point1.transform.position, Quaternion.identity);
            mb.GetComponent<NetworkObject>().Spawn();
            mb.GetComponent<NetworkObject>().TrySetParent(transform);
            ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(mb), player);
            StartCoroutine(DeleteObj(mb));
        }
        else
        {
            GameObject mb = Instantiate(muzzle_blast, fire_point2.transform.position, Quaternion.identity);
            mb.GetComponent<NetworkObject>().Spawn();
            mb.GetComponent<NetworkObject>().TrySetParent(transform);
            ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(mb), player);
            StartCoroutine(DeleteObj(mb));
        }
        ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(blast), player);
        StartCoroutine(DeleteObj(blast));
        Entity entityHit = current_target.GetComponent<Entity>();
        if (ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit) && IsServer)
        {
            entityHit.GetComponent<IDamageable>().TakeDamageServerRpc(damage);

            
        }
        firing = false;
        current_target = this.gameObject;
    }

    IEnumerator DeleteObj(GameObject obj) 
    {
        yield return new WaitForSeconds(1);
        Destroy(obj);
    }
}

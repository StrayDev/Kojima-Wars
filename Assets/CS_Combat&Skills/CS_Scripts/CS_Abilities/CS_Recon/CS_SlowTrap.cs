using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_SlowTrap : NetworkBehaviour, IDamageable
{
    public float detectionRadius;
    public float slowRadius;
    public float duration;
    public float slowTime;
    public float health;
    public LayerMask enemyMask;
    private float yOffset;
    private SphereCollider sc;

    private bool triggered = false;

    private List<GameObject> players;

    private GameObject player;
    private Camera FPSCamera;
    private Transform cameraTransform;

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        cameraTransform = player.GetComponentInChildren<MechLookPitch>().transform;
        GetFPSCamRecursively(player.transform);

        players = new List<GameObject>();
        yOffset = transform.localScale.y / 2;
        
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = cameraTransform.transform.forward * 5;
        }
        transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset + 0.5f, targetPoint.z);
        StartCoroutine(DestroyObject(duration));
        sc = GetComponent<SphereCollider>();
        sc.radius = detectionRadius;
    }

    void RemoveSlow()
    {
        foreach (GameObject player in players)
        {
            ServerAbilityManager.Instance.SlowEnemyServerRPC(player.GetComponent<NetworkObject>(),
                player.GetComponent<CS_PlayerStats>().speed * 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.GetComponent<Entity>())
        {
            if (!triggered &&
            ServerAbilityManager.Instance.IsEnemy(gameObject, other.GetComponent<Entity>()))
            {
                Debug.Log("IsEnemy1");
                StopAllCoroutines();
                StartCoroutine(DestroyObject(slowTime));
                //Play animation or start particle effect
                sc.radius = slowRadius;
                triggered = true;
            }

            if (ServerAbilityManager.Instance.IsEnemy(gameObject, other.GetComponent<Entity>()))
            {
                Debug.Log("IsEnemy");
                Debug.Log("Player Stats before: " + other.GetComponent<CS_PlayerStats>().speed);
                players.Add(other.gameObject);
                //other.GetComponent<CS_PlayerStats>().speed /= 2;
                Debug.Log("Player Stats after: " + other.GetComponent<CS_PlayerStats>().speed);
                ServerAbilityManager.Instance.SlowEnemyServerRPC(other.GetComponent<NetworkObject>(), 
                    other.GetComponent<CS_PlayerStats>().speed / 2);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        if (other.GetComponent<Entity>())
        {
            if (ServerAbilityManager.Instance.IsEnemy(gameObject, other.GetComponent<Entity>()))
            {
                Debug.Log("LogThree");
                players.Remove(other.gameObject);
                ServerAbilityManager.Instance.SlowEnemyServerRPC(other.GetComponent<NetworkObject>(), 
                    other.GetComponent<CS_PlayerStats>().speed * 2);
            }
        }
        
    }

    IEnumerator DestroyObject(float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveSlow();
        Destroy(gameObject);
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

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (!triggered)
        {
            health -= damage;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int heal)
    {
    }


    public bool IsAlive()
    {
        return health > 0.0f;
    }

}

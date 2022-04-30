using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_Lockdown : NetworkBehaviour, IDamageable
{
    public float radius;
    public float timer;
    private GameObject player;
    public float stunDuration;

    public GameObject bubble;
    public Vector3 targetScale;
    public float bubbleSpeed = 2;
    public float health;

    private Collider[] objectsInRange;

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        StartCoroutine(Countdown(timer));

        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        Vector3 temp = player.transform.position + player.transform.forward * 5;
        //temp.y += gameObject.transform.localScale.y / 2;
        transform.position = temp;

        //targetScale = bubble.transform.localScale;
        //bubble.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (!IsServer) return;
        /*if (bubble.transform.localScale.x < targetScale.x)
        {
            bubble.transform.localScale += new Vector3(1, 1, 1) * bubbleSpeed * Time.deltaTime;
        }*/
        ServerAbilityManager.Instance.LockdownBubbleServerRPC(GetComponent<NetworkObject>(),
            targetScale, bubbleSpeed, Time.deltaTime);
    }

    void LockDown()
    {
        objectsInRange = Physics.OverlapSphere(transform.position, radius);
        foreach (var enemy in objectsInRange)
        {
            if (enemy.GetComponent<IDamageable>() != null && enemy.GetComponent<Entity>() != null &&
                ServerAbilityManager.Instance.IsEnemy(gameObject, enemy.GetComponent<Entity>()))
            {
                ServerAbilityManager.Instance.SlowEnemyServerRPC(enemy.GetComponent<NetworkObject>(),
            enemy.GetComponent<CS_PlayerStats>().speed / 2);
            }
        }
        /*gameObject.GetComponent<MeshCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        bubble.GetComponent<MeshRenderer>().enabled = false;*/
        ServerAbilityManager.Instance.LockdownDisableClientRPC(GetComponent<NetworkObject>());
    }

    IEnumerator Countdown(float time)
    {
        yield return new WaitForSeconds(time);
        LockDown();
        yield return new WaitForSeconds(stunDuration);
        foreach (var enemy in objectsInRange)
        {
            if (enemy.GetComponent<IDamageable>() != null && enemy.GetComponent<Entity>() != null &&
                ServerAbilityManager.Instance.IsEnemy(gameObject, enemy.GetComponent<Entity>()))
            {
                ServerAbilityManager.Instance.SlowEnemyServerRPC(enemy.GetComponent<NetworkObject>(),
            enemy.GetComponent<CS_PlayerStats>().speed * 2);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int heal)
    {
        
    }
}

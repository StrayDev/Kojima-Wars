using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NeuralTheft : NetworkBehaviour
{
    public LayerMask enemyMask;
    [SerializeField] Shader see_through;
    [SerializeField] Shader original;
    List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) 
        {
            return;        
        }
        //this.GetComponent<NetworkObject>().Spawn();
        enemies = new List<GameObject>();
        foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.GetComponent<IDamageable>() != null)
            {
                if (ServerAbilityManager.Instance.IsEnemy(gameObject, obj.GetComponent<Entity>()))
                {
                    enemies.Add(obj);
                }
            }

        }
        StartCoroutine(Reveal(enemies));        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Reveal(List<GameObject> enemies) 
    {
        foreach (GameObject enemy in enemies) 
        {
            ServerAbilityManager.Instance.RevealEnemyServerRPC(ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId, enemy.gameObject.GetComponent<NetworkObject>(), 1);
        }
        yield return new WaitForSeconds(2);
        foreach (GameObject enemy in enemies)
        {
            ServerAbilityManager.Instance.RevealEnemyServerRPC(ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId, enemy.gameObject.GetComponent<NetworkObject>(), 1);
        }
        yield return new WaitForSeconds(2);
        foreach (GameObject enemy in enemies)
        {
            ServerAbilityManager.Instance.RevealEnemyServerRPC(ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId, enemy.gameObject.GetComponent<NetworkObject>(), 1);
        }
    }
}

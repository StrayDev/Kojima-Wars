using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NeuralAdaptation : NetworkBehaviour
{
    public int duration;
    public float speed_increase;
    [SerializeField] Shader through_walls;
    [SerializeField] Shader original;
    public GameObject player;
    public List<GameObject> enemies;
    private CS_PlayerStats player_stats;
    public LayerMask enemy_layer;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) 
        {
            enabled = false;
            return;
        }
        //ServerAbilityManager.Instance.RegisterNewAbilityServerRpc(new NetworkObjectReference(this.GetComponent<NetworkObject>()), ServerAbilityManager.Instance.GetOwner(gameObject).gameObject);
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        player_stats = player.GetComponent<CS_PlayerStats>();
        float base_speed = player_stats.speed;
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
        StartCoroutine(Hunt(enemies, base_speed));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Hunt(List<GameObject> enemies, float base_speed) 
    {
        foreach (GameObject enemy in enemies)
        {
            /*foreach (Transform child in enemy.transform)
            {
                if (child.gameObject.GetComponents<Material>().Length > 0)
                {
                    foreach (Material m in child.gameObject.GetComponents<Material>())
                    {
                        m.shader = through_walls;
                    }
                }
            }*/
            ServerAbilityManager.Instance.RevealEnemyServerRPC(ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId, enemy.gameObject.GetComponent<NetworkObject>(), duration);
        }
        player_stats.speed += speed_increase;
        yield return new WaitForSeconds(duration);
        foreach (GameObject enemy in enemies)
        {
            /*foreach (Transform child in enemy.transform)
            {
                if (child.gameObject.GetComponents<Material>().Length > 0)
                {
                    foreach (Material m in child.gameObject.GetComponents<Material>())
                    {
                        m.shader = original;
                    }
                }
            }*/
        }
        player_stats.speed = base_speed;
        Destroy(this.gameObject);
    }
}

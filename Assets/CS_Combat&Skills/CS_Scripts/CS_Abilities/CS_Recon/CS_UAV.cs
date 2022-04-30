using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

public class CS_UAV : NetworkBehaviour, IDamageable
{

    [SerializeField] private float speed;
    [SerializeField] private float radius;
    [SerializeField] private float height;
    [SerializeField] private int scanInterval;
    [SerializeField] private int scanDuration;
    [SerializeField] private int scanRange;
    [SerializeField] private int lifetime;
    [SerializeField] private int health;


    private bool test;
    [SerializeField] private Vector3 positionOffset;
    private Vector3 middle;

    private GameObject player;
    private float timeCounter;

    //Particles
    /*public ParticleSystem _highDmg;
    public ParticleSystem _midDmg;*/
    /*private float midDmg;
    private float highDmg;*/
    void Start()
    {
        if (!IsServer)
        {
            GetComponent<Collider>().enabled = false;
            enabled = false;
            return;
        }
        
        player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
        positionOffset = player.transform.position;
        middle = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + height, player.transform.localPosition.z);

        /*midDmg = health * 0.80f;
        highDmg = health * 0.50f;*/
       
        /*_midDmg.Stop();
        _highDmg.Stop();*/
        StartCoroutine(DestroyObject());
        StartCoroutine(scanForEnemies());
    }

    void Update()
    {
        if (!IsServer) return;
        
        Movement();
        
        /*if (health <= midDmg && health >= highDmg)
        {
            _midDmg.Play();
            Debug.Log("Mid Damage" + health);
        }

        if (health < highDmg && health > 0) 
        {
            _highDmg.Play();
            Debug.Log("High Damage" + health);
        }*/
        
      
        
    }


    private IEnumerator scanForEnemies()
    {
        yield return new WaitForSeconds(scanInterval);
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            Entity entityHit = player.GetComponent<Entity>();
            if (Vector3.Distance(transform.position, player.transform.position) <= scanRange)
            {
                if (entityHit && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                
                    ServerAbilityManager.Instance.RevealEnemyServerRPC(
                        ServerAbilityManager.Instance.GetOwner(gameObject).gameObject.GetComponent<Entity>().clientId,
                        entityHit.gameObject, scanDuration);
                }
            }

        }
        StartCoroutine(scanForEnemies());

        yield return new WaitForSeconds(scanDuration);


    }

    private void changeMaterialRecurisvely(GameObject gameObject, Shader shader)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.TryGetComponent(out SkinnedMeshRenderer smr))
            {
                if (smr.GetComponent<Renderer>().material.shader != shader)
                {
                    smr.GetComponent<Renderer>().material.shader = shader;
                }
                
            }
            changeMaterialRecurisvely(child.gameObject, shader);
        }
    }
    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }
    private void Movement()
    {
        timeCounter += Time.deltaTime * speed;

        float x = Mathf.Cos(timeCounter) * radius;
        float y = height;
        float z = Mathf.Sin(timeCounter) * radius;

        transform.position = new Vector3(x, y, z) + positionOffset;
        
        transform.LookAt(middle);
        transform.RotateAround(transform.position, transform.up, 180);


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
    
    [ServerRpc(RequireOwnership = false)]
    public void HealDamageServerRpc(int heal)
    {

    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color =new Vector4(0,1,0,0.2f);
        Gizmos.DrawSphere(transform.position /*+ (transform.forward * transform.localScale.z / 2)*/, scanRange);
    }
}

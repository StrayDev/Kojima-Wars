using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : MonoBehaviour
{
    //References
    public Rigidbody rb;
    public GameObject explosion;
    public WeaponStats stats;

    //Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;
    public LayerMask whatIsEnemies;

    //Damage
    public int explosionDamage;
    public float explosionRange;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    private int collisions;
    PhysicMaterial physics_mat;
    private bool hasExploded;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //When to explode:
        if (collisions > maxCollisions) Explode();

        //Countdown lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        Vector3 position = transform.position;
        if (explosion != null && !hasExploded)
        {
            Instantiate(explosion, position, Quaternion.identity);
            hasExploded = true;
        }
        

        //Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<IDamageable>()?.TakeDamageServerRpc(explosionDamage);
        }

        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Count up collisions
        collisions++;

        if (collision.collider.CompareTag("Player") && explodeOnTouch) Explode();
    }

    private void Setup()
    {
        //New Physics Mat
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set Gravity
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}

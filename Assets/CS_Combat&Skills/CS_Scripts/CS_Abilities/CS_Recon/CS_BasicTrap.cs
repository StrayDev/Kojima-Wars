using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CS_BasicTrap : NetworkBehaviour
{
    [SerializeField]
    private float detectionRadius;
    private float duration;
    private float health;
    public LayerMask enemyMask;

    public GameObject lineRendererObject;
    private List<LineRenderer> lines;

    private float yOffset;
    private GameObject player;
    private Camera FPSCam;

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

        lines = new List<LineRenderer>();

        yOffset = transform.localScale.y / 2;
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = cameraTransform.transform.forward * 5;
        }

        transform.position = new Vector3(targetPoint.x, targetPoint.y + yOffset, targetPoint.z);
    }

    private void Update()
    {
        if (!IsServer) return;

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);
        List<Collider> listenemies = new List<Collider>();
        //Debug.Log("List count: " + lines.Count + "   Array count: " + objectsInRange.Length);
        foreach (var enemy in objectsInRange)
        {
            if (enemy.GetComponent<IDamageable>() != null && 
                ServerAbilityManager.Instance.IsEnemy(gameObject, enemy.GetComponent<Entity>()))
            {
                listenemies.Add(enemy);
                Debug.Log(enemy.name);
            }
            
            //Reveal on minimap
        }

        if (lines.Count < listenemies.Count)
        {
            int numtoadd = listenemies.Count - lines.Count;
            CreateLine(numtoadd);
        }
        else if (lines.Count > listenemies.Count)
        {
            int numtoremove = lines.Count - listenemies.Count;
            RemoveLine(numtoremove);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].SetPosition(0, gameObject.transform.position);
            lines[i].SetPosition(1, listenemies[i].transform.position);
        }
    }

    void CreateLine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newLine = Instantiate(lineRendererObject);
            newLine.GetComponent<NetworkObject>().Spawn();
            newLine.GetComponent<NetworkObject>().TrySetParent(this.transform);
            newLine.GetComponent<LineRenderer>().positionCount = 2;
            lines.Add(newLine.GetComponent<LineRenderer>());
        }
    }

    void RemoveLine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            lines.RemoveAt(0);
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }

    private void GetFPSCamRecursively(Transform obj)
    {
        //Get HUD recursively in children
        foreach (Transform child in obj)
        {
            if (child.TryGetComponent(out Camera camera))
            {
                FPSCam = camera;
            }
            else
            {
                GetFPSCamRecursively(child);
            }
        }
    }
}

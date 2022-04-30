using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AINavMesh : MonoBehaviour
{
    // so we can set it in the editor
    public GameObject moveDestination;

    private GameObject target;

    // reference to the nav mesh agent attached to the Ai in this case
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (moveDestination == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit , 100.0f))
            {
                if(hit.transform.tag == "ClickAbleObjects")
                {
                    Debug.Log(hit.transform.name);
                    moveDestination.transform.position = hit.transform.position;
                    navMeshAgent.destination = moveDestination.transform.position;
                }
                else
                {
                    Debug.Log("Click Not Valid");
                }
                   
            }
        }
        navMeshAgent.destination = moveDestination.transform.position;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AIMovement : NetworkBehaviour
{
    GameObject[] nodes;

    AIFindPath AIHandler;

    GameObject moveto;
    public Vector3 move_position;
    public bool reverseSearch = false;

    public Vector3 navmove;

    public bool moving = false;
    public bool final_move = false;
    public  bool pause_move = false;

    // reference to the nav mesh agent attached to the Ai in this case
    private NavMeshAgent navMeshAgent;
    public GameObject moveDestination;

    public List<GameObject> movePath = new List<GameObject>();
    int current_path = 0;

    public UnitDefenceLocation defenceLocation;

    private void Awake()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].GetComponent<Node>().index = i;
        }
        AIHandler = GameObject.FindGameObjectWithTag("AIHandler").GetComponent<AIFindPath>();
        moveto = GameObject.Find("MoveTo");


        
    }

    // Update is called once per frame
    void Update()
    {
                /*
        if (move_position != null)
        {
            moveto.transform.position = move_position;
        }

        if (Mouse.current.leftButton.IsPressed())
        {
            RaycastHit hit;
            UnityEngine.Debug.Log("Clicked");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, 100000.0f))
            {
                UnityEngine.Debug.Log(hit.transform.name);
                moveto.transform.position = hit.point;

                movePath = AIHandler.findPath(findClosestNode(this.gameObject.transform.position), findClosestNode(move_position), false);
                if (movePath.Count > 0)
                {
                    current_path = 0;
                    moving = true;
                    final_move = false;
                }
            }
        }
        */

        if (IsOwnedByServer)
        {
            if (!pause_move)
            {
                if (final_move)
                {
                    navMeshAgent.SetDestination(move_position);
                    if (Vector2.Distance(new Vector2(moveDestination.transform.position.x, moveDestination.transform.position.z), new Vector2(transform.position.x, transform.position.z)) <= 2)
                    {
                        final_move = false;
                    }
                }
                else if (moving)
                {
                    moveDestination = movePath[current_path];
                    if (Vector2.Distance(new Vector2(movePath[current_path].transform.position.x, movePath[current_path].transform.position.z), new Vector2(transform.position.x, transform.position.z)) <= 2)
                    {
                        moveDestination = movePath[current_path];

                        current_path++;
                        if (current_path >= movePath.Count)
                        {
                            current_path = 0;
                            moving = false;
                            final_move = true;
                        }
                    }
                    navMeshAgent.SetDestination(moveDestination.transform.position);

                }
            }
        }
        
    }

    public void move(Vector3 _moveto)
    {
        move_position = _moveto;
        movePath = AIHandler.findPath(findClosestNode(this.gameObject.transform.position), findClosestNode(move_position), false);
        if (movePath.Count > 0)
        {
            current_path = 0;
            moving = true;
            final_move = false;
        }
    }

    int findClosestNode(Vector3 transform)
    {
        float distance = 9999999;
        int index = 0;
        for (int i = 0; i < nodes.Length; ++i)
        {
            float dist = (nodes[i].transform.position - transform).magnitude;
            if (dist < distance)
            {
                distance = dist;
                index = i;
            }
        }
        return index;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : MonoBehaviour
{
    public GameObject[] neighbours;
    public List<Tuple<GameObject, float>> distances;
    public int index = 0;
    public float radius = 50;


    // Start is called before the first frame update
    void Start()
    {
        //FindNextNode();
        distances = new List<Tuple<GameObject, float>>();
        for (int i = 0; i < neighbours.Length; ++i)
        {
            distances.Add(new Tuple<GameObject,float>(neighbours[i],(neighbours[i].transform.position - this.transform.position).magnitude));
        }
    }

   public void FindNextNode()
    {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, radius);
        var list = new List<GameObject>();
        foreach (var col in cols)
        {
            if(col.tag == "Node")
            {
                RaycastHit hit;
                Vector3 dir = (col.transform.position - this.transform.position).normalized;

                if(Physics.Raycast(this.transform.position,dir,out hit, radius))
                {
                    if(hit.collider == col)
                    {
                        list.Add(hit.collider.gameObject);
                    }
                }
            }
        }
        neighbours = new GameObject[list.Count];
        int i = 0;
        foreach (var item in list)
        {
            neighbours[i] = item;
            i++;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

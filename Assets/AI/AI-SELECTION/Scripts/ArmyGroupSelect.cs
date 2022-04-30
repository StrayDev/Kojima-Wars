using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyGroupSelect : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] SelectAI()
    {
        
        int total_children = this.transform.childCount;
        GameObject[] objects = new GameObject[total_children];
        for (int i = 0; i < total_children; ++i)
        {
            objects[i] = this.transform.GetChild(i).gameObject;
        }

        return objects;
    }
}

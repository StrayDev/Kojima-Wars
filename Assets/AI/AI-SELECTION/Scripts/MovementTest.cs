using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float speed = 1.0f;

    void Start()
    {
        
    }

    void Update()
    {
        
        float step =  speed * Time.deltaTime; 
        transform.position = Vector3.MoveTowards(transform.position, transform.parent.position , step);

    }
}


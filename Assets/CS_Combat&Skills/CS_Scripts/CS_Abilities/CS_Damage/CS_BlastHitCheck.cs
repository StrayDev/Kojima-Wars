using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_BlastHitCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemy") 
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Plane"))
            {
                Debug.Log("Hit enemy plane");
            }
            else 
            {
                Debug.Log("Hit enemy mech");            
            }
        }
    }
}

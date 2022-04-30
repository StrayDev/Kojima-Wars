using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempMovement : MonoBehaviour
{

    float moveSpeed = 10.0F;
    Rigidbody rb;
    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current[Key.A].wasPressedThisFrame)
        {
            left = true;
            right = false;
            
        }
        else if (Keyboard.current[Key.D].wasPressedThisFrame)
        {
            left = false;
            right = true;
            
        }
        if (Keyboard.current[Key.S].wasPressedThisFrame)
        {
            up = true;
            down = false;
            
        }
        else if (Keyboard.current[Key.W].wasPressedThisFrame)
        {
            up = false;
            down = true;
            
        }

        if(Keyboard.current[Key.A].wasReleasedThisFrame)
        {
            left = false;
        }
        if (Keyboard.current[Key.D].wasReleasedThisFrame)
        {
            right = false;
        }
        if (Keyboard.current[Key.S].wasReleasedThisFrame)
        {
            up = false;
        }
        if (Keyboard.current[Key.W].wasReleasedThisFrame)
        {
            down = false;
        }

        if (left)
        {
            transform.position = (new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z));
        }
        else if(right)
        {
            transform.position = (new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z));
        }
        if(up)
        {
            transform.position = (new Vector3(transform.position.x, transform.position.y, transform.position.z + moveSpeed * Time.deltaTime));
        }
        else if (down)
        {
            transform.position = (new Vector3(transform.position.x, transform.position.y, transform.position.z - moveSpeed * Time.deltaTime));
        }


    }
}

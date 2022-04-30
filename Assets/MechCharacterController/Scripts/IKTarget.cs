using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDistanceFade : MonoBehaviour
{
    [Serializable]
    public struct ObjectFadeDistance
    {
        public GameObject element;
        public float maxDistanceFromCamera;
    }

    [SerializeField] private ObjectFadeDistance[] objects;
    private void Update()
    {
        Camera camera = Camera.main;
        foreach (ObjectFadeDistance o in objects)
        {
            o.element.SetActive(Vector3.Distance(o.element.transform.position, camera.transform.position) < o.maxDistanceFromCamera);
        }
    }
}

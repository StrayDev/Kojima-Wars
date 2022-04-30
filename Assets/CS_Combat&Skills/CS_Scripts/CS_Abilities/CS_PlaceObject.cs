using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PlaceObject : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Camera cam;
    public GameObject firingpoint;

    public GameObject objectToSpawn;
    public float range;
    public LayerMask objectLayer;

    public bool isOn = false;

    private RaycastHit rayHit;

    GameObject obj;
    float yoffset;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, cam.gameObject.transform.position);
    }

    private void LateUpdate()
    {
        if (isOn)
        {
            lineRenderer.enabled = true;
            if (obj == null)
            {
                obj = Instantiate(objectToSpawn);
                yoffset = obj.transform.localScale.y / 2;
            }
            
            Vector3 direction = cam.transform.forward;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out rayHit, range))
            {
                if (rayHit.collider.gameObject != obj)
                {
                    targetPoint = rayHit.point;
                }
                else
                {
                    targetPoint = ray.GetPoint(range);
                }
                
            }
            else
            {
                targetPoint = ray.GetPoint(range);
            }

            lineRenderer.SetPosition(1, targetPoint);
            lineRenderer.SetPosition(0, firingpoint.transform.position);
            obj.transform.position = targetPoint;
        }
        else
        {
            lineRenderer.enabled = false;
            if (obj != null) Destroy(obj.gameObject);
        }
    }
}

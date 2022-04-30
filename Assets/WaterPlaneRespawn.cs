using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlaneRespawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Attempt to get the mech character controller from the other object
        MechCharacterController mechCharacterController = other.gameObject.GetComponent<MechCharacterController>();

        // If the mech character controller is valid then a mech has entered the water plane
        if(mechCharacterController != null)
        {
            // Force the mech to respawn
            mechCharacterController.KillMech();
        }
    }
}

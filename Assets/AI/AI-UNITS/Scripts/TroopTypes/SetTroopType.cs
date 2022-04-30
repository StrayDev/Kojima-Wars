using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetTroopType : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

  public enum TroopTypes
  {
        // INFANTRY
        Standard_Infantry,
        Anti_Air_Infantry,
        Anti_Tank_Infantry,

        // LAND VEHICLE
        Standard_Tank,
        Anti_Air_Truck,
        Heavy_Tank,

        // AIR CRATFS
        Standard_Helicopter,
        Scout_Helicopter
  }

    public TroopTypes troopTypes;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(troopTypes)
        {
            case TroopTypes.Standard_Infantry:
                //Debug.Log("Standard Infantry");
                StandardInfantry();
                break;
            case TroopTypes.Anti_Air_Infantry:
                //Debug.Log("Anti Air Infantry");
                AntiAirInfantry();
                break;
            case TroopTypes.Anti_Tank_Infantry:
                //Debug.Log("Anti Tank Infantry");
                AntiTankInfantry();
                break;
            case TroopTypes.Standard_Tank:
                //Debug.Log("Standard Tank");
                StandardTank();
                break;
            case TroopTypes.Anti_Air_Truck:
                //Debug.Log("Anti Air Truck");
                AntiAirTruck();
                break;
            case TroopTypes.Heavy_Tank:
                //Debug.Log("Heavy Tank");
                HeavyTank();
                break;
            case TroopTypes.Standard_Helicopter:
                //Debug.Log("Standard Helicopter");
                StandardHelicopter();
                break;
            case TroopTypes.Scout_Helicopter:
                //Debug.Log("Scout Helicopter");
                ScoutHelicopter();
                break;





        }
    }
    void StandardInfantry()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 0.5F;
        navMeshAgent.height = 1.0f;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void AntiAirInfantry()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 0.5F;
        navMeshAgent.height = 1.0f;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void AntiTankInfantry()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 0.5F;
        navMeshAgent.height = 1.0f;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void StandardTank()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 3.0F;
        navMeshAgent.height = 2.0F;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void AntiAirTruck()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 3.0F;
        navMeshAgent.height = 2.0F;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void HeavyTank()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 3.0F;
        navMeshAgent.height = 2.0F;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void StandardHelicopter()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 0.5F;
        navMeshAgent.height = 1.0f;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
    void ScoutHelicopter()
    {
        // STEERING
        navMeshAgent.speed = 10.0F;
        navMeshAgent.angularSpeed = 120.0F;
        navMeshAgent.acceleration = 8.0F;
        navMeshAgent.stoppingDistance = 0.0F;
        navMeshAgent.autoBraking = true;

        // OBSTACLE AVOIDANCE
        navMeshAgent.radius = 0.5F;
        navMeshAgent.height = 1.0f;
        navMeshAgent.avoidancePriority = 50;

        // PATH FINDING

        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.autoRepath = true;
    }
}

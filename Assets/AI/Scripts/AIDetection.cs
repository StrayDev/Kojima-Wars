using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AI_AgentController))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(NetworkObject))]
public class AIDetection : NetworkBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [SerializeField]
    private GameTeamData teamData;

    public bool has_target = false;
    GameObject target = null;
    public float check_targets_timeout = 3; // how long between checking if the target is still in view/ range and then attack target
    float check_targets_timer = 0;

    public List<GameObject> possible_targets;
    public List<GameObject> in_range_targets;

    LayerMask ignoreMask;

    private SphereCollider detection_sphere = null;
    private AI_AgentController agentController = null;
    private Entity entityComponent = null;

    public override void OnNetworkSpawn()
    {
        detection_sphere = GetComponent<SphereCollider>();
        agentController = GetComponent<AI_AgentController>();
        entityComponent = GetComponent<Entity>();

        possible_targets = new List<GameObject>();
        in_range_targets = new List<GameObject>();

        // Assuming team 0 in the team data SO is the red team
        ignoreMask = (entityComponent.TeamName == teamData.GetTeamDataAtIndex(0).TeamName) ? LayerMask.GetMask("AI_RED") : LayerMask.GetMask("AI_BLUE");

        setDetectionRadius(30);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHost) return;
        check_targets_timer += Time.deltaTime;

        if (has_target && target)
        {
            Vector3 dir = (target.transform.position - this.transform.position).normalized;
            dir.y = 0.0f;

            if (Vector3.Angle(transform.forward, dir) > 1.0f)
            {
                RotateToTarget(dir);
            }
        }
        if (check_targets_timer >= check_targets_timeout)
        {
            check_targets_timer = 0;
            if (in_range_targets.Count > 0)
            {
                possible_targets.Clear();
                List<GameObject> itemsToRemove;
                itemsToRemove = new List<GameObject>();
                foreach (var item in in_range_targets)
                {
                    if (item == null || item.GetComponent<IDamageable>().IsAlive() == false)
                    {
                        itemsToRemove.Add(item);
                        continue;
                    }
                    RaycastHit hit;

                    Vector3 dir = (item.transform.position - this.transform.position).normalized;

                    if (Physics.Raycast(this.transform.position, dir, out hit, detection_sphere.radius, ~ignoreMask))
                    {
                        if (hit.collider.gameObject == item.gameObject)
                        {
                            Debug.DrawRay(transform.position, dir * hit.distance, Color.green, 1f);
                            possible_targets.Add(item);
                        }
                    }
                }
                foreach (var item in itemsToRemove)
                {
                    in_range_targets.Remove(item);
                }

                if (has_target && target != null && possible_targets.Contains(target))
                {
                    attackTarget();

                }
                else
                {
                    pickTarget();
                    if (has_target)
                    {
                        attackTarget();
                    }
                }
            }
        }
        if (has_target)
        {
            agentController.StopNavMeshAgentMovementServerRpc();
        }
        else
        {
            agentController.ResumeNavMeshAgentMovementServerRpc();
        }
    }
    void attackTarget()
    {
        RaycastHit hit;

        Vector3 dir = (target.transform.position - this.transform.position).normalized;
        dir.y = 0.0f;

        if (Vector3.Angle(transform.forward, dir) <= 10.0f)
        {
            if (Physics.Raycast(this.transform.position, dir, out hit, detection_sphere.radius, ~ignoreMask))
            {
                if (hit.collider.gameObject == target.gameObject)
                {
                    Debug.DrawRay(transform.position, dir * hit.distance, Color.red, 2f);
                    ServerAbilityManager.Instance.HandleMechWeaponFireServerRpc(transform.position, transform.position, dir, agentController.GetDamageStrength(), new Unity.Netcode.NetworkObjectReference(gameObject), new Unity.Netcode.NetworkObjectReference(gameObject));
                    if (target.GetComponent<IDamageable>().IsAlive() == false)
                    {
                        possible_targets.Remove(target);
                        in_range_targets.Remove(target);
                        //Destroy(target);
                        has_target = false;
                        pickTarget();
                    }
                }
            }
        }
    }

    void RotateToTarget(Vector3 dir)
    {
        Vector3 lookTowards = Vector3.RotateTowards(transform.forward, dir, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(lookTowards, Vector3.up);
    }

    public void setDetectionRadius(float radii)
    {
        detection_sphere.radius = radii;
    }

    public float getDetectionRadius()
    {
        return detection_sphere.radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherEntityComponent = other.gameObject.GetComponent<Entity>();
        if (otherEntityComponent != null)
        {
            if (otherEntityComponent.TeamName != entityComponent.TeamName)
            {
                in_range_targets.Add(other.gameObject);
                RaycastHit hit;

                Vector3 dir = (other.transform.position - this.transform.position).normalized;

                if (Physics.Raycast(this.transform.position, dir, out hit, detection_sphere.radius, ~ignoreMask))
                {
                    if (hit.collider.gameObject == other.gameObject)
                    {
                        Debug.DrawRay(transform.position, dir * hit.distance, Color.red, 1f);
                        if (!has_target)
                        {

                            target = other.gameObject;
                            has_target = true;
                            agentController.StopNavMeshAgentMovementServerRpc();
                        }
                    }
                }
            }

        }
    }

    private void pickTarget()
    {
        if (possible_targets.Count > 0)
        {
            target = possible_targets[Random.Range(0, possible_targets.Count - 1)];
            has_target = true;
        }
        else
        {
            target = null;
            has_target = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var script = other.gameObject.GetComponent<BaseUnitClass>();
        if (script != null)
        {
            if (script.team != this.GetComponent<BaseUnitClass>().team)
            {
                if (has_target)
                {
                    if (other.gameObject == target)
                    {
                        if (possible_targets.Count < 1)
                        {
                            // move towards the last known position
                            agentController.SetNavMeshAgentDestinationServerRpc(other.transform.position);
                        }
                        else
                        {
                            pickTarget();
                        }
                    }
                }
                in_range_targets.Remove(other.gameObject);
            }
        }
    }

}

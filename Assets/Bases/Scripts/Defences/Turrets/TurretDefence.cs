using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurretDefence : BaseDefence
{
    [Header("Turret Properties")]
    [SerializeField] protected ETurretDamageType m_fireType = ETurretDamageType.BASIC;
    [Tooltip("The delay in seconds between each firing of the turret.")]
    [SerializeField] protected float m_fireDelay = 0.0f;

    [Header("Projectile References")]
    [SerializeField] protected Transform m_spawnPoint;
    [SerializeField] protected TurretProjectile m_projectilePrefab = null;

    [Header("Look At References")]
    [SerializeField] protected Transform m_lookAtTransform;
    [SerializeField] private bool m_lookAtXAxis;
    [SerializeField] private bool m_lookAtYAxis;
    [SerializeField] private bool m_lookAtZAxis;

    protected float m_fireTimer = 0.0f;
    private Vector3 fireDirection = Vector3.zero;
    private bool shouldFire = false;
    protected List<Entity> m_entitiesInRange = new List<Entity>();

    protected Entity m_target;

    protected virtual void Update()
    {
        if (!IsServer) return;

        CheckTargetFire();

        Entity closestEntity = null;
        float closestEntityDistance = 0;
        
        foreach (Entity entity in m_entitiesInRange)
        {
            float distance = Vector3.Distance(transform.position, entity.transform.position);
            if (closestEntity == null || distance < closestEntityDistance)
            {
                closestEntity = entity;
                closestEntityDistance = distance;
            }
        }
        
        m_target = closestEntity;

        UpdateTurretDefenceServerRpc();
    }

    protected virtual void CheckTargetFire()
    {
        if (m_target != null)
        {
            LookToTarget();

            m_fireTimer += Time.deltaTime;
            if (m_fireTimer >= m_fireDelay)
            {
                m_fireTimer = 0;
                CheckFire();
            }
        }
    }

    [ServerRpc]
    protected virtual void UpdateTurretDefenceServerRpc()
    {
        UpdateTurretDefenceClientRpc(m_lookAtTransform.localRotation, shouldFire, fireDirection);
        shouldFire = false;
    }

    [ClientRpc]
    protected virtual void UpdateTurretDefenceClientRpc(Quaternion turretLookAtRotation, bool _shouldFire, Vector3 _fireDirection)
    {
        m_lookAtTransform.localRotation = turretLookAtRotation;

        if (_shouldFire)
        {
            Fire(_fireDirection);
        }
    }

    protected virtual void CheckFire()
    {
        fireDirection = m_target.m_collider.bounds.center - m_spawnPoint.position;
        shouldFire = true;
    }

    protected virtual void Fire(Vector3 _fireDirection)
    {
        TurretProjectile projectile = Instantiate(m_projectilePrefab, m_spawnPoint.position, Quaternion.identity);
        projectile.SetDirection(_fireDirection);
    }

    protected virtual void LookToTarget()
    {
        var targetPosition = new Vector3(
            m_lookAtXAxis ? m_target.m_collider.bounds.center.x : transform.position.x,
            m_lookAtYAxis ? m_target.m_collider.bounds.center.y : transform.position.y,
            m_lookAtZAxis ? m_target.m_collider.bounds.center.z : transform.position.z
        );

        m_lookAtTransform.LookAt(targetPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Entity entity = other.gameObject.GetComponent<Entity>();

        if (entity == null)
        {
            return;
        }

        if (entity.TeamName != m_baseController.TeamOwner)
        {
            m_entitiesInRange.Add(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        Entity entity = other.gameObject.GetComponent<Entity>();

        if (entity == null)
        {
            return;
        }

        m_entitiesInRange.Remove(entity);
    }
}
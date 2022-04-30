using UnityEngine;

public class ReloadingTurret : TurretDefence
{
    [Header("Reloading References")]
    [SerializeField] GameObject m_reloadParticleSystem = default;
    [SerializeField] private float m_reloadDelay = 0.0f;

    private bool m_isReloading = false;
    private float m_reloadTimer = 0.0f;

    protected override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        if (!IsServer) return;

        m_isReloading = true;
        m_reloadParticleSystem.SetActive(false);
    }

    protected override void CheckTargetFire()
    {
        if (m_target != null)
        {
            LookToTarget();

            if (m_isReloading)
            {
                m_reloadTimer += Time.deltaTime;
                if (m_reloadTimer > m_reloadDelay)
                {
                    m_fireTimer = 0;
                    m_isReloading = false;
                    m_reloadParticleSystem.SetActive(true);
                }
            }
            else
            {
                m_fireTimer += Time.deltaTime;
                if (m_fireTimer >= m_fireDelay)
                {
                    m_fireTimer = 0;
                    CheckFire();
                }
            }
        }
    }
}
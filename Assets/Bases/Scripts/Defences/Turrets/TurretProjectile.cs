using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] private float m_speed = 5.0f;
    [SerializeField] private float m_maxLifeTime = 5.0f;

    private float m_lifeTime = 0.0f;
    private Vector3 m_direction;

    public void SetDirection(Vector3 direction)
    {
        m_direction = direction;
    }

    void FixedUpdate()
    {
        Vector3 position = gameObject.transform.position;
        position += m_direction * m_speed * Time.deltaTime;
        gameObject.transform.position = position;

        m_lifeTime += Time.deltaTime;
        if (m_lifeTime > m_maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}
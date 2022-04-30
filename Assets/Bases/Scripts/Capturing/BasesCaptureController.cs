using System.Collections;
using UnityEngine;

public class BasesCaptureController : MonoBehaviour
{
    #region Singleton Setup
    private BasesCaptureController()
    {

    }

    public static BasesCaptureController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            SetUp();
        }
    }

    #endregion

    public float CaptureMultiplyer { get; private set; } = 2;

    [Tooltip("The starting multiplyer for capturing bases. Higher means faster captures.")]
    [SerializeField] private float m_startCaptureMultiplyer = 2.5f;

    [Tooltip("The time in seconds until the capture multiplyer is set to 1")]
    [SerializeField] private float m_timeToBaseCaptureMultiplyer = 30.0f;

    void Start()
    {
        StartTimer();
    }

    void SetUp()
    {
        CaptureMultiplyer = m_startCaptureMultiplyer;
    }

    void StartTimer()
    {
        StartCoroutine(Countdown(m_timeToBaseCaptureMultiplyer));
    }

    void SetCaptureMultiplyer()
    {
        CaptureMultiplyer = 1.0f;
    }

    private IEnumerator Countdown(float duration)
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        SetCaptureMultiplyer();
    }
}
using System;
using System.Collections;
using UnityEngine;

public class Counter
{
    public enum TimerType
    {
        SECONDS = 0,
        MINUTES = 1
	}

    public event Action OnComplete;
    public event Action OnChanged;

    public bool IsComplete => CurrentTime >= m_timeLimitInSeconds;
    public float PercentageComplete => CurrentTime / (float)m_timeLimitInSeconds;
    public float TimeRemaining => m_timeLimitInSeconds - CurrentTime;

    public bool IsCounting { get; set; } = false;

    public float CurrentTime { get; private set; } = 0.0f;

    private const float WAIT_TIME_IN_SECONDS = 0.1f;

    private float m_timeLimitInSeconds = 1.0f;

    private MonoBehaviour m_parent;

    public Counter(MonoBehaviour parent, float _timeLimit, TimerType _type)
    {
        m_parent = parent;

        if (_type == TimerType.MINUTES)
        {
            _timeLimit *= 60;
        }

        m_timeLimitInSeconds = _timeLimit;
	}

    IEnumerator CountDown(float _wait)
    {
        while (CurrentTime < m_timeLimitInSeconds)
        {
            CurrentTime += _wait;
            OnChanged?.Invoke();

            if (CurrentTime > m_timeLimitInSeconds)
            {
                OnComplete?.Invoke();
            }

            yield return new WaitForSeconds(_wait);
        }
    }

    public void SetTimeLimit(float _time) 
    { 
        m_timeLimitInSeconds = _time; 
    }

    /// <summary>
    /// Sets the timer to 0 then starts.
    /// </summary>
    public void StartTimer()
    {
        CurrentTime = 0.0f;
        Resume();
    }
    
    /// <summary>
    /// Carrys on counting from previoys time.
    /// </summary>
    public void Resume()
    {
        IsCounting = true;
        m_parent.StartCoroutine(CountDown(WAIT_TIME_IN_SECONDS));
    }

    /// <summary>
    /// Haults timer.
    /// </summary>
    public void Pause()
    {
        m_parent.StopCoroutine(CountDown(WAIT_TIME_IN_SECONDS));
        IsCounting = false;
	}

    /// <summary>
    /// Timer is reset to 0.
    /// </summary>
    public void ResetTimer()
    {
        CurrentTime = 0.0f;
    }
    
    /// <summary>
    /// Timer is reset to 0 and stopped.
    /// </summary>
    public void Stop()
    {
        Pause();
        ResetTimer();
    }
}
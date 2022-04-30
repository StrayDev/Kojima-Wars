using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText = default;

    public void UpdateTime(string timeString)
    {
        m_timerText.text = timeString;
    }
}
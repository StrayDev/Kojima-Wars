using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreObject : MonoBehaviour
{
    [SerializeField] private Image m_fillImage = default;
    [SerializeField] private TextMeshProUGUI m_scoreText = default;

    public void SetTeamColour(Color colour)
    {
        m_fillImage.color = colour;
    }
    
    public void SetTeamScoreText(int score, int maxScore)
    {
        m_scoreText.text = score.ToString();
        
        var percentage = (float)score / (float)maxScore;
        m_fillImage.fillAmount = percentage;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image m_backImage;
    [SerializeField] private Image m_fillImage;
    [SerializeField] private Image m_fillColour;
    
    public void SetAbilityImage(Sprite sprite)
    {
        if (sprite != null)
        {
            m_backImage.sprite = sprite; 
            m_fillImage.sprite = sprite;
            
            m_fillColour.fillAmount = 1;
            m_fillImage.fillAmount = 1;
        }
        else
        {
            m_backImage.sprite = null;
            m_fillImage.sprite = null;

            m_fillColour.fillAmount = 0;
            m_fillImage.fillAmount = 0;
        }
    }

    public void SetIconColour(Color colour)
    {
        m_fillColour.color = colour;
    }
}

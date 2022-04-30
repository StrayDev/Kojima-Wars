using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonUI : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Settings"), Space(5.0f)] 
    [SerializeField][CanBeNull] private GameObject m_buttonSelectionImage;
    [SerializeField] private Color m_buttonSelectionColour;
    [SerializeField] private TextMeshProUGUI m_buttonText;
    [SerializeField] private Color m_buttonColour;
    [SerializeField] private bool m_useSelectionImage;
    
    private Color m_textDefaultColour;
    private Button m_button;

    public void Start()
    {
        m_button = GetComponent<Button>();
        m_textDefaultColour = m_buttonText.color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_buttonText.color = m_buttonColour;
        
        if (m_useSelectionImage)
        {
            m_buttonSelectionImage.GetComponent<Image>().color = m_buttonSelectionColour;
            m_buttonSelectionImage.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_buttonText.color = m_textDefaultColour;
        
        if (m_useSelectionImage)
        {
            m_buttonSelectionImage.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_buttonText.color = m_textDefaultColour;

        if (m_useSelectionImage)
        {
            m_buttonSelectionImage.SetActive(false);
        }
        
        // if select then do something
    }
}

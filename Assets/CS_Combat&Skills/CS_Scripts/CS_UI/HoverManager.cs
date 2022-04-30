using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverManager : MonoBehaviour
{
    public TextMeshProUGUI tipBodyText;
    public TextMeshProUGUI tipTitleText;
    public RectTransform tipWindow;

    public static Action<string, string, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;

    private void OnEnable()
    {
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }

    private void Start()
    {
        HideTip();
    }

    private void ShowTip(string tipTitle, string tipText, Vector2 mousePos)
    {
        tipTitleText.text = tipTitle;
        tipBodyText.text = tipText;

        float width = tipBodyText.preferredWidth + 10;
        float height = tipTitleText.preferredHeight + tipBodyText.preferredHeight + 10;

        tipWindow.sizeDelta = new Vector2(width, height);
        tipWindow.gameObject.SetActive(true);
        tipWindow.transform.position = new Vector2(mousePos.x + tipWindow.sizeDelta.x - 350 / 2, mousePos.y);
    }

    private void HideTip()
    {
        tipTitleText.text = default;
        tipBodyText.text = default;
        
        tipWindow.gameObject.SetActive(false);
    }
}

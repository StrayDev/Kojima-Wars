using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrencyUI : MonoBehaviour
{
    [Header("UI References")] 
    public TextMeshProUGUI currencyText;
    public Image currencyImage;

    private int _currencyAmount = 0;

    private void OnEnable()
    {
        PlayerInformation.CurrencyUpdated += OnCurrencyUpdate;
    }
    private void OnDisable()
    {
        PlayerInformation.CurrencyUpdated -= OnCurrencyUpdate;
    }

    public void OnCurrencyUpdate(int currencyAmount)
    {
        _currencyAmount = currencyAmount;
        currencyText.text = _currencyAmount.ToString("#");
    }
}

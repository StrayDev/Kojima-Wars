using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_AbilityUI : MonoBehaviour
{
    public Image loadingBar;
    public float currentAmount;
    public float maxTime;

    private void Update()
    {
        loadingBar.fillAmount = currentAmount / maxTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{
    [SerializeField] private PlayerInformationSO scriptableTest;
    [SerializeField] private TextMeshProUGUI nameText;

    private string m_name;
    private void OnEnable()
    {
        PlayerInformation.OnPlayerNameSet += SetPlayerName;
    }
    
    private void OnDisable()
    {
        PlayerInformation.OnPlayerNameSet -= SetPlayerName;
    }

    public void SetPlayerName(string name)
    {
        m_name = name;
        nameText.text = m_name.ToUpper();
    }

    
}

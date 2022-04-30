using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassSelectionTabManager : MonoBehaviour
{

    [SerializeField] private List<Button> buttons = default;

    private Button selectedButton = default;

    private void Awake()
    {
        if (!HasValidRefrences()) return;
        selectedButton = buttons[0];

        foreach(var b in buttons)
        {
            b.onClick.AddListener(() => { OnButtonSelected(b); });
        }
    }

    private void OnEnable()
    {
        var button = selectedButton;
        button.Select();
    }

    private void OnButtonSelected(Button button)
    {
        selectedButton = button;
    }

    private bool HasValidRefrences()
    {
        if (buttons.Count < 1)
        {
            Debug.LogError("No Buttons Assigned to List", this);
            return false;
        }
        return true;
    }

}

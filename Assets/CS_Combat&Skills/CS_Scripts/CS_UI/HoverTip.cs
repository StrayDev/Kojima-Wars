using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tipTitleText;
    [TextArea] public string tipText;
    private float timeToWait = 0.2f;

    public InputActionManager inputActions;
    Vector2 mouseInput;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HoverManager.OnMouseLoseFocus();
    }

    void MousePos(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }

    private void ShowMessage()
    {
        HoverManager.OnMouseHover(tipTitleText, tipText, Mouse.current.position.ReadValue());
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
    }
}

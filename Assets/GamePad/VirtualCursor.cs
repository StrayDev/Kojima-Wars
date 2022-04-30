using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class VirtualCursor : MonoBehaviour
{
    private Mouse virtualMouse;
    [SerializeField]
    private RectTransform cursorTransform;

    private Vector2 _inputVector = default;

    private float cursorSpeed = 1000f;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasRectTransform;

    private Vector2 padding;

    private void Start()
    {
        padding = this.gameObject.GetComponent<RectTransform>().sizeDelta / 2;

        CursorManager.SetVirtualCursor(this.gameObject);
    }

    private void OnEnable()
    {
        InputManager.UI.VirtualCursorPosition.performed += OnReadNavigationInput;
        InputManager.UI.VirtualCursorPosition.canceled  += OnReadNavigationInput;
        InputManager.UI.Click.performed += OnReadClickInput;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("virtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse);

        if(cursorTransform != null)
        {
            Vector2 positon = new Vector2(Screen.width / 2, Screen.height / 2);
            InputState.Change(virtualMouse.position, positon);
        }
    }

    private void OnDisable()
    {
        if(virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        InputManager.UI.VirtualCursorPosition.performed -= OnReadNavigationInput;
        InputManager.UI.VirtualCursorPosition.canceled  -= OnReadNavigationInput;
        InputManager.UI.Click.performed -= OnReadClickInput;
    }
    void OnReadNavigationInput(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }

    void OnReadClickInput(InputAction.CallbackContext context)
    {
        //Debug.Log("Click");
        virtualMouse.CopyState<MouseState>(out var mouseState);
        mouseState.WithButton(MouseButton.Left, context.performed);

        InputState.Change(virtualMouse, mouseState);
    }

    private void Update()
    {
        if (virtualMouse == null || Gamepad.current == null)
            return;

        Vector2 currentPositon = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPositon + (_inputVector * cursorSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, padding.x, Screen.width - padding.x);
        newPosition.y = Mathf.Clamp(newPosition.y, padding.y, Screen.height - padding.y);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, _inputVector);

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.current, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }
}

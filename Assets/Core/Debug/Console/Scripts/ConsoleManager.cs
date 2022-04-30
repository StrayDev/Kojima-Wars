using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Console.Scripts
{
    public class ConsoleManager : MonoBehaviour
    {
        [SerializeField] private int maxLogCount = 100;

        [SerializeField] private TMP_InputField inputField = default;
        [SerializeField] private ScrollRect scrollRect = default;

        [SerializeField] private GameObject canvas = default;
        [SerializeField] private GameObject contentParent = default;
        [SerializeField] private GameObject msgPrefab = default;

        [SerializeField] private GameStateDataSO gameData = default;

        private Queue<GameObject> msgList = new Queue<GameObject>();

        private void Awake()
        {
            // hide console
            DontDestroyOnLoad(gameObject);

            // set up input 
            var actions = new InputActions().Dev;
            actions.Enable();

            // register callbacks
            actions.ToggleConsole.performed += ToggleConsole;
            actions.Submit.canceled += SubmitCommand;

            DeveloperConsole.PrintToConsoleCallback += CreateConsoleMessage;

            // print help message
            var prompt = "<color=#00ffffff>[ Info ]</color> : type <b>/help</b> for list of commands";
            CreateConsoleMessage(prompt, Color.white);

            // hide canvas
            canvas.SetActive(false);
        }

        private void OnDisable() => DeveloperConsole.UnhookUnityConsole();

        private ControlType inputType = ControlType.MECH;

        private void ToggleConsole(InputAction.CallbackContext context)
        {
            //if (gameData.isPaused) return;           

            // disable if no event system in scene
            if (!EventSystemFound()) return;

            InputManager.ToggleInput();
            ToggleMouseCursor();

            canvas.SetActive(!canvas.activeSelf);
            if (!canvas.activeSelf) return;

            inputField.Select();
            inputField.ActivateInputField();
        }

        private void ToggleMouseCursor()
        {
            if (canvas.activeSelf)
            {
                CursorManager.DisableCursor("console-manager");
            }
            else
            {
                CursorManager.EnableCursor("console-manager");
            }
        }

        private bool EventSystemFound()
        {
            return FindObjectOfType<EventSystem>() != null;
        }

        private void SubmitCommand(InputAction.CallbackContext context)
        {
            DeveloperConsole.SubmitCommand(inputField.text);
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        private void CreateConsoleMessage(string input, Color colour)
        {
            // create the message
            var msg = Instantiate(msgPrefab, contentParent.transform);
            var text = msg.GetComponent<TMP_Text>();
            text.text = input;
            text.color = colour;

            // keep view at end of texts
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;

            // delete if too many messages
            msgList.Enqueue(msg);
            if (msgList.Count > maxLogCount)
            {
                var toDestroy = msgList.Dequeue();
                Destroy(toDestroy);
            }

        }
    }
}
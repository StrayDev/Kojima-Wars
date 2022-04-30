using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public enum MenuState
    {
        MAIN,
        PLAY,
        SETTINGS,
        CREDITS
    }

    [SerializeField] private string version;
    [SerializeField] private TMP_Text version_text;

    [SerializeField] private Image UWE_logo;

    [SerializeField] CanvasGroup play_canvas;
    [SerializeField] CanvasGroup settings_canvas;
    [SerializeField] CanvasGroup credit_canvas;

    [Header("Game Launch")] [SerializeField]
    string gameLaunchSceneName = "TestGameScene";

    [Header("Play Page")] [SerializeField] Button joinGameButton;
    [SerializeField] Button hostGameButton;
    [SerializeField] Button endGameButton;
    [SerializeField] Button leaveGameButton;
    [SerializeField] Button playGameButton;
    [SerializeField] TMPro.TMP_Text relayJoinCodeText;
    [SerializeField] string relayJoinCodePrefix = "Join Code: ";
    [SerializeField] TMPro.TMP_InputField relayJoinCodeInputField;

    private Vector3 logo_goal_pos;

    private Vector3 logo_hidden_pos;

    //private MenuState menu_state = MenuState.MAIN;
    private bool mid_transition = false;

    private string enteredRelayJoinCode = "";

    void Start()
    {
        version_text.text = version;
        play_canvas.alpha = 0;
        play_canvas.interactable = false;
        play_canvas.blocksRaycasts = false;
        settings_canvas.alpha = 0;
        settings_canvas.interactable = false;
        settings_canvas.blocksRaycasts = false;
        credit_canvas.alpha = 0;
        credit_canvas.interactable = false;
        credit_canvas.blocksRaycasts = false;
        setupLogo();
        setupPlayPage();
    }

    private void setupPlayPage()
    {
        resetEnteredRelayJoinCode();

        relayJoinCodeText.gameObject.SetActive(false);
        endGameButton.gameObject.SetActive(false);
        leaveGameButton.gameObject.SetActive(false);
        playGameButton.gameObject.SetActive(false);
    }

    private void setupLogo()
    {
        logo_goal_pos = UWE_logo.transform.position;
        logo_hidden_pos = UWE_logo.transform.position;
        logo_hidden_pos.y -= 200;
        UWE_logo.transform.position = logo_hidden_pos;
        makeLogoHidden(false);
    }

    private void makeLogoHidden(bool _hide)
    {
        StartCoroutine(moveLogo(_hide, 0.6f));
    }

    private IEnumerator moveLogo(bool _hide, float _duration)
    {
        float e_time = 0f;
        float duration = _duration;
        Vector3 target = _hide ? logo_hidden_pos : logo_goal_pos;
        Vector3 current = UWE_logo.transform.position;
        while (e_time < duration)
        {
            Vector3 new_pos = Vector3.Lerp(current, target, (e_time / duration));
            UWE_logo.transform.position = new_pos;
            e_time += Time.deltaTime;
            yield return null;
        }

        UWE_logo.transform.position = _hide ? logo_hidden_pos : logo_goal_pos;
    }

    public void startGamePressed()
    {
        changeMenuState(MenuState.PLAY);
    }

    public void settingsPressed()
    {
        changeMenuState(MenuState.SETTINGS);
    }

    public void creditsPressed()
    {
        changeMenuState(MenuState.CREDITS);
    }

    public void backButtonPressed()
    {
        changeMenuState(MenuState.MAIN);
    }

    public async void hostGameButtonPressed()
    {
        ScreenLog.Instance.Print("Host game pressed", Color.cyan);

        // Hide the host game button
        hostGameButton.gameObject.SetActive(false);

        // Hide the join game button
        joinGameButton.gameObject.SetActive(false);

        // Hide the relay join code input field
        relayJoinCodeInputField.gameObject.SetActive(false);

        // Start a new session on this machine. Sessions must always have at least 1 player client connected. This will be the host machine that starts the match.
        var sessionConnectionInfo = await Networking.NetworkLibrary.StartSession();
        if (!sessionConnectionInfo.Succesful)
        {
            ScreenLog.Instance.Print("SESSION START WAS UNSUCCESFUL.", Color.red, 5f);

            // Show the host game button
            hostGameButton.gameObject.SetActive(true);

            // Show the join game button
            joinGameButton.gameObject.SetActive(true);

            // Show the relay join code input field
            relayJoinCodeInputField.gameObject.SetActive(true);

            return;
        }

        // Set and show the relay join code
        updateRelayJoinCodeDisplayText(sessionConnectionInfo.RelayJoinCode);
        relayJoinCodeText.gameObject.SetActive(true);

        // Show the end game button
        endGameButton.gameObject.SetActive(true);

        // Show the play game button
        playGameButton.gameObject.SetActive(true);
    }

    public void endGameButtonPressed()
    {
        // Show the join game button
        joinGameButton.gameObject.SetActive(true);

        // Show the relay join code input field
        relayJoinCodeInputField.gameObject.SetActive(true);

        // Show the host game button
        hostGameButton.gameObject.SetActive(true);

        // Hide and reset the relay join code text
        resetEnteredRelayJoinCode();
        relayJoinCodeText.gameObject.SetActive(false);

        // Hide the end game button
        endGameButton.gameObject.SetActive(false);

        // Hide the play game button
        playGameButton.gameObject.SetActive(false);

        // End the network session
        Networking.NetworkLibrary.EndSession();
    }

    public async void joinGameButtonPressed()
    {
        // Hide the host game button
        hostGameButton.gameObject.SetActive(false);

        // Hide the join game button
        joinGameButton.gameObject.SetActive(false);

        // Hide the relay join code input field
        relayJoinCodeInputField.gameObject.SetActive(false);

        // Attempt to join the match at the connectIP address
        if (!await Networking.NetworkLibrary.JoinSession(enteredRelayJoinCode))
        {
            // The client failed to join the session
            ScreenLog.Instance.Print("UNABLE TO JOIN THE SESSION WITH JOIN CODE: " + enteredRelayJoinCode, Color.red,
                5f);

            // Show the host game button
            hostGameButton.gameObject.SetActive(true);

            // Show the join game button
            joinGameButton.gameObject.SetActive(true);

            // Show the relay join code input field
            relayJoinCodeInputField.gameObject.SetActive(true);

            return;
        }

        // Set and show the relay join code display text
        updateRelayJoinCodeDisplayText(enteredRelayJoinCode);
        relayJoinCodeText.gameObject.SetActive(true);

        // Show the leave game button
        leaveGameButton.gameObject.SetActive(true);
    }

    public void leaveGameButtonPressed()
    {
        // Hide and reset the relay join code display text
        resetEnteredRelayJoinCode();
        relayJoinCodeText.gameObject.SetActive(false);

        // Hide the leave game button
        leaveGameButton.gameObject.SetActive(false);

        // Show the host game button
        hostGameButton.gameObject.SetActive(true);

        // Show the join game button
        joinGameButton.gameObject.SetActive(true);

        // Show the relay join code input field
        relayJoinCodeInputField.gameObject.SetActive(true);

        // Leave the network session
        Networking.NetworkLibrary.LeaveSession();
    }

    public void playGameButtonPressed()
    {
        ScreenLog.Instance.Print("Play game pressed", Color.cyan);
        // transition out
        
        
        // Open and switch to the game scene
        StartCoroutine( SceneLoader.NetworkLoadSceneCoroutine(gameLaunchSceneName));
        //Networking.NetworkLibrary.OpenScene(gameLaunchSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void relayJoinCodeInputFieldEndEdit(string newValue)
    {
        // Update the entered relay join code
        enteredRelayJoinCode = newValue.ToUpper();
    }

    private void resetEnteredRelayJoinCode()
    {
        relayJoinCodeInputField.text = "";
        relayJoinCodeInputFieldEndEdit("");
    }

    private void updateRelayJoinCodeDisplayText(string joinCode)
    {
        relayJoinCodeText.text = relayJoinCodePrefix + joinCode;
    }

    private void changeMenuState(MenuState _new_state)
    {
        if (!mid_transition)
        {
            mid_transition = true;
            switch (_new_state)
            {
                case MenuState.MAIN:
                {
                    StartCoroutine(updateCanvasGroup(
                        new List<CanvasGroup>() {play_canvas, settings_canvas, credit_canvas}, false, () =>
                        {
                            mid_transition = false;
                            makeLogoHidden(false);
                        }));
                    break;
                }
                case MenuState.PLAY:
                {
                    StartCoroutine(updateCanvasGroup(
                        new List<CanvasGroup>() {play_canvas, settings_canvas, credit_canvas}, false, () =>
                        {
                            StartCoroutine(updateCanvasGroup(new List<CanvasGroup>() {play_canvas}, true, () =>
                            {
                                mid_transition = false;
                                makeLogoHidden(true);
                            }));
                        }));
                    break;
                }
                case MenuState.SETTINGS:
                {
                    StartCoroutine(updateCanvasGroup(
                        new List<CanvasGroup>() {play_canvas, settings_canvas, credit_canvas}, false, () =>
                        {
                            StartCoroutine(updateCanvasGroup(new List<CanvasGroup>() {settings_canvas}, true, () =>
                            {
                                mid_transition = false;
                                makeLogoHidden(true);
                            }));
                        }));
                    break;
                }
                case MenuState.CREDITS:
                {
                    StartCoroutine(updateCanvasGroup(
                        new List<CanvasGroup>() {play_canvas, settings_canvas, credit_canvas}, false, () =>
                        {
                            StartCoroutine(updateCanvasGroup(new List<CanvasGroup>() {credit_canvas}, true, () =>
                            {
                                mid_transition = false;
                                makeLogoHidden(true);
                            }));
                        }));
                    break;
                }
                default:
                {
                    throw new ArgumentException($"Unknown Menu state: {_new_state}");
                }
            }
        }
    }

    private IEnumerator updateCanvasGroup(List<CanvasGroup> affected_canvas_list, bool _make_visable, Action _callback)
    {
        float e_time = 0f;
        float duration = 0.5f;
        Dictionary<int, float> list_of_current = new Dictionary<int, float>();
        for (int i = 0; i < affected_canvas_list.Count; i++)
        {
            list_of_current[i] = affected_canvas_list[i].alpha;
        }

        float target = _make_visable ? 1f : 0f;
        while (e_time < duration)
        {
            for (int i = 0; i < affected_canvas_list.Count; i++)
            {
                float new_alpha = Mathf.Lerp(list_of_current[i], target, (e_time / duration));
                affected_canvas_list[i].alpha = new_alpha;
            }

            e_time += Time.deltaTime;
            yield return null;
        }

        affected_canvas_list.ForEach(_e =>
        {
            _e.alpha = target;
            _e.interactable = _make_visable;
            _e.blocksRaycasts = _make_visable;
        });
        _callback();
    }
}
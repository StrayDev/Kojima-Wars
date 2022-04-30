using Unity.Netcode;
using UnityEngine;

namespace Core
{
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField] private MenuReferenceContainer container = default;

        private readonly string prefix = "Lobby Code : ";
        private string _joinCode = default;
        
        private void Awake()
        {
            UIHelper.SetGroupActive(container.joinGameGroup, false);
            UIHelper.SetGroupActive(container.inLobbyGroup, false);
            
            container.inputField.onSubmit.AddListener(OnSubmitJoinCode);
            container.submitButton.onClick.AddListener(OnSubmitJoinCode);
            container.errorMessageText.enabled = false;
        }

        private void SetCanSubmitCode(bool value)
        {
            container.inputField.interactable = value;
            container.submitButton.interactable = value;
        }
        
        
        public async void OnHostGameButtonPressed()
        {
            // disable the button
            container.hostGameButton.interactable = false;
            
            // try to connect and launch host
            var connection = await Networking.NetworkLibrary.StartSession();

            if (!connection.Succesful)
            {
                container.errorMessageText.text = connection.Error;
                container.errorMessageText.enabled = true;
                container.hostGameButton.interactable = true;
                return;
            }

            container.errorMessageText.enabled = false;

            // reenable for next time
            container.hostGameButton.interactable = true;

            // submit relay code
            container.joinCodeText.text = prefix + connection.RelayJoinCode.ToUpper();



            // move to lobby
            UIHelper.SetGroupActive(container.modeSelectGroup, false);
            UIHelper.SetGroupActive(container.inLobbyGroup, true);
        }
        
        public void OnJoinGameButtonPressed()
        {
            // stops you from spamming both buttons
            if (!container.hostGameButton.interactable)
            {
                Debug.Log("Not interactable");
                return;
            } 
            UIHelper.ResetInputField(container.inputField);
            
            UIHelper.SetGroupActive(container.modeSelectGroup, false);
            UIHelper.SetGroupActive(container.joinGameGroup, true);
        }

        public void CopyCode() {
            GUIUtility.systemCopyBuffer = container.joinCodeText.text.Substring(prefix.Length);
        }


        public void OnSubmitJoinCode() => OnSubmitJoinCode(container.inputField.text);
        public async void OnSubmitJoinCode(string input)
        {
            // disable submit 
            SetCanSubmitCode(false);
            
            // if code is not 6 long
            if (input.Length != 6)
            {
                // invalid code //
                SetCanSubmitCode(true);
                container.errorMessageText.text = "Invalid join code";
                container.errorMessageText.enabled = true;
                return ;
            }
            
            // Attempt to join the match at the connectIP address
            if (!await Networking.NetworkLibrary.JoinSession(input))
            {
                container.errorMessageText.text = "Game not found";
                container.errorMessageText.enabled = true;
                SetCanSubmitCode(true);
                UIHelper.ResetInputField(container.inputField);
                return;
            }
            container.errorMessageText.enabled = false;

            UIHelper.SetGroupActive(container.joinGameGroup, false);
            UIHelper.SetGroupActive(container.inLobbyGroup, true);
            
            // store the join code
            _joinCode = input;
            container.joinCodeText.text = prefix + _joinCode.ToUpper();
        }

        public void OnLeaveLobbyButtonPressed()
        {

            if (NetworkManager.Singleton.IsServer)
            {
                Networking.NetworkLibrary.EndSession();
            }
            else
            {
                Networking.NetworkLibrary.LeaveSession();
            }

            UIHelper.SetGroupActive(container.inLobbyGroup, false);
            UIHelper.SetGroupActive(container.modeSelectGroup, true);
        }

        public void OnJoinBackButton()
        {
            container.errorMessageText.enabled = false;
            UIHelper.SetGroupActive(container.modeSelectGroup, true);
            UIHelper.SetGroupActive(container.joinGameGroup, false);
        }
    }
}

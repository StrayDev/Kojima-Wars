using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

namespace Core
{
    public class PlayerCardHandler : NetworkBehaviour
    {
        [SerializeField] private GameObject playerCard = default;
        [SerializeField] private GameObject parent = default;
        [SerializeField] private float StartCountdownDuration = 1f;

        [SerializeField] private PlayerDataSO playerdata = default;
        [SerializeField] private string SceneToEnter = "GameScene";

        [SerializeField] private Color ReadyColor = Color.blue;
        [SerializeField] private string ReadyText = "Ready";
        [SerializeField] private Color NotReadyColor = Color.red;
        [SerializeField] private string NotReadyText = "Not Ready";

        private Dictionary<ulong, PlayerCard> _cardList;

        private Coroutine gameStartCountdownCoroutine;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                if (NetworkManager.Singleton == null) return;
                var network = NetworkManager.Singleton;
                _cardList = new Dictionary<ulong, PlayerCard>();
                network.OnClientConnectedCallback += OnPlayerConnect;
                network.OnClientDisconnectCallback += OnPlayerLeave;
                playerdata.List.Clear();
                OnPlayerConnect(network.LocalClientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerDataServerRpc(ulong id, FixedString32Bytes name, bool isRedTeam)
        {
            UpdatePlayerData(id, name.ConvertToString(), isRedTeam);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateCardFieldsServerRpc()
        {
            foreach(KeyValuePair<ulong, PlayerCard> pair in _cardList)
            {
                pair.Value.UpdateNameFieldClientRpc(pair.Value.GetPlayerName());
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnPlayerCardReadyUpPressedServerRpc(ulong readiedPlayerId)
        {
            // Update the card ready status on all connections
            PlayerCard card = _cardList[readiedPlayerId];

            card.is_ready = !card.is_ready;

            ChangeCardReadyStatusClientRpc(card.gameObject, card.is_ready);

            // If the card is not ready and the game start countdown is running
            if(!card.is_ready /* is game countdown running? */)
            {
                // Stop the countdown
                StopGameCountdown();
            }

            // Update player data
            UpdatePlayerData(readiedPlayerId, card.GetPlayerName(), card.is_red_team.Value);

            // Check if every card is readied up
            bool canStartGame = true;
            foreach(KeyValuePair<ulong, PlayerCard> pair in _cardList)
            {
                if(!pair.Value.is_ready)
                {
                    canStartGame = false;
                    break;
                }
            }

            // Start the game countdown if every connection is readied up
            if(canStartGame)
            {
                StartGameCountdown();
            }
        }

        [ClientRpc]
        public void ChangeCardReadyStatusClientRpc(NetworkObjectReference cardReference, bool readyStatus)
        {
            NetworkObject cardObject = null;
            cardReference.TryGet(out cardObject);

            PlayerCard card = cardObject.GetComponent<PlayerCard>();

            if(readyStatus)
            {
                SetCardReadyParams(card, true, ReadyText, ReadyColor);
            }
            else
            {
                SetCardReadyParams(card, false, NotReadyText, NotReadyColor);
            }
        }

        private void SetCardReadyParams(PlayerCard card, bool enabled, string text, Color color)
        {
            card.SetPlayerNameFieldEnabled(enabled);
            card.SetReadyTextString(text);
            card.SetReadyTextColor(color);
        }

        private void StartGameCountdown()
        {
            if (!IsServer) return;

            gameStartCountdownCoroutine = StartCoroutine(GameCountdownFunc());
        }

        private void StopGameCountdown()
        {
            StopCoroutine(gameStartCountdownCoroutine);
        }

        public IEnumerator GameCountdownFunc()
        {
            yield return new WaitForSeconds(StartCountdownDuration);

            var coroutine = SceneLoader.NetworkLoadSceneCoroutine(SceneToEnter);
            StartCoroutine(coroutine);
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton == null) return;

            var network = NetworkManager.Singleton;
            network.OnClientConnectedCallback -= OnPlayerConnect;
            network.OnClientDisconnectCallback -= OnPlayerLeave;
        }

        private void OnPlayerConnect(ulong id)
        {
            OnPlayerConnectServerRpc(id);
        }

        private void OnPlayerLeave(ulong id)
        {
            OnPlayerLeaveServerRpc(id);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnPlayerConnectServerRpc(ulong id)
        {
            // create the prototype
            var playerCard = Instantiate(this.playerCard, parent.transform);
            // spawn the objects
            playerCard.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);

            // set up playercard stuff here
            PlayerCard playerCardComponent = playerCard.GetComponent<PlayerCard>();

            _cardList[id] = playerCardComponent;

            PlayerData data = new PlayerData();
            data.SetName(playerCardComponent.getName().ToString());
            data.SetTeamRed(playerCardComponent.is_red_team.Value);
            playerdata.List[id] = data;

            playerCardComponent.setNameServerRpc(new FixedString32Bytes($"Player {_cardList.Count}"));
            playerCardComponent.setTeamStatusServerRpc(_cardList.Count % 2 != 0);
        }

        // Only call this function on the server
        public void UpdatePlayerData(ulong id, string name, bool isRedTeam)
        {
            if (!IsServer) return;

            PlayerData data = new PlayerData();
            data.SetName(name.ToString());
            data.SetTeamRed(isRedTeam);
            playerdata.List[id] = data;
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnPlayerLeaveServerRpc(ulong id)
        {
            OnPlayerLeaveClientRpc(id);
        }

        [ClientRpc]
        private void OnPlayerLeaveClientRpc(ulong id)
        {
            // destroy 
            Destroy(_cardList[id]);

            // remove from list
            _cardList.Remove(id);
            playerdata.List.Remove(id);
        }

        public void killCards()
        {
            foreach (var card in _cardList)
            {
                Destroy(card.Value);
            }
            _cardList.Clear();
        }

    }
}

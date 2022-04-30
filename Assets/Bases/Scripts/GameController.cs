using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;
using UnityEngine.SceneManagement;
using Core;
using System.Threading.Tasks;

public enum WinCondition
{
    SCORE,
    BASES,
    TIMER
}

public class GameController : NetworkBehaviour
{
    [SerializeField] private GameStateDataSO m_gameStateData = default;
    [SerializeField] private GameTeamData m_gameTeamData = default;
    [SerializeField] private PlayerDataSO m_playerData = default;
    [SerializeField] private GameTimerUI m_GameTimerUI = default;
    [SerializeField] private TeamScoreUI m_TeamScoreUI = default;
    [SerializeField] private UnitMapUI m_UnitMapUI = default;

    private bool m_gameFinished = false;

    public Counter GameTimer
    {
        get;
        private set;
    }

    public BaseController[] m_allBases = { };

    [ServerRpc(RequireOwnership = false)]
    public void SelectAIGroupServerRpc(ulong localPlayerId, int groupIndex)
    {
        AIGroup group = m_playerData.GetAIGroup(localPlayerId, groupIndex);

        if (group.BaseController != null)
        {
            SelectBaseSelectionIconsClientRpc(localPlayerId, group.BaseController.GetBaseId());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectBaseAIMapServerRpc(ulong localPlayerId, int baseId, int groupIndex)
    {
        // Get the selected base controller
        BaseController selectedBaseController = null;
        foreach(BaseController baseController in m_allBases)
        { 
            if(baseId == baseController.GetBaseId())
            {
                selectedBaseController = baseController;
                break;
            }
        }

        Debug.Log("Selected map: " + selectedBaseController.name);

        // Get group data from player data
        AIGroup group = m_playerData.GetAIGroup(localPlayerId, groupIndex);

        // Set base controller ai group are at
        m_playerData.SetBaseControllerForGroup(localPlayerId, groupIndex, selectedBaseController);

        // Move agents in group to the base controller
        foreach (GameObject agent in group.agents)
        {
            if(m_playerData.GetPlayerTeam(localPlayerId) && selectedBaseController.TeamOwner == "red" ||
              !m_playerData.GetPlayerTeam(localPlayerId) && selectedBaseController.TeamOwner == "blue")
            {
                agent.GetComponent<AI_AgentController>().MoveToDefendPosition(groupIndex, baseId);
            }
            else
            {
                agent.GetComponent<AI_AgentController>().MoveToDefendPosition(groupIndex, baseId);
            }
        }
    }

    [ClientRpc]
    private void SelectBaseSelectionIconsClientRpc(ulong localPlayerId, int baseId)
    {
        if (NetworkManager.Singleton.LocalClientId != localPlayerId) return;
        m_UnitMapUI.SetBaseSelectionIcons(localPlayerId, baseId);
    }

    [ServerRpc]
    public void AddScoreToTeamServerRpc(string teamName, int scoreToAdd)
    {
        GameTeamData.TeamData teamData = m_gameTeamData.GetTeamData(teamName);
        teamData.Score += scoreToAdd;
        m_gameTeamData.SetTeamData(teamName, teamData);

        m_TeamScoreUI.OnTeamScoreUpdated(teamName, teamData.Score);
        UpdateScoreClientRpc(teamName, teamData.Score);

        if (teamData.Score >= m_gameStateData.scoreThreshold)
        {
            HandleEndGame(WinCondition.SCORE, teamName);
        }
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(string teamName, int score)
    {
        if (IsServer) return;

        m_TeamScoreUI.OnTeamScoreUpdated(teamName, score);
    }

    [ServerRpc]
    public void PauseGameTimerServerRpc()
    {
        GameTimer.Pause();
    }

    [ServerRpc]
    public void ResumeGameTimerServerRpc()
    {
        GameTimer.Resume();
    }

    private void Awake()
    {
        CursorManager.DisableCursor("main-menu");
        m_allBases = FindObjectsOfType<BaseController>();
        foreach (BaseController baseController in m_allBases)
        {
            baseController.OnOwnerChanged += OnBaseChangedOwner;
        }
    }

    private void Start()
    {
        if(IsServer)
        {
            GameTimer = new Counter(this, m_gameStateData.maxGameLength, Counter.TimerType.MINUTES);
            GameTimer.OnComplete += OnGameTimerComplete;
            GameTimer.OnChanged += OnGameTimerChangedServerRpc;
            GameTimer.StartTimer();
        }
    }

    private void OnDisable()
    {
        if(IsServer)
        {
            GameTimer.OnComplete -= OnGameTimerComplete;
        }

        foreach (BaseController baseController in m_allBases)
        {
            baseController.OnOwnerChanged -= OnBaseChangedOwner;
        }
    }

    private void OnBaseChangedOwner(string newOwner)
    {
        string previousName = null;
        foreach (BaseController baseController in m_allBases)
        {
            if (previousName != null && baseController.TeamOwner != previousName)
            {
                return;
            }

            previousName = baseController.TeamOwner;
        }

        HandleEndGame(WinCondition.BASES, previousName);
    }

    [ServerRpc]
    private void OnGameTimerChangedServerRpc()
    {
        if (GameTimer.IsCounting)
        {
            float timeRemaining = GameTimer.TimeRemaining;
            float secondsRemaining = timeRemaining % 60;
            float minutesRemaining = (timeRemaining - secondsRemaining) / 60;
            UpdateGameTimeClientRpc(minutesRemaining, secondsRemaining);
        }
    }

    [ClientRpc]
    private void UpdateGameTimeClientRpc(float minutesRemaining, float secondsRemaining)
    {
        string text = $"{minutesRemaining.ToString("00")}:{secondsRemaining.ToString("00")}";
        m_GameTimerUI.UpdateTime(text);
    }

    private void OnGameTimerComplete()
    {
        string winner = null;

        int bestScore = 0;
        // Key is team name and value is team data
        Dictionary<string, GameTeamData.TeamData> teamDataDictionary = m_gameTeamData.GetTeamDataDictionary();
        foreach (KeyValuePair<string, GameTeamData.TeamData> keyValuePair in teamDataDictionary)
        {
            if(keyValuePair.Value.Score > bestScore)
            {
                bestScore = keyValuePair.Value.Score;
                winner = keyValuePair.Key;
            }
        }

        HandleEndGame(WinCondition.TIMER, winner);
    }

    private void HandleEndGame(WinCondition condition,  string winningTeam)
    {
        GameTimer.Pause();

        // pause all game input
        GameObject.Find("PauseManager").GetComponent<PauseManager>().Pause(false);

        if (GameObject.Find("PauseUI").GetComponent<PauseUIManager>().UIActive)
        {
            GameObject.Find("PauseUI").GetComponent<PauseUIManager>().HidePauseUI();
        }
        if (GameObject.Find("PlayerClassUI").GetComponent<PlayerClassUI>().isActiveAndEnabled)
        {
            GameObject.Find("PlayerClassUI").GetComponent<PlayerClassUI>().ShowUI(false);
        }
        InputManager.SetInputType(ControlType.NONE);


        EndGameServerRpc(condition, winningTeam);
    }

    [ServerRpc(RequireOwnership =false)]
    private void EndGameServerRpc(WinCondition condition, FixedString32Bytes winningTeam)
    {
        if(!m_gameFinished)
        {
            m_gameFinished = true;
            // TODO Authorize game has ended on server (Score has been reached)
            //Debug.Log("Server handling end game for winning team: " + winningTeam);
            ShowGameFinishedUIClientRpc(condition, winningTeam);
        }
    }

    [ClientRpc]
    private void ShowGameFinishedUIClientRpc(WinCondition condition, FixedString32Bytes winningTeam)
    {
        var GameFinishedUI = GameObject.Find("GameFinishedUI");
        GameFinishedUI.GetComponent<GameFinishedUIController>().OnGameEnd(condition, winningTeam.ConvertToString());
    }

    public void ExitGame()
    {
        //Debug.Log("Exiting game");

        ExitGameServerRpc(NetworkManager.Singleton.LocalClientId);
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExitGameServerRpc(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.ServerClientId)
        {
            ExitGameAllClientRpc();
            DisconnectAfterAllClientsAsync();
        }
        else
        {
            ExitGameSingleClientRpc(clientID);
        }
    }

    private async void DisconnectAfterAllClientsAsync()
    {
        while(NetworkManager.Singleton.ConnectedClients.Count > 1)
        {
            await Task.Delay(500);
        }
        
        Networking.NetworkLibrary.EndSession();
        SceneManager.LoadScene("MenuScene");
    }

    [ClientRpc]
    private void ExitGameAllClientRpc()
    {
        if (IsServer) return;

        Networking.NetworkLibrary.LeaveSession();
        SceneManager.LoadScene("MenuScene");
    }

    [ClientRpc]
    private void ExitGameSingleClientRpc(ulong clientID)
    {
        if(clientID == NetworkManager.Singleton.LocalClientId)
        {
            Networking.NetworkLibrary.LeaveSession();
            SceneManager.LoadScene("MenuScene");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;


    [SerializeField] private string networkedPlayerStartTag = "NetworkedPlayerStart";
    [SerializeField] private Vector3 defaultPlayerSpawnPosition = Vector3.zero;
    [SerializeField] private Quaternion defaultPlayerSpawnRotation = Quaternion.identity;
    //[SerializeField] private GameTeamData m_gameTeamData = default;

    void Start()
    {
        ScreenLog.Instance.Print("Game scene start", Color.green, 5f);

        // Spawn the player prefab
        if (IsHost)
        {
            SpawnPlayersServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayersServerRpc()
    {
        // Get networked player starts in the scene
        var networkedPlayerStarts = GameObject.FindGameObjectsWithTag(networkedPlayerStartTag);
        //InitialBaseController[] baseControllers = FindObjectsOfType<InitialBaseController>();

        // Check if there are less networked player starts than players
        if (networkedPlayerStarts.Length < NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            ScreenLog.Instance.Print("SPAWNING MORE NETWORKED PLAYERS THAN THERE ARE PLAYER STARTS.", Color.yellow, 5f);
        }

        // For each connected client
        int i = 0;
        foreach (NetworkClient connectedClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            //string playerTeam = m_gameTeamData.GetTeamDataAtIndex(i % m_gameTeamData.TeamCount).TeamName;
            //i++;
            //Debug.Log(i + ", " + i % m_gameTeamData.TeamCount + ", " + playerTeam);

            //BaseController initialBase = null;

            //Vector3 spawnPosition = defaultPlayerSpawnPosition;
            //Quaternion spawnRotation = defaultPlayerSpawnRotation;

            //foreach (InitialBaseController controller in baseControllers)
            //{
            //    if (controller.InitialBaseOwner == playerTeam)
            //    {
            //        initialBase = controller;
            //        spawnPosition = initialBase.PlayerStartPosition.position;
            //        spawnRotation = initialBase.PlayerStartPosition.rotation;
            //        break;
            //    }
            //}

            //if (initialBase == null)
            //{
            //    ScreenLog.Instance.Print(
            //        $"Player team {playerTeam} has no starting base, spawning player in default position.",
            //        Color.red,
            //        10f
            //    );
            //}

            var spawnPosition = defaultPlayerSpawnPosition;
            var spawnRotation = defaultPlayerSpawnRotation;

            if ( i < networkedPlayerStarts.Length)
            {
                var start = networkedPlayerStarts[i];
                spawnPosition = start.transform.position;
                spawnRotation = start.transform.rotation;
                ++i;
            }

            // Spawn a player prefab 
            GameObject player = Instantiate(playerPrefab, spawnPosition, spawnRotation);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(connectedClient.ClientId, true);
            //player.GetComponent<Entity>().ChangeTeam(playerTeam);
        }
    }
}

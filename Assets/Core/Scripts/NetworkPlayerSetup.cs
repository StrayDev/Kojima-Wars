using Cinemachine;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// This class is responsible for spawning in the network 
    /// prefabs for the player and setting their initial position
    /// </summary>

    public class NetworkPlayerSetup : NetworkBehaviour
    {
        private static NetworkPlayerSetup instance = default;
        public static NetworkPlayerSetup Get() => instance;

        public static CinemachineVirtualCamera MechCamera = null;

        public event Action<ulong> OnSpawnEvents = default;

        [Header("MECH Prefabs")]
        [SerializeField] private GameObject mechPrefab = default;
        [SerializeField] private GameObject mechCameraPrefab = default;

        [Header("VTOL Prefabs")]
        [SerializeField] private GameObject vtolPrefab = default;
        [SerializeField] private GameObject vtolCameraPrefab = default;
        [SerializeField] private GameObject mapCameraPrefab = default;
        
        [Header("Spawn Lists")]
        [SerializeField] public List<Transform> redSpawnList = default;
        [SerializeField] private List<Transform> blueSpawnList = default;

        // player data
        [SerializeField] private PlayerDataSO playerData = default;

        private void Start()
        {
            instance = this;

            if (!IsServer) return;
            
            if (redSpawnList.Count == 0 || blueSpawnList.Count == 0)
            {
                Debug.LogWarning("Spawn list is not populated", this);
                return;
            }

            SpawnPlayersServerRpc();
            SetPlayerPositionsServerRpc();
            SpawnAndAssignCamerasServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayersServerRpc()
        {
            // for each connected client
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                // spawn a player prefab 
                var mech = Instantiate(mechPrefab);
                var vtol = Instantiate(vtolPrefab);

                mech.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId, true);
                vtol.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId, true);
                mech.GetComponent<Entity>().SetClientIDClientRpc(client.ClientId);
                vtol.GetComponent<Entity>().SetClientIDClientRpc(client.ClientId);

                TransformManager.RegisterPlayer(client.ClientId, mech, vtol);
                OnSpawnServerRpc(client.ClientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnPlayerServerRpc(Vector3 basePosition, Quaternion baseRotation, ulong respawningPlayerId)
        {
            var mcc = MechCharacterController.List[(int)respawningPlayerId];
            mcc.PositionMech(basePosition, baseRotation);

            OnSpawnServerRpc(respawningPlayerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerPositionsServerRpc()
        {
            var redCount = 0;
            var blueCount = 0;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var mech = TransformManager.GetMechFromClientID(client.ClientId);
                var vtol = TransformManager.GetVTOLFromClientID(client.ClientId);

                // check the team
                var team = playerData.GetPlayerTeam(client.ClientId);

                // find player name
                var name = playerData.GetPlayerName(client.ClientId);

                Transform currentPoint;

                Entity mechEntity = mech.GetComponent<Entity>();
                Entity vtolEntity = vtol.GetComponent<Entity>();
                if (team) // red
                {
                    mechEntity.ChangeTeamClientRpc("red");
                    vtolEntity.ChangeTeamClientRpc("red");
                    currentPoint = redSpawnList[redCount];
                    redCount++;
                    redCount %= redSpawnList.Count;
                }
                else // blue
                {
                    mechEntity.ChangeTeamClientRpc("blue");
                    vtolEntity.ChangeTeamClientRpc("blue");
                    currentPoint = blueSpawnList[blueCount];
                    blueCount++;
                    blueCount %= blueSpawnList.Count;
                }

                mechEntity.SetNameClientRpc(name);
                vtolEntity.SetNameClientRpc(name);

                // set the values
                mech.GetComponent<MechCharacterController>().PositionMech(currentPoint.position, currentPoint.rotation);
                
                // sets the player information for the clients // will need to check if server otherwise it takes last clients name?
                mech.GetComponent<PlayerInformation>().SetPlayerNameClientRpc(playerData.GetPlayerName(client.ClientId));
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnAndAssignCamerasServerRpc()
        {
            // spawn a player prefab 
            var mechCamera = Instantiate(mechCameraPrefab);
            var vtolCamera = Instantiate(vtolCameraPrefab);
            var mapCamera = Instantiate(mapCameraPrefab);
            
            mechCamera.GetComponent<NetworkObject>().Spawn(true);
            vtolCamera.GetComponent<NetworkObject>().Spawn(true);
            mapCamera.GetComponent<NetworkObject>().Spawn(true);

            MechCamera = mechCamera.GetComponent<CinemachineVirtualCamera>();
        }


        [ServerRpc]
        private void OnSpawnServerRpc(ulong spawningPlayerId)
        {
            // Call a client rpc that filters by respawning player id and shows the class ui on a ui manager?
            if(spawningPlayerId == OwnerClientId) // If the spawning player's id is the owner of the NetworkPlayerSetup script. This is the server by default as it is not spawned with ownership.
            {
                OnSpawn(spawningPlayerId);
            }
            else
            {
                OnSpawnClientRpc(spawningPlayerId);
            }
        }

        [ClientRpc]
        private void OnSpawnClientRpc(ulong spawningPlayerId)
        {
            if (spawningPlayerId != NetworkManager.Singleton.LocalClientId) // If the local machine id is the spawning player's id
                return;

            OnSpawn(spawningPlayerId);
        }

        private void OnSpawn(ulong spawningPlayerId)
        {
            OnSpawnEvents?.Invoke(spawningPlayerId);
        }
    }
}

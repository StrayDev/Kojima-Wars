using Unity.Netcode;
using UnityEngine;

namespace Networking.Scripts
{
    public class NetworkLogger : MonoBehaviour
    {
        void Start()
        {
            var network = NetworkManager.Singleton;
            
            network.OnClientConnectedCallback += OnClientConnected;
            network.OnServerStarted += OnServerStarted;
            network.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnServerStarted()
        {
            Debug.Log($"Server Started");
        }
        
        private void OnClientConnected(ulong id)
        {
            Debug.Log($"Client Connected with id : { id }");
        }
        
        private void OnClientDisconnected(ulong id)
        {
            Debug.Log($"Client { id } Disconnected");
        }
        
    }
}

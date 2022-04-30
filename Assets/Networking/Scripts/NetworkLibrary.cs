using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public static class NetworkLibrary
    {
        public static async Task<SessionConnectionInfo> StartSession()
        {
            var sessionConnectionInfo = new SessionConnectionInfo();

            // Check this machine is not currently a server or client
            if ((NetworkManager.Singleton.IsServer) || (NetworkManager.Singleton.IsClient))
            {
                Debug.LogWarning("THE MACHINE TRYING TO START A SESSION IS ALREADY CONNECTED TO A SESSION.");
                sessionConnectionInfo.Error = "Already connected to a session";
                // A match cannot be started if it is as this machine is already in a game
                return sessionConnectionInfo;
            }

            var relayHostData = await Relay.RelayManager.SetupRelayAsync();

            // Start a host (client and server)
            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogWarning("FAILED TO START HOST (SERVER/CLIENT).");

                sessionConnectionInfo.Error = "Failed to start host";

                // Shutdown the failed host
                NetworkManager.Singleton.Shutdown();

                return sessionConnectionInfo;
            }

            //Debug.Log("Started session");

            sessionConnectionInfo.RelayJoinCode = relayHostData.JoinCode;
            sessionConnectionInfo.Succesful = true;

            return sessionConnectionInfo;
        }

        public static void EndSession()
        {
            // Check this is the host as only the host can end a session
            if (NetworkManager.Singleton.IsHost)
            {
                //Debug.Log("Host ending session");
                NetworkManager.Singleton.Shutdown();
            }
        }

        public static async Task<bool> JoinSession(string relayJoinCode)
        {
            // Check this machine is not currently a server or client
            if ((NetworkManager.Singleton.IsServer) || (NetworkManager.Singleton.IsClient))
            {
                // The match cannot be joined if the machine is already connected to a match
                Debug.LogWarning("This client is already in a session");

                return false;
            }

            // Check the join code is not empty
            if (relayJoinCode == "")
            {
                Debug.LogWarning("Attempting to join a session with an invalid join code.");
                return false;
            }
            
            Debug.Log("Attempting to connect to session with code : " + relayJoinCode.ToUpper());

            // Connect to the session with the join code through relay
            var relayJoinData = await Relay.RelayManager.JoinRelayAsync(relayJoinCode);

            // return if failed to connect to relay with join with code
            if (relayJoinData.ConnectionFailed)
            {
                return false;
            }
            
            // Set the connect IP address to connect to (Handled by relay, so no longer needed)
            //NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = ip;

            // Start a client
            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogWarning("FAILED TO START CLIENT.");

                // Shutdown the failed client
                NetworkManager.Singleton.Shutdown();

                return false;
            }

            Debug.LogWarning("Connected to session with code: " + relayJoinCode);
            Debug.LogWarning("Joined session");

            return true;
        }

        public static void LeaveSession()
        {
            // Check this is a client and not the host as only clients can leave a session. Hosts should use EndSession to end the session
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Shutting down client");
                NetworkManager.Singleton.Shutdown(); 
            }
        }

        public static void OpenScene(string scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            // Check the host is calling as only the host can load new scenes
            if(NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(scene, mode);
            }
        }
    }
}

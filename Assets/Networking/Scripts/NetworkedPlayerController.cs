// Define ENABLE_DEBUG to enable debug overlay for the player
#define ENABLE_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace Networking
{
    public class NetworkedPlayerController : NetworkBehaviour
    {
        private string connectionTypeLabel = "";

        // Start is called before the first frame update
        void Start()
        {
#if ENABLE_DEBUG
            // Set the connection type label
            if (IsHost)
            {
                connectionTypeLabel = "Host (Server/Client " + NetworkManager.Singleton.LocalClientId.ToString() + ")";
            }
            else if (IsClient)
            {
                connectionTypeLabel = "Client " + NetworkManager.Singleton.LocalClientId.ToString();
            }
#endif // ENABLE_DEBUG

        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            if (IsLocalPlayer)
            {
                ScreenLog.Instance.Print("Jump", Color.cyan);
                GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 2.0f, 0.0f), ForceMode.Impulse);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnNetworkSpawn()
        {
            ScreenLog.Instance.Print(OwnerClientId.ToString(), Color.cyan);
        }

        private void Awake()
        {
        }

#if ENABLE_DEBUG
        private void OnGUI()
        {
            // Start a new immediate gui rect area
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));

            // Print a label to display the connection status of the machine
            GUILayout.Label(connectionTypeLabel);

            GUILayout.EndArea();
        }
#endif // ENABLE_DEBUG
    }
}

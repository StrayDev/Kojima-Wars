using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class InstantCaptureCommand : IConsoleCommand
    {
        private bool _toggleCapture = false; 
        
        public string Name() => "ic";

        public string Description() => "instantly captures the base the player is currently in proximity to";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene"))
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
                {
                    var player = GameObject.FindGameObjectsWithTag("Player")[i];
                
                    if (player.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        if (!_toggleCapture)
                        {
                            DeveloperConsole.Print("Instant Capture [ON]");

                            foreach (BaseCaptureZone zone in GameObject.FindObjectsOfType<BaseCaptureZone>())
                            {
                                zone.SetCaptureProgress(1);
                            }

                            _toggleCapture = true;
                        }
                        else
                        {
                            DeveloperConsole.Print("Instant Capture [OFF]");
                            
                            foreach (BaseCaptureZone zone in GameObject.FindObjectsOfType<BaseCaptureZone>())
                            {
                                zone.SetCaptureProgress(0);
                            }
                            
                            _toggleCapture = false;
                        }
                        
                        //player.GetComponent<MechCharacterController>().HandleMechDeadClientRpc();
                        //DeveloperConsole.Print(player.GetComponent<CS_PlayerStats>().health.Value.ToString());
                    }
                } 
            }
        }
    }
}

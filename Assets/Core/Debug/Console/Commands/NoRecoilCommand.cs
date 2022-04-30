using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class NoRecoilCommand : IConsoleCommand
    {
        private bool _isRecoilOn;
        public string Name() => "norecoil";

        public string Description() => "Removes recoil on all weapons";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene"))
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
                {
                    var player = GameObject.FindGameObjectsWithTag("Player")[i];

                    if (player.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        if (!_isRecoilOn)
                        {
                            DeveloperConsole.Print("NO RECOIL [ON]");
                            GameObject.FindGameObjectsWithTag("Player")[i].GetComponentInChildren<Recoil>().weaponScript.isRecoilOn = false;
                            _isRecoilOn = true;
                        }
                        else
                        {
                            DeveloperConsole.Print("NO RECOIL [OFF]");
                            GameObject.FindGameObjectsWithTag("Player")[i].GetComponentInChildren<Recoil>().weaponScript.isRecoilOn = true;
                            _isRecoilOn = false;
                        }
                    }
                }
            }
        }
    }
}
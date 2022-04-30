using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class InfiniteCurrencyCommand : IConsoleCommand
    {
        public string Name() => "givecurry";

        public string Description() => "Gives the player infinite resources";

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
                        PlayerInformation playerInformation = GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerInformation>();
                        playerInformation.IncreasePlayerCurrency(999999);
                    }
                }
            }
            else
            {
                DeveloperConsole.Print("Will you please listen, I am not the messiah!");
            }
        }
    }
}

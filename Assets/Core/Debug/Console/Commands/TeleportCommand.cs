using Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class TeleportCommand : IConsoleCommand
    {

        private GameObject selectedBase;

        //command name
        public string Name() => "tp";

        public string Description() => "/tp <base number>  :  teleports the player to the specified location. \n"
                                       + " [ Args ] \"1\", \"2\", \"3\" etc ";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene"))
            {
                var net = NetworkManager.Singleton;
                if (args.Length != 1)
                {
                    var error = "[ Invalid ]  /tp <location>  :  Takes 1 argument.";
                    DeveloperConsole.Print(error);
                    return;
                }

                InitialBaseController[] iBases = Object.FindObjectsOfType<InitialBaseController>();
                BaseController[] _bases = Object.FindObjectsOfType<BaseController>();
                int totalLength = iBases.Length + _bases.Length;
                //checks for integer arguments being passed
                int[] intArgs = new int[args.Length];
                if (int.TryParse(args[0], out intArgs[0]))
                {
                    
                    //checks if value is a valid range
                    if (intArgs[0] >= totalLength || intArgs[0] <= 0)
                    {
                        DeveloperConsole.Print("[ Invalid ] base value not found");
                        return;
                    }
                    else
                    {

                        for (int i = 1; i < totalLength + 1; i++)
                        {
                            //check which base is picked
                            if (args[0] == i.ToString())
                            {
                                selectedBase = i < iBases.Length ?  iBases[i].gameObject : _bases[i- iBases.Length].gameObject;
                                DeveloperConsole.Print(selectedBase.name);
                            }
                        }

                        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
                        {
                            var player = GameObject.FindGameObjectsWithTag("Player")[i];
                            //check which player to teleport
                            if (player.GetComponent<NetworkObject>().OwnerClientId == net.LocalClientId)
                            {
                                if (selectedBase.GetComponent<InitialBaseController>() != null)
                                {
                                    player.GetComponent<MechCharacterController>().PositionMech(selectedBase.GetComponent<InitialBaseController>().PlayerStartPosition.position, Quaternion.identity);
                                }
                                if(selectedBase.GetComponent<BaseController>() != null)
                                {
                                    player.GetComponent<MechCharacterController>().PositionMech(selectedBase.GetComponent<BaseController>().PlayerStartPosition.position, Quaternion.identity);
                                }
                                
                            }
                        }
                    }
                }
                else
                {
                    DeveloperConsole.Print("[ Invalid ] type mismatch on /tp. integer required");
                }
            }
        }
    }
}


using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class PauseTimerCommand : IConsoleCommand
    {
        public string Name() => "pausetime";

        public string Description() => "pauses / continues the game timer";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene"))
            {
                GameController controller = GameObject.Find("GameController").GetComponent<GameController>();

                if (controller.GameTimer.IsCounting)
                {
                    controller.PauseGameTimerServerRpc();
                }
                else
                {
                    controller.ResumeGameTimerServerRpc();
                }
            }
        }
    }
}

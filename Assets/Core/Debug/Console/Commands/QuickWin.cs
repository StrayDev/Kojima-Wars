using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class QuickWin : IConsoleCommand
    {
        public string Name() => "win";

        public string Description() => "grants the team given in the arguments the winning amount of points";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene"))
            {
                Object.FindObjectOfType<GameController>().AddScoreToTeamServerRpc(args[0], 1000);
            }
        }
    }
}

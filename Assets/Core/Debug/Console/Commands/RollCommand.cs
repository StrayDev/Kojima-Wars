using UnityEngine;

namespace Console.Commands
{
    public class RollCommand : IConsoleCommand
    {
        public string Name() => "roll";
        public string Description() => "Not fully implemented";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley");
            DeveloperConsole.Print("[ RickRoll ]  LMAO", Color.magenta);
        }
        
    }
}

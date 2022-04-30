using UnityEngine;

namespace Console.Commands
{
    public class QuitCommand : IConsoleCommand
    {
        public string Name() => "quit";
        public string Description() => "Force quits the application";
        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            if (!Application.isEditor) Application.Quit(); 
            
            #if UNITY_EDITOR //
            UnityEditor.EditorApplication.isPlaying = false;
            #endif // UNITY_EDITOR
        }
    }
}

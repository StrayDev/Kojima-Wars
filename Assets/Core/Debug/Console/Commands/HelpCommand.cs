using System.Linq;
using UnityEngine;

namespace Console.Commands
{
    public class HelpCommand : IConsoleCommand
    {
        public string Name() => "help";
        public string Description() => "Prints a list of common commands to console";
        public bool IsHidden() => true;

        public void Execute(string[] args)
        {
            var list = DeveloperConsole.Commands;

            foreach (var c in list.Where(c => !c.Value.IsHidden()))
            {
                var text = $"<color=#00ffffff>[ Info ]</color> : <b>/{ c.Key }</b> : { c.Value.Description() }";
                DeveloperConsole.Print(text, Color.white);
            }        
        }
    }
}

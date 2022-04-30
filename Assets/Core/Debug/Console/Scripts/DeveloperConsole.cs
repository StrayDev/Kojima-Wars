using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Console
{
    public static class DeveloperConsole
    {
        private static string prefix = "/";

        public static Dictionary<string, IConsoleCommand> Commands = new Dictionary<string, IConsoleCommand>();

        public static Action<string, Color> PrintToConsoleCallback = default;
        
        public static void Print(string msg, Color color) => PrintToConsoleCallback?.Invoke(msg, color); 
        public static void Print(string msg) => Print(msg, Color.white);
        

        static DeveloperConsole()
        {
            // register commands
            foreach (var command in CommandList.List)
            {
                Commands.Add(command.Name(), command);
            }
            
            // hook into unity's logging system
            Application.logMessageReceived += ProcessLog;
        }
        
        public static void UnhookUnityConsole() => Application.logMessageReceived -= ProcessLog;

        public static void SubmitCommand(string command)
        {
            // empty string do nothing
            var trim = command.Trim();
            if (trim.Length < 1) return;

            command = command.ToLower();
            
            // check for prefix
            if (!command.StartsWith(prefix))
            {
                // invalid input prints gray
                PrintToConsoleCallback?.Invoke($"<color=white>[ Invalid ]</color>  \"{command}\"", Color.grey);
                return;
            }
            
            // Process the command
            ProcessCommand(command);
        }

        private static void ProcessCommand(string command)
        {
            // remove prefix and split string by space
            var input = command.Remove(0, prefix.Length);
            var split = input.Split(' ');
            

            // prints invalid command
            if (!Commands.ContainsKey(split[0]))
            {
                PrintToConsoleCallback?.Invoke($"<color=white>[ Invalid ]</color>  {command} ", Color.grey);
                return;
            }
            
            // prints if the command is not hidden
            if (!Commands[split[0]].IsHidden())
            {
                PrintToConsoleCallback?.Invoke($"[ Command ]  {command} ", Color.white);
            }
            
            Commands[split[0]].Execute(split.Skip(1).ToArray());
        }

        private static void ProcessLog(string message, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                {
                    var msg = $"<color=white>[ {type} ]</color>  {message}";
                    PrintToConsoleCallback?.Invoke(msg, Color.white);
                    break;
                }
                case LogType.Warning:
                {
                    var msg = $"<color=yellow>[ Warn ]</color>  {message}";
                    PrintToConsoleCallback?.Invoke(msg, Color.white);
                    break;
                }
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                {
                     var msg = $"<color=red>[ {type} ]</color>  {message}";
                     PrintToConsoleCallback?.Invoke(msg, Color.white);
                     break;
                }
            }
        }
    }
}

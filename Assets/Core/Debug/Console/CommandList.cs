using System.Collections.Generic;
using Console.Commands;

namespace Console
{
    public static class CommandList
    {
        public static List<IConsoleCommand> List;

        static CommandList()
        {
            List = new List<IConsoleCommand>
            {
                // general commands
                new HelpCommand(),
                new QuitCommand(),
                new RollCommand(),
                new MouseLockCommand(),
                
                // scene transitions
                new SceneCommand(),
                
                // networking commands
                
                // debug commands
                new KillCommand(),
                new KillAllCommand(),
                new QuickWin(),
                new PauseTimerCommand(),
                new InstantCaptureCommand(),
                new GodModeCommand(),
                new TeleportCommand(),
                new NoRecoilCommand(),
                new InfiniteCurrencyCommand(),
                new UnlimitedAmmoCommand(),
            };
        }
    }
}

using Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console.Commands
{
    public class SceneCommand : IConsoleCommand
    {
        // command args
        private const string Splash = "splash";
        private const string Menu = "menu";
        private const string Game = "game";
        private const string Test = "test";
        
        // scene names
        private const string SplashScene = "SplashScene";
        private const string MenuScene = "MenuScene";
        private const string GameScene = "GameScene";
        private const string TestScene = "TestGameScene";

        public string Name() => "scene";

        public string Description() => "/Scene <name>  :  Forces a move to the specified scene. \n"
                                       + " [ Args ] \"splash\", \"menu\", \"game\" ";

        public bool IsHidden() => false;

        public async void Execute(string[] args)
        {
            if (args.Length != 1)
            {
                var error = "[ Invalid ]  /Scene <name>  :  Takes 1 argument.";
                DeveloperConsole.Print(error);
                return;
            }

            var scene = "";
                
            switch (args[0])
            {
                case Splash:
                    scene = SplashScene;
                    break;
                
                case Menu:
                    scene = MenuScene;
                    break;
                
                case Game:
                    scene = GameScene;
                    break;

                case Test:
                    scene = TestScene;
                    break;

                default:
                {
                    var error = $"[ Invalid ]  <color=Red>\"{args[0]}\"</color> is not a valid scene.";
                    DeveloperConsole.Print(error);
                    return;
                }
            }

            var net = NetworkManager.Singleton;
            if (net.IsServer)
            {
                var id = NetworkManager.Singleton.LocalClientId;
                NetworkManager.Singleton.DisconnectClient(id);
            }
            else 
            {
                var msg = "Only the Host can change the scene";
                Debug.LogWarning(msg);
                return;
            }

            if (Application.isPlaying)
            {
                var coroutine = SceneLoader.NetworkLoadSceneCoroutine(scene);
                NetworkManager.Singleton.StartCoroutine(coroutine);
            }
        }
    }
}
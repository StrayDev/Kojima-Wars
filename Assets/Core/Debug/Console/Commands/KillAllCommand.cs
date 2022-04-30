using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Console.Commands
{
    public class KillAllCommand : IConsoleCommand
    {
        public string Name() => "killall";

        public string Description() => "kills all players";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                CombatComponent combatComponent = players[i].GetComponent<CombatComponent>();
                combatComponent.TakeDamage(combatComponent.MaxHealthValue + combatComponent.MaxShieldsValue);
                players[i].GetComponent<MechCharacterController>().KillMech();
            }
        }
    }
}
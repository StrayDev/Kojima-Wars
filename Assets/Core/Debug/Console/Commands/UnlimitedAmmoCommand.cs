using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Console.Commands
{
    public class UnlimitedAmmoCommand : IConsoleCommand
    {
        public string Name() => "ammogalore";

        public string Description() => "unlimited ammo";

        public bool IsHidden() => false;

        public void Execute(string[] args)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            {

                if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    GameObject.FindGameObjectsWithTag("Player")[i].transform.GetComponentInChildren<WeaponScript>().infiniteAmmo = true;
                }
            }
        }
    }
}

using System.Collections.Generic;
using Networking;
using Unity.Netcode;
using UnityEngine;

public class PlayerObject
{
    public bool IsMech = true;
    public GameObject Mech;
    public GameObject VTOL;
}

/// <summary>
/// This class manages the transformation between the player forms
/// To use this the player needs to register itself using RegisterPlayer()
/// 
/// You can then change form using SwitchMode()
/// Note SwitchMode will set the input  
/// </summary>

public static class TransformManager
{
    private static Dictionary<ulong, PlayerObject> playerObjects;

    static TransformManager()
    {
        playerObjects = new Dictionary<ulong, PlayerObject>();
    }

    public static void RegisterPlayer(ulong id, GameObject mech, GameObject vtol)
    {
        // create the player data
        var player = new PlayerObject();

        // set refrences
        player.Mech = mech;
        player.VTOL = vtol;

        // set disabled by default
        //player.VTOL.SetActive(false);
        //player.VTOL.GetComponent<NetworkedModeSwitcher>().SetActive(false);

        // register to the list
        playerObjects.Add(id, player);
    }

    [ServerRpc(RequireOwnership = false)]
    public static void ToggleMechModeServerRpc(ulong id)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var player = playerObjects[id];
        SwitchMode(player);
    }

    private static void SwitchMode(PlayerObject player)
    {
        // sets the position of the new form to current position
        AlignObjectTransforms(player);

        // toggles the active gameobject
        var isMech = player.IsMech;

        SetActiveGameObject(player, isMech);
        
        // sets the active control type
        var controlls = isMech ? ControlType.VTOL : ControlType.MECH;
        InputManager.SetInputType(controlls);

        // set bool for type
        player.IsMech = !isMech;
    }

    private static void SetActiveGameObject(PlayerObject player, bool isMech)
    {
/*        if (isMech)
        {
            player.Mech.GetComponent<NetworkedModeSwitcher>().SetActive(!isMech);
            player.VTOL.GetComponent<NetworkedModeSwitcher>().SetActive(isMech);
        }
        else
        {
            player.VTOL.GetComponent<NetworkedModeSwitcher>().SetActive(isMech);
            player.Mech.GetComponent<NetworkedModeSwitcher>().SetActive(!isMech);
        }*/
    }

    private static void AlignObjectTransforms(PlayerObject player)
    {
        if (player.IsMech)
        {
            player.VTOL.transform.position = player.Mech.transform.position;
            player.VTOL.transform.rotation = player.Mech.transform.rotation;
        }
        else
        {
            player.Mech.transform.position = player.VTOL.transform.position;
            player.Mech.transform.rotation = player.VTOL.transform.rotation;
        }
    }

    public static GameObject GetMechFromClientID(ulong id)
    {
        return playerObjects[id].Mech;
    }

    public static GameObject GetVTOLFromClientID(ulong id)
    {
        return playerObjects[id].VTOL;
    }

    public static Entity GetActiveEntityFromClientID(ulong id)
    {
        if (playerObjects[id].IsMech)
        {
            return playerObjects[id].Mech.GetComponent<Entity>();
        }
        else
        {
            return playerObjects[id].VTOL.GetComponent<Entity>();
        }
    }
}

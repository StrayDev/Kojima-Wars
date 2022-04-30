using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class UnitGroup
{
    public List<GameObject> units;
    public int index;
}

public class UnitManager : MonoBehaviour
{
    public ulong playerId = 0;
    private int index = 1;
    public int unitIndex = 0;
    public List<GameObject> activeGroups;

    public List<SelectGroup> groups = new List<SelectGroup>();

    private void Update()
    {
        if (playerId == 0)
        {
            playerId = GetComponent<NetworkObject>().NetworkObjectId;
        }
        
        if (groups.Count < 10)
        {
            foreach (var buttons in FindObjectsOfType<SelectGroup>())
            {
                if (buttons.name == "Group " + index)
                {
                    groups.Add(buttons);
                    index++;
                }
            }
        }
    }

}

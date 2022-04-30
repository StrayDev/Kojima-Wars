using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefenceLocationInfo
{
    [Header("Location Information")]
    public Vector3 location;

    [Header("Unit Information")]
    public GameObject unit;
    public GameObject enemyUnit;
    public int groupId = -1;
    public int enemyGroupId = -1;

    [Header("Check Information")]
    public bool occupied = false;
    public bool enemyOccupied = false;
}

public class UnitDefenceLocation : MonoBehaviour
{
    public List<DefenceLocationInfo> defenceLocationInfo = new List<DefenceLocationInfo>();


    void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            defenceLocationInfo.Add(new DefenceLocationInfo()); 
            defenceLocationInfo[i].location = transform.GetChild(i).position;
        }
    }

    private void Update()
    {
        foreach(var locations in defenceLocationInfo)
        {
            if(locations.occupied)
            {
                if(locations.unit == null)
                {
                    locations.occupied = false;
                }
            }
        }
    }
}

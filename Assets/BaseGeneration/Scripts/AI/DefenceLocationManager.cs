using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceLocationManager : MonoBehaviour
{
    public List<UnitDefenceLocation> unitDefenceLocations = new List<UnitDefenceLocation>();

    void Start()
    {
        foreach(Transform child in transform)
        {
            unitDefenceLocations.Add(child.GetComponent<UnitDefenceLocation>());
        }
    }




}

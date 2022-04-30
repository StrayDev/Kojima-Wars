using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BaseDefencesController : NetworkBehaviour
{
    public BaseDefenceLocation[] DefenceLocations => m_defenceLocations;

    [SerializeField] private BaseDefenceLocation[] m_defenceLocations = { };

    public void DestroyAllDefences()
    {
        foreach (BaseDefenceLocation location in m_defenceLocations)
        {
            location.DestroyDefenceObject();
        }
    }

    public int GetDefenceIndex(BaseDefenceLocation location)
    {
        for(int i = 0; i < m_defenceLocations.Length; ++i)
        {
            if (m_defenceLocations[i] == location) return i;
        }
        return -1;
    }
}
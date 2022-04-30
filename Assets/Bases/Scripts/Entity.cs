using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Entity : NetworkBehaviour
{
    public enum EEntityType
    {
        PLAYER,
        UNIT,
    }

    public string TeamName => m_teamName;
    public ulong clientId;
    public string Name => m_name;


    public EEntityType EntityType => m_entityType;

    [SerializeField] private string m_teamName;
    [SerializeField] private EEntityType m_entityType;

    [SerializeField] private string m_name;
    [HideInInspector] public Collider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<Collider>();
    }

    [ClientRpc]
    public void ChangeTeamClientRpc(FixedString32Bytes newTeam)
    {
        m_teamName = newTeam.ConvertToString();

        if (m_entityType == EEntityType.PLAYER)
        {
            if (gameObject.GetComponent<PlayerInformation>() != null)
            {
                gameObject.GetComponent<PlayerInformation>().SetTeamName(m_teamName);
            }

            if (gameObject.GetComponent<MechCharacterController>() != null)
            {
                gameObject.GetComponent<MechCharacterController>().UpdatePlayerMaterial(m_teamName);
            }
        }
    }

    [ClientRpc]
    public void SetNameClientRpc(FixedString32Bytes name)
    {
        if (name != "")
        {
            m_name = name.ConvertToString();
        }
        else
        {
            m_name = "Null";
        }
    }

    [ClientRpc]
    public void SetClientIDClientRpc(ulong id)
    {
        clientId = id;
    }
}
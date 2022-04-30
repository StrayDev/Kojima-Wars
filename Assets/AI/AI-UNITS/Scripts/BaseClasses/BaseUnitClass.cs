using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitClass : Entity
{
    public enum TeamType
    {
        NO_TEAM = -1,
        RED = 0,
        BLUE
    };

    public virtual EUnitTypes UnitType
    {
        get;
        private set;
    }

    // players stats
    private string unitName = "";
    private string unitDescription = "";

    public TeamType team = TeamType.NO_TEAM;    // public for debug

    //Stats
    private int stamina = 0;
    public float health = 0;
    private int speed = 0;
    private int strength = 0;

    public string UnitName { get { return unitName; } set { unitName = value; } }

    public string UnitDescription { get { return unitDescription; } set { unitDescription = value; } }

    public int Stamina { get { return stamina; } set { stamina = value; } }

    public int Strength { get { return strength; } set { strength = value; } }

    public float Health { get { return health; } set { health = value; } }

    public int Speed { get { return speed; } set { speed = value; } }

    public TeamType Team { get { return team; } set { team = value; } }

/*    public bool DoDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            return true;
        }
        return false;
    }*/

/*
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRPC(int damage)
    {
        Health -= damage;
        if (Health <= 0.0f) Destroy(gameObject);
    }

    [ServerRpc]
    public void HealServerRpc(int amount)
    {
        Health += amount;
    }

    public bool IsAlive()
    {
        return Health > 0.0f;
    }*/
}

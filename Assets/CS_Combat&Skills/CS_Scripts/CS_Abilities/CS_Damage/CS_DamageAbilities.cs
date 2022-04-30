using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageAbilities", menuName = "New Ability/DamageAbilities")]
public class CS_DamageAbilities : AbilitySO
{
    public enum AbilityName
    {
        PlasmaCannon,
        SMRTGrenade,
        Airstrike,
        AOEStun,
        RingofFire,
        ElectricSmoke,
        EMPBolt,
        SAMTurret,
        EMPBlast,
    }
    public AbilityName id;


/*    public string abilityName;
    public float cooldownTime;
    public float activeTime;
    public GameObject instantiatedObject;*/

/*    public override void CastAbility(Entity caster)
    {
        Type thistype = this.GetType();
        MethodInfo theMethod = thistype.GetMethod(id.ToString());
        theMethod.Invoke(this, null);
        AbilityPrefabSpawner.Instance.SpawnAbilityObjectServerRpc(id, new NetworkObjectReference(caster.gameObject));
    }*/

    
/*    public void PlasmaCannon()
    {
        Debug.Log("Plasma Cannon");
    }
    public void SMRTGrenade()
    {
        Debug.Log("SMRT Grenade");

    }
    public void Airstrike()
    {
        Debug.Log("Airstrike");

    }
    public void AOEStun()
    {
        Debug.Log("AOEStun");
    }
    public void RingofFire()
    {
        Debug.Log("Ring Of Fire");
    }
    public void ElectricSmoke()
    {
        Debug.Log("ElectricSmoke");
    }
    public void EMPBolt()
    {
        Debug.Log("EMPBolt");
    }
    public void SAMTurret()
    {
        Debug.Log("SAMTurret");
    }
    public void EMPBlast()
    {
        Debug.Log("EMPBlast");
    }*/
    
}

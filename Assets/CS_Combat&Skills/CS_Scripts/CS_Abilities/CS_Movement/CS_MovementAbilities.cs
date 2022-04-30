using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementAbilities", menuName = "New Ability/MovementAbilities")]
public class CS_MovementAbilities : AbilitySO
{
    public enum AbilityName
    {
        Dash,
        RepulsionBlast,
        GroundSlam,
        Silence,
        Cloak,
        Recall,
        SpeedBoost,
        ShortTP,
        BaseTP
    }
    public AbilityName id;

/*    public string abilityName;
    public float cooldownTime;
    public float activeTime;
    public GameObject instantiatedObject;*/

    public bool BasesNotSpawned = true;


/*    public override void CastAbility(Entity caster)
    {
        Type thistype = this.GetType();
        MethodInfo theMethod = thistype.GetMethod(id.ToString());
        theMethod.Invoke(this, null);
        AbilityPrefabSpawner.Instance.SpawnAbilityObjectServerRpc(id, new NetworkObjectReference(caster.gameObject));
    }*/

    public void Dash()
    {
        Debug.Log("DASH");
    }
    public void RepulsionBlast()
    {
    }
    public void GroundSlam()
    {
    }
    public void Silence()
    {
    }
    public void Cloak()
    {
    }
    public void Recall()
    {
    }
    public void SpeedBoost()
    {
    }
    public void ShortTP()
    {
    }

    public void BaseTP()
    {
        /*if (BasesNotSpawned)
        {
            Debug.Log("True");
            Instantiate(instantiatedObject);
            BasesNotSpawned = false;
        }*/
    }
}

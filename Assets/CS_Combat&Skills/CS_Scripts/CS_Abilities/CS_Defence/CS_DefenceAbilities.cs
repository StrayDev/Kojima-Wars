using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenceAbilities", menuName = "New Ability/DefenceAbilities")]
public class CS_DefenceAbilities : AbilitySO
{
    public enum AbilityName
    {
        OrdnancePack,
        BubbleShield,
        ShieldRegen,
        HealingField,
        BioticGrenade,
        HealingBullets,
        SmokeGrenade,
        NineBang,
        DropShield
    }
    public AbilityName id;

    /*  public string abilityName;
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

    public void OrdnancePack()
    {
//        Debug.Log("Pack Given");
    }
    public void BubbleShield()
    {
 //       Debug.Log("Bubbleshield");
    }
    public void ShieldRegen()
    {
 //       Debug.Log("Bubble Barrier");
    }
    public void HealingField()
    {
  //      Debug.Log("Healing field");
    }
    public void BioticGrenade()
    {
  //      Debug.Log("Healing Grenade");
    }
    public void SmokeGrenade()
    {
    }
    public void NineBang()
    {
    }
    public void DropShield()
    {
    }
   
}

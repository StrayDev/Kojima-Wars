using System;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ReconAbilities", menuName = "New Ability/ReconAbilities")]
public class CS_ReconAbilities : AbilitySO
{
    public enum AbilityName
    {
        MotionSensor,
        Drone,
        UAV,
        Lockdown,
        CloneTrap,
        AOESlow,
        ReconGrenade,
        NeuralTheft,
        NeuralAdaptation
    }
    public AbilityName id;

    public bool droneActive;
   
    public void MotionSensor()
    {
    }
    public void UAV()
    {

    }
    public void Trap()
    {
    }
    public void CloneTrap()
    {
    }
    public void AOESlow()
    {
    }
    public void ReconGrenade()
    {
    }
    public void NeuralTheft()
    {
    }
    public void NeuralAdaptation()
    {
    }
}

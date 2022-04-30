using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech_IdleState : IState
{
    private MechControllerScript _mech;

    public Mech_IdleState(MechControllerScript owner)
    {
        _mech = owner;
    }

    public void OnStateEnter() { Debug.Log("<color=orange>Entering Mech idle state</color>"); }
    public void OnStateUpdate() { Debug.Log("<color=orange>Executing Mech idle state</color>"); }
    public void OnStateExit() { Debug.Log("<color=orange>Exiting Mech idle state</color>"); }
}
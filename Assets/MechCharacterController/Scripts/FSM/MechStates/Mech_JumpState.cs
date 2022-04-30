using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech_JumpState : IState
{
    private MechControllerScript _mech;

    public Mech_JumpState(MechControllerScript owner)
    {
        _mech = owner;
    }

    public void OnStateEnter() { Debug.Log("<color=yellow>Entering Mech jump state</color>"); }
    public void OnStateUpdate() { Debug.Log("<color=yellow>Executing Mech jump state</color>"); }
    public void OnStateExit() { Debug.Log("<color=yellow>Exiting Mech jump state</color>"); }
}
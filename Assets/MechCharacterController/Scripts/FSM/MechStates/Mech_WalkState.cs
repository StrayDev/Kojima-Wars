using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech_WalkState : IState
{
    private MechControllerScript _mech;

    public Mech_WalkState(MechControllerScript owner)
    {
        _mech = owner;
    }

    public void OnStateEnter() { Debug.Log("<color=lime>Entering Mech walk state</color>"); }
    public void OnStateUpdate() { Debug.Log("<color=lime>Executing Mech walk state</color>"); }
    public void OnStateExit() { Debug.Log("<color=lime>Exiting Mech walk state</color>"); }
}

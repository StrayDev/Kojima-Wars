using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTankClass : BaseUnitClass
{
    public override EUnitTypes UnitType => EUnitTypes.TANK;
    public BaseTankClass()
    {
        UnitName = "Tank";
        UnitDescription = "Armoured ground fighter";
        Stamina = 10;
        Strength = 15;
        Health = 10;
        Speed = 15;
    }
}

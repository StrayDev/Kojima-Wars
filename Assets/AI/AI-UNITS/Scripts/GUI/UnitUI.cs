using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public Text ClassName;
    public Text ClassDescription;
    public Text StaminaValue;
    public Text StrengthValue;
    public Text HealthValue;
    public Text SpeedValue;

    public bool inSoldier;
    public bool inTank;
    public bool inPlane;

    public bool inAntiTankSoldier;
    public bool inAntiAirSoldier;

    public bool inHeavyTank;

    public bool inAntiAirTruck;

    public bool inScoutHelecopter;


    //private BaseUnitClass class1 = new BaseTankClass();
    //private BaseUnitClass class2 = new BasePlaneClass();
    //private BaseUnitClass class3 = new BaseInfantryClass();

    //private BaseUnitClass class4 = new BaseHeavyTankClass();
    //private BaseUnitClass class5 = new BaseScoutHelecopterClass();
    //private BaseUnitClass class6 = new BaseAntiTankClass();

    //private BaseUnitClass class7 = new BaseAntiAirTruck();
    //private BaseUnitClass class8 = new BaseAntiAirClass();


    void Start()
    {
        /*
        ClassName.text = "";
        ClassDescription.text = "";
        StaminaValue.text = "";
        StrengthValue.text = "";
        HealthValue.text = "";
        SpeedValue.text = "";
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
        */
    }

    //void Update()
    //{
    //    if (inTank)
    //    {
    //        ClassName.text = "Name: " + class1.UnitName;
    //        ClassDescription.text = "Desciption: " + class1.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class1.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class1.Strength.ToString();
    //        HealthValue.text = "Health: " + class1.Health.ToString();
    //        SpeedValue.text = "Speed: " + class1.Speed.ToString();
    //    }
    //    if (inPlane)
    //    {
    //        ClassName.text = "Name: " + class2.UnitName;
    //        ClassDescription.text = "Desciption: " + class2.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class2.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class2.Strength.ToString();
    //        HealthValue.text = "Health: " + class2.Health.ToString();
    //        SpeedValue.text = "Speed: " + class2.Speed.ToString();
    //    }
    //    if (inSoldier)
    //    {
    //        ClassName.text = "Name: " + class3.UnitName;
    //        ClassDescription.text = "Desciption: " + class3.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class3.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class3.Strength.ToString();
    //        HealthValue.text = "Health: " + class3.Health.ToString();
    //        SpeedValue.text = "Speed: " + class3.Speed.ToString();
    //    }
    //    if (inHeavyTank)
    //    {
    //        ClassName.text = "Name: " + class4.UnitName;
    //        ClassDescription.text = "Desciption: " + class4.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class4.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class4.Strength.ToString();
    //        HealthValue.text = "Health: " + class4.Health.ToString();
    //        SpeedValue.text = "Speed: " + class4.Speed.ToString();
    //    }
    //    if (inScoutHelecopter)
    //    {
    //        ClassName.text = "Name: " + class5.UnitName;
    //        ClassDescription.text = "Desciption: " + class5.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class5.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class5.Strength.ToString();
    //        HealthValue.text = "Health: " + class5.Health.ToString();
    //        SpeedValue.text = "Speed: " + class5.Speed.ToString();
    //    }
    //    if (inAntiTankSoldier)
    //    {
    //        ClassName.text = "Name: " + class6.UnitName;
    //        ClassDescription.text = "Desciption: " + class6.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class6.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class6.Strength.ToString();
    //        HealthValue.text = "Health: " + class6.Health.ToString();
    //        SpeedValue.text = "Speed: " + class6.Speed.ToString();
    //    }
    //    if (inAntiAirSoldier)
    //    {
    //        ClassName.text = "Name: " + class8.UnitName;
    //        ClassDescription.text = "Desciption: " + class8.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class8.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class8.Strength.ToString();
    //        HealthValue.text = "Health: " + class8.Health.ToString();
    //        SpeedValue.text = "Speed: " + class8.Speed.ToString();
    //    }
    //    if (inAntiAirTruck)
    //    {
    //        ClassName.text = "Name: " + class7.UnitName;
    //        ClassDescription.text = "Desciption: " + class7.UnitDescription;
    //        StaminaValue.text = "Stamina: " + class7.Stamina.ToString();
    //        StrengthValue.text = "Strength: " + class7.Strength.ToString();
    //        HealthValue.text = "Health: " + class7.Health.ToString();
    //        SpeedValue.text = "Speed: " + class7.Speed.ToString();
    //    }
    //}

    public void Soldier()
    {
        inSoldier = true;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }

    public void Tank()
    {
        inSoldier = false;
        inTank = true;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }

    public void Plane()
    {
        inSoldier = false;
        inTank = false;
        inPlane = true;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }

    public void AntiAirSoldier()
    {
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = true;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }

    public void HeavyTank()
    {
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = true;
        inAntiAirTruck = false;
    }

    public void ScoutHelecopter()
    {
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = true;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }

    public void AntiAirTruck()
    {
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = false;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = true;
    }

    public void AntiTankSoldier()
    {
        inSoldier = false;
        inTank = false;
        inPlane = false;
        inAntiAirSoldier = false;
        inAntiTankSoldier = true;
        inScoutHelecopter = false;
        inHeavyTank = false;
        inAntiAirTruck = false;
    }
}

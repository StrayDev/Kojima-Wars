using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_WeaponSelect : MonoBehaviour
{
    public WeaponStats assaultRifle;
    public WeaponStats burstRifle;
    public WeaponStats DMR;
    public WeaponStats sniperRifle;
    public WeaponStats SMG;
    public WeaponStats pistol;

    public WeaponScript weaponScript;

    public void AssignWeapon(WeaponStats weapon)
    {
        weaponScript.stats = weapon;
        weaponScript.bulletsLeft = weaponScript.stats.magazineSize;
        weaponScript.bulletsShot = 0;
        //weaponScript.AssignModel();
    }
}

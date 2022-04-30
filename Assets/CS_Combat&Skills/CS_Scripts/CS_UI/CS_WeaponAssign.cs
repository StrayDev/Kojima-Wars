using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CS_WeaponAssign : MonoBehaviour
{
    private CS_WeaponSelect ws;

    public WeaponStats weapon;
    public Sprite weaponSprite;
    public string weaponName;

    public Image selectedWeaponImage;
    public TMP_Text selectedWeaponText;

    private void Start()
    {
        //ws = GameObject.FindGameObjectWithTag("Player").GetComponent<CS_WeaponSelect>();
        weaponName = weapon.weaponName;
    }

    public void AssignWeapon()
    {
        ws.AssignWeapon(weapon);
        selectedWeaponImage.sprite = weaponSprite;
        selectedWeaponText.text = weaponName;
    }
}

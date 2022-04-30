using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "New Weapon/Weapons")]
public class WeaponStats : ScriptableObject
{
    public enum ReloadType { Magazine, Single }
    public enum FiringMode { Default, Burst, Shotgun, Projectile }
    
    
    public string weaponName;
    public string weaponDescription;

    [Space(4)] [Header("Damage Per Second")] [Space(4)]
    [Tooltip("X Axis: Distance (m)\nY Axis: Damage")]
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0.0F, 20.0F, 1000.0F, 0.0F);
    public float damage => damageFalloff.Evaluate(0);
    [Tooltip("The number of bullets fired per second.")] [Range(0.0F, 30.0F)] 
    public float fireRate = 10.0F; 
    
    [Space(4)] [Header("Ammo and Reloading")] [Space(4)] 
    [Tooltip("The maximum number of bullets in the magazine at once.")]
    public int magazineSize = 20;
    [Tooltip("How this weapon reloads.")] 
    public ReloadType reloadType = ReloadType.Magazine;
    [Tooltip("How many seconds it takes to reload.")] [Range(0.0F, 3.0F)]
    public float reloadTime = 1.0F;
    [Tooltip("Whether this weapon reloads automatically when empty.")]
    public bool automaticallyReload = false;

    [Space(4)] [Header("Aiming Down Sights")] [Space(4)] 
    [Tooltip("Whether or not you can actually aim down the sights of this weapon")]
    public bool canADS = true;
    [Tooltip("How many seconds it takes to aim down sights.")] [Range(0.0F,1.0F)]
    public float ADSTime = 0.2F;
    [Tooltip("How much the camera FOV decreases when aiming down sights")] [Range(0.0F, 50.0F)]
    public float ADSZoom = 20.0F;

    [Space(4)][Header("Recoil")][Space(4)]
    [Tooltip("The maximum spread angle when hip-fired.")] [Range(0.0F,10.0F)]
    public float hipfireSpread = 1.5F;
    [Tooltip("The maximum spread angle when aiming down sights.")] [Range(0.0F, 10.0F)]
    public float ADSSpread = 0.75F;
    [Space(4)]
    
    [Tooltip("The recoil rotation when hip-firing. (x: horizontal, y: vertical, z: tilt)")]
    public Vector3 hipfireRecoil = new Vector3(1.0F, 0.75F, 0.1F);
    [Tooltip("The recoil rotation when aiming down sights. (x: horizontal, y: vertical, z: tilt)")]
    public Vector3 ADSRecoil = new Vector3(0.6F, 0.5F, 0.1F);
    
    [Space(4)]
    [Tooltip("How quickly the camera recoils after firing.")][Range(0.0F,100.0F)]
    public float recoilSnappiness = 100.0F;
    [Tooltip("The rate at which the camera returns to its original position at a time after firing.")]
    public AnimationCurve recoilRecoveryCurve = AnimationCurve.EaseInOut(0.1F, 0.0F, 0.2F, 10.0F);
    
    [Space(4)][Header("Weapon Model")][Space(4)]
    [FormerlySerializedAs("gunModel")] public GameObject weaponModel;
    public Sprite icon;
    [FMODUnity.EventRef] public string shootSound;
    [FMODUnity.EventRef] public string reloadSound;
    
    [Space(4)] [Header("Firing Mode")] [Space(4)]
    public bool automatic = true;
    public FiringMode firingMode = FiringMode.Default;
    
    // Burst Mode
    public int bulletsPerBurst = 3;
    [Range(0.0F, 5.0F)]
    public float timeBetweenBursts = 1.0F;
    
    // Shotgun Mode
    public int numberOfPellets = 10;
    
    // Projectile Mode
    public GameObject projectile;
}
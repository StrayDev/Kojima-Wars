using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Create UI Audio Data", fileName = "UIAudioData")]
public class UIAudioEvents : ScriptableObject
{
    [Header("Combat")]
    [FMODUnity.EventRef]
    public string hitmarker;
    [FMODUnity.EventRef]
    public string damageTaken;
    [FMODUnity.EventRef]
    public string missileLockedOnSelf;
    [FMODUnity.EventRef]
    public string missileLockedOnEnemy;
    [FMODUnity.EventRef]
    public string slowed;


    [Header("Objectives")]
    [FMODUnity.EventRef]
    public string baseCaptured;
    [FMODUnity.EventRef]
    public string baseLost;
    [FMODUnity.EventRef]
    public string baseCapturing;
    [FMODUnity.EventRef]
    public string baseContested;


    [Header("Team")]
    [FMODUnity.EventRef]
    public string teamVictory;
    [FMODUnity.EventRef]
    public string teamLoss;
}

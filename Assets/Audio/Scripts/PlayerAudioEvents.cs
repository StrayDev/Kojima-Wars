using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Create Player Audio Data", fileName = "PlayerAudioData")]
public class PlayerAudioEvents : ScriptableObject
{
    [Header("Ground Movement")]
    [FMODUnity.EventRef]
    public string footstep = null;
    [FMODUnity.EventRef]
    public string jump = null;
    [FMODUnity.EventRef]
    public string land = null;
    [FMODUnity.EventRef]
    public string dash = null;


    [Header("Water Movement")]
    [FMODUnity.EventRef]
    public string enterWater = null;
    [FMODUnity.EventRef]
    public string waterMovement = null;
    [FMODUnity.EventRef]
    public string exitWater = null;


    [Header("Air Movement")]
    [FMODUnity.EventRef]
    public string takeOff = null;
    [FMODUnity.EventRef]
    public string landing = null;
    [FMODUnity.EventRef]
    public string airMovement = null;

    [Header("General Audio")]
    [FMODUnity.EventRef]
    public string transformation = null;
    [FMODUnity.EventRef]
    public string death = null;
    [FMODUnity.EventRef]
    public string respawn = null;
}

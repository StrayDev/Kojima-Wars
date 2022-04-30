using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Create Environmental Audio Data", fileName = "EnvironmentAudioData")]
public class EnvironmentAudioEvents : ScriptableObject
{

    [FMODUnity.EventRef]
    public string oceanWaveSplash;
    [FMODUnity.EventRef]
    public string oceanWaveCrash;


    [FMODUnity.EventRef]
    public string windWhistle;
    [FMODUnity.EventRef]
    public string windTrees;
}

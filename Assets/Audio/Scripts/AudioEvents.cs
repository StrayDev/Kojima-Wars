using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Create Global Audio Data Store", fileName = "GlobalAudioData")]
public class AudioEvents : ScriptableObject
{
    public PlayerAudioEvents playerAudioEvents;
    public CombatAudioEvents combatAudioEvents;
    public EnvironmentAudioEvents environmentAudioEvents;
    public UIAudioEvents uIAudioEvents;
}

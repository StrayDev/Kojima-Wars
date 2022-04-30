using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AppData", menuName = "Scriptable Objects/AppData")]
public class AppDataSO : ScriptableObject
{
    // settings for individual players
    [Header("PLAYER SETTINGS", order = 0)]
    [Space(2)]
    [Header("Default values", order = 1)]
    public float defaultMousePitchSensitivity = .25f;
    public float defaultMouseYawSensitivity   = .25f;

    public float defaultFieldOfView = 60f;

    public float defaultMasterVolume = 50f;
    public float defaultMusicVolume  = 100f;
    public float defaultFXVolume     = 100f;

    public bool defaultInvertedFlightControls = false;

    [Header("Current values", order = 2)]
    public float mousePitchSensitivity  = .25f;
    public float mouseYawSensitivity    = .25f;

    public float fieldOfView = 1f;

    public float masterVolume = 50f;
    public float musicVolume  = 100f;
    public float fXVolume     = 100f;

    public bool invertedFlightControls = false;
}

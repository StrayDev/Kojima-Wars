using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettingsData
{
    public float horizontalSensitivity;
    public float verticalSensitivity;
    public float fov;
    public float masterVolume;
    public float musicVolume;
    public float fXVolume;
    public bool  invertedFlightControls;

    public GameSettingsData(AppDataSO appData)
    {
        horizontalSensitivity = appData.mousePitchSensitivity;
        verticalSensitivity   = appData.mouseYawSensitivity;
        fov                   = appData.fieldOfView;
        masterVolume          = appData.masterVolume;
        musicVolume           = appData.musicVolume;
        fXVolume              = appData.fXVolume;
        invertedFlightControls= appData.invertedFlightControls;
    }
}

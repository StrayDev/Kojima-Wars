using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TMP_Text horizontalSensitivityValueText, verticalSensitivityValueText, fieldOfViewValueText, masterVolumeValueText, musicVolumeValueText, fXVolumeValueText;
    [SerializeField] private Slider horizontalSensitivitySlider, verticalSensitivitySlider, fieldOfViewSlider, masterVolumeSlider, musicVolumeSlider, fXVolumeSlider;
    [SerializeField] private Toggle invertedControlsToggle;

    [SerializeField] private AppDataSO appData = default;

    private void Start()
    {
        LoadSettings();
        //SetDefaultSettings();
    }

    public void SetHorizontalSensitivity(float sensitivity)
    {
        horizontalSensitivityValueText.text = sensitivity.ToString();
        appData.mousePitchSensitivity = sensitivity / 100f;
    }
    public void SetVerticalSensitivity(float sensitivity)
    {
        verticalSensitivityValueText.text = sensitivity.ToString();
        appData.mouseYawSensitivity = sensitivity / 100f;
    }
    public void SetFOV(float fov)
    {
        fieldOfViewValueText.text = fov.ToString();
        appData.fieldOfView = fov;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolumeValueText.text = volume.ToString();
        appData.masterVolume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        musicVolumeValueText.text = volume.ToString();
        appData.musicVolume = volume;
    }
    public void SetFXVolume(float volume)
    {
        fXVolumeValueText.text = volume.ToString();
        appData.fXVolume = volume;
    }

    public void SetInvertedFlightControls(bool inverted)
    {
        //volumeValueText.text = volume.ToString();
        appData.invertedFlightControls = inverted;
    }

    public void SetDefaultSettings()
    {
        appData.mousePitchSensitivity  = appData.defaultMousePitchSensitivity;
        appData.mouseYawSensitivity    = appData.defaultMouseYawSensitivity;
        appData.fieldOfView            = appData.defaultFieldOfView;
        appData.masterVolume           = appData.defaultMasterVolume;
        appData.musicVolume            = appData.defaultMusicVolume;
        appData.fXVolume               = appData.defaultFXVolume;
        appData.invertedFlightControls = appData.defaultInvertedFlightControls;

        SetValues();
        SaveSettings();
    }

    public void SetValues()
    {
        horizontalSensitivitySlider.value = appData.mousePitchSensitivity * 100;
        verticalSensitivitySlider.value   = appData.mouseYawSensitivity * 100;
        fieldOfViewSlider.value           = appData.fieldOfView;
        masterVolumeSlider.value          = appData.masterVolume;
        musicVolumeSlider.value           = appData.musicVolume;
        fXVolumeSlider.value              = appData.fXVolume;
        invertedControlsToggle.isOn       = appData.invertedFlightControls;

        horizontalSensitivityValueText.text = (appData.mousePitchSensitivity * 100).ToString();
        verticalSensitivityValueText.text   = (appData.mouseYawSensitivity * 100).ToString();
        fieldOfViewValueText.text           = appData.fieldOfView.ToString();
        masterVolumeValueText.text          = appData.masterVolume.ToString();
        musicVolumeValueText.text           = appData.musicVolume.ToString();
        fXVolumeValueText.text              = appData.fXVolume.ToString();
    }

    public void SaveSettings()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.dat";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameSettingsData data = new GameSettingsData(appData);

        formatter.Serialize(stream, data);
        stream.Close();
        //Debug.Log("Game settings saved to " + path);
    }

    public void LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.dat";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameSettingsData loadedSettings = formatter.Deserialize(stream) as GameSettingsData;
            stream.Close();

            appData.mousePitchSensitivity  = loadedSettings.horizontalSensitivity;
            appData.mouseYawSensitivity    = loadedSettings.verticalSensitivity;
            appData.fieldOfView            = loadedSettings.fov;
            appData.masterVolume           = loadedSettings.masterVolume;
            appData.musicVolume            = loadedSettings.musicVolume;
            appData.fXVolume               = loadedSettings.fXVolume;
            appData.invertedFlightControls = loadedSettings.invertedFlightControls;

            //Debug.Log("Save file found in " + path + ", values set from save file");

            SetValues();
        }
        else
        {
            SetDefaultSettings();
            //Debug.Log("Save file not found in " + path + ", values set to default");
        }
    }
}

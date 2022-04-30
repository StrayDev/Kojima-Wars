using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    //Events & Instances
    public static AudioManager Instance;

    public AudioEvents events;
    private Dictionary<string, List<FMOD.Studio.EventInstance>> eventInstancePools;

    //Settings
    [SerializeField] private AppDataSO appdata;

    public FMOD.Studio.Bus masterVolume;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        eventInstancePools = new Dictionary<string, List<FMOD.Studio.EventInstance>>();

        masterVolume = FMODUnity.RuntimeManager.GetBus("bus:/Master");
    }

    private void Update()
    {
        masterVolume.setVolume(appdata.masterVolume / 100); //Normalized for FMOD (0.0 - 1.0)
    }

    public void CreateFMODInstance(string audioEvent, int size)
    {
        CreateFMODInstancePoolServerRpc(audioEvent, size);
    }

    [ServerRpc (RequireOwnership = false)]
    public void CreateFMODInstancePoolServerRpc(string audioEvent, int size)
    {
        CreateFMODInstancePoolClientRpc(audioEvent, size);
    }
    
    [ClientRpc]
    public void CreateFMODInstancePoolClientRpc(string audioEvent, int size)
    {
        if (eventInstancePools.ContainsKey(audioEvent)) return;

        List<FMOD.Studio.EventInstance> newInstances = new List<FMOD.Studio.EventInstance>();
        for(int i = 0; i < size; ++i)
        {
            newInstances.Add(FMODUnity.RuntimeManager.CreateInstance(audioEvent));
            newInstances[i].set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(new Vector3(0,0,0)));
        }
        eventInstancePools.Add(audioEvent, newInstances);
    }

    public void PlayLocalFMODOneShot(string audioEvent, Vector3 position)
    {
        List<FMOD.Studio.EventInstance> eventInstances = eventInstancePools[audioEvent];
        FMOD.Studio.EventInstance instance = GetEventInstanceFromPool(eventInstances);
        UpdatePosition(instance, position);
        instance.start();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayFMODOneShotServerRpc(string audioEvent, Vector3 position)
    {
        PlayFMODOneShotClientRpc(audioEvent, position);
    }

    [ClientRpc]
    public void PlayFMODOneShotClientRpc(string audioEvent, Vector3 position)
    {
        PlayLocalFMODOneShot(audioEvent, position);
    }

    public List<FMOD.Studio.EventInstance> GetEventInstancePool(string audioEvent)
    {
        return eventInstancePools[audioEvent];
    }

    public FMOD.Studio.EventInstance GetEventInstanceFromPool(List<FMOD.Studio.EventInstance> pool)
    {
        foreach(var instance in pool)
        {
            FMOD.Studio.PLAYBACK_STATE state;
            instance.getPlaybackState(out state);
            if(state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                return instance;
            }
        }
        return new FMOD.Studio.EventInstance();
    }

    public FMOD.Studio.PLAYBACK_STATE GetPlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);

        return state;
    }

    public void StartFMODLoop(string audioEvent)
    {
        FMOD.Studio.EventInstance instance = GetEventInstanceFromPool(eventInstancePools[audioEvent]);
        instance.start();
    }

    public void EndFMODLoop(string audioEvent)
    {
        FMOD.Studio.EventInstance instance = GetEventInstanceFromPool(eventInstancePools[audioEvent]);
        instance.stop(STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void UpdatePosition(FMOD.Studio.EventInstance instance, Vector3 newPosition)
    {
        FMOD.ATTRIBUTES_3D attributes, newAttributes;
        instance.get3DAttributes(out attributes);
        newAttributes = FMODUnity.RuntimeUtils.To3DAttributes(newPosition);
        attributes.position = newAttributes.position;
        instance.set3DAttributes(attributes);
    }

    public void UpdatePosition(string audioEvent, Vector3 newPosition)
    {
        foreach (FMOD.Studio.EventInstance instance in AudioManager.Instance.GetEventInstancePool(audioEvent))
        {
            FMOD.ATTRIBUTES_3D attributes, newAttributes;
            instance.get3DAttributes(out attributes);
            newAttributes = FMODUnity.RuntimeUtils.To3DAttributes(newPosition);
            attributes.position = newAttributes.position;
            instance.set3DAttributes(attributes);
        }
    }

    public void SetAudioEventParameters(string audioEvent, string parameterID, float value)
    {
        foreach (FMOD.Studio.EventInstance instance in AudioManager.Instance.GetEventInstancePool(audioEvent))
        {
            instance.setParameterByName(parameterID, value);
        }
    }
}

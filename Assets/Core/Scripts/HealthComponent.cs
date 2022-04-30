using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine;

public class HealthComponent : NetworkBehaviour
{
    // event to be called when the health is set
    [SerializeField] private UnityEvent<int,int> OnValueChanged = default;

    // health value to be sychronized
    private int healthSync = default;


    // maintains a static list of all health components
    private static List<HealthComponent> List = new List<HealthComponent>();
    public override void OnNetworkSpawn() => List.Add(this);
    public override void OnNetworkDespawn() => List.Remove(this);


    // getters for setting the callback
    public UnityEvent<int, int> GetCallback() => OnValueChanged;

    public static ref UnityEvent<int,int> GetCallback(ulong ownerID)
    {
        if(!TryGetComponent(ownerID, out var component))
        {
            Debug.LogError("HealthComponent.GetCallback() is being called before the object has been spawned");
        }

        // return a ref to the callback
        return ref component.OnValueChanged;
    }

    // functions for geting the health value
    public int GetHealth() => healthSync;
    public static int GetHealth(ulong ownerID)
    {
        // returns health component value with matching id
        if (TryGetComponent(ownerID, out var component)) return component.healthSync; 
        // not able to find a matching health component
        Debug.LogError($"You are trying to GetHealth() before the player has spawned");
        return -1;
    }

    private static bool TryGetComponent(ulong ownerID, out HealthComponent component)
    {
        // get the component if it exists
        foreach (var c in List)
        {
            if (c.OwnerClientId != ownerID) continue;
            component = c;
            return true;
        }

        // set value 
        component = null;
        return false;
    }

    // functions for setting the health value
    public void SetHealth(int value) => SetHealthServerRpc(value);
    public static void SetHealth(ulong ownerID, int value)
    {
        // returns health component value with matching id
        if (TrySetComponentValues(ownerID, value)) return;

        // not able to find a matching health component
        Debug.LogError($"You are trying to SetHealth() before the player has spawned");
    }

    private static bool TrySetComponentValues(ulong ownerID, int value)
    {
        bool success = false;
        foreach (var component in List)
        {
            if (component.OwnerClientId != ownerID) continue;
            component.SetHealthServerRpc(value);
            success = true;
        }
        return success;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetHealthServerRpc(int value) => SetHealthClientRpc(value);

    [ClientRpc]
    private void SetHealthClientRpc(int value)
    {
        // set the value
        var old = healthSync;
        healthSync = value;
        OnValueChanged.Invoke(old, value);
    }

}

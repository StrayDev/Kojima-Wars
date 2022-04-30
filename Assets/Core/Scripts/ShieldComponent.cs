using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine;

public class ShieldComponent : NetworkBehaviour
{
    // event to be called when the Shield is set
    [SerializeField] private UnityEvent<int, int> OnValueChanged = default;

    // Shield value to be sychronized
    private int ShieldSync = default;


    // maintains a static list of all Shield components
    private static List<ShieldComponent> List = new List<ShieldComponent>();
    public override void OnNetworkSpawn() => List.Add(this);
    public override void OnNetworkDespawn() => List.Remove(this);


    // getters for setting the callback
    public UnityEvent<int, int> GetCallback() => OnValueChanged;

    public static ref UnityEvent<int, int> GetCallback(ulong ownerID)
    {
        if (!TryGetComponent(ownerID, out var component))
        {
            Debug.LogError("ShieldComponent.GetCallback() is being called before the object has been spawned");
        }

        // return a ref to the callback
        return ref component.OnValueChanged;
    }

    // functions for geting the Shield value
    public int GetShield() => ShieldSync;
    public static int GetShield(ulong ownerID)
    {
        // returns Shield component value with matching id
        if (TryGetComponent(ownerID, out var component)) return component.ShieldSync;
        // not able to find a matching Shield component
        Debug.LogError($"You are trying to GetShield() before the player has spawned");
        return -1;
    }

    private static bool TryGetComponent(ulong ownerID, out ShieldComponent component)
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

    // functions for setting the Shield value
    public void SetShield(int value) => SetShieldServerRpc(value);
    public static void SetShield(ulong ownerID, int value)
    {
        // returns Shield component value with matching id
        if (TrySetComponentValues(ownerID, value)) return;

        // not able to find a matching Shield component
        Debug.LogError($"You are trying to SetShield() before the player has spawned");
    }

    private static bool TrySetComponentValues(ulong ownerID, int value)
    {
        bool success = false;
        foreach (var component in List)
        {
            if (component.OwnerClientId != ownerID) continue;
            component.SetShieldServerRpc(value);
            success = true;
        }
        return success;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetShieldServerRpc(int value) => SetShieldClientRpc(value);

    [ClientRpc]
    private void SetShieldClientRpc(int value)
    {
        // set the value
        var old = ShieldSync;
        ShieldSync = value;
        OnValueChanged.Invoke(old, value);
    }

}

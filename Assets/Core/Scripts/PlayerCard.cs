using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Networking;
using Unity.Collections;
using TMPro;

public class PlayerCard : NetworkBehaviour
{
    [SerializeField] private TMP_InputField player_name_field;
    [SerializeField] private TMP_Text team_text;
    [SerializeField] private TMP_Text team_button_text;
    [SerializeField] private TMP_Text ready_text;
    [SerializeField] private Button ready_button;
    [SerializeField] private Button team_button;

    public UnityEvent<ulong> onReadyPressed = new UnityEvent<ulong>();

    public NetworkVariable<bool> is_red_team { get; private set; } = new NetworkVariable<bool>(true);
    private NetworkVariable<FixedString32Bytes> string_name = new NetworkVariable<FixedString32Bytes>();

    public FixedString32Bytes getName() => string_name.Value;
    public bool is_ready { get; set; } = false;
    public NetworkVariable<ulong> id { get; private set; } = new NetworkVariable<ulong>();

    private PlayerCardHandler player_card_handler = default;

    public override void OnNetworkSpawn()
    {
        // Get the player card handler component in the scene
        player_card_handler = GameObject.FindGameObjectWithTag("CardManager").GetComponent<PlayerCardHandler>();

        // Network variables 
        is_red_team.OnValueChanged += teamValueChanged;
        string_name.OnValueChanged += NameChanged;

        // Input text field
        player_name_field.onEndEdit.AddListener(nameTextChanged);

        player_card_handler.UpdatePlayerDataServerRpc(OwnerClientId, getName(), is_red_team.Value);

        // Disable everything here
        player_name_field.enabled = IsOwner;
        ready_button.enabled = IsOwner;
        ready_button.image.color = !IsOwner ? new Color(1, 1, 1, 0) : Color.white;
        team_button.enabled = IsOwner;
        team_button.image.color = !IsOwner ? new Color(1, 1, 1, 0) : Color.white;
        team_button_text.text = IsOwner ? "Change Team" : "";
        player_name_field.readOnly = !IsOwner;
        player_name_field.image.color = !IsOwner ? new Color(1, 1, 1, 0) : Color.white;
        player_name_field.GetComponentsInChildren<TMP_Text>()[1].color = IsOwner ? Color.black : Color.white;

        player_card_handler.UpdateCardFieldsServerRpc();
    }

    [ClientRpc]
    public void UpdateNameFieldClientRpc(FixedString32Bytes fixedName)
    {
        player_name_field.text = fixedName.ConvertToString();
    }

    public string GetPlayerName()
    {
        return player_name_field.text;
    }

    public void SetPlayerNameFieldEnabled(bool enabled)
    {
        player_name_field.enabled = enabled;
    }

    public void SetReadyTextString(string val)
    {
        ready_text.text = val;
    }

    public void SetReadyTextColor(Color col)
    {
        ready_text.color = col;
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateIsReadyServerRpc(bool _new_val)
    {
        Debug.Log("Setting value of is_ready to: " + _new_val);

        is_ready = _new_val;
    }

    private void nameTextChanged(string _new)
    {
        nameTextChangedServerRpc(new FixedString32Bytes(_new));
    }

    [ServerRpc]
    private void nameTextChangedServerRpc(FixedString32Bytes _new)
    {
        string_name.Value = _new;
    }

    [ServerRpc(RequireOwnership = false)]
    public void setNameServerRpc(FixedString32Bytes _new)
    {
        string_name.Value = _new;
        player_name_field.text = _new.ToString();
    }

    private void teamValueChanged(bool _old, bool _new)
    {
        teamStatusChangedServerRpc();
    }
    private void NameChanged(FixedString32Bytes _old, FixedString32Bytes _new)
    {
        NameChangedServerRpc(_new);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NameChangedServerRpc(FixedString32Bytes _new)
    {
        NameChangedClientRpc(_new);
    }

    [ClientRpc]
    private void NameChangedClientRpc(FixedString32Bytes _new)
    {
        player_name_field.text = _new.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void setTeamStatusServerRpc(bool _new_val)
    {
        is_red_team.Value = _new_val;
    }

    public void updateTeamStatus()
    {
        updateTeamStatusServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void teamStatusChangedServerRpc()
    {
        updateTeamStatusClientRpc();
    }

    [ServerRpc]
    public void updateTeamStatusServerRpc()
    {
        if (!is_ready)
        {
            is_red_team.Value = !is_red_team.Value;
            //team_text.text = is_red_team.Value ? "Red Team" : "Blue Team";
        }
        updateTeamStatusClientRpc();
    }

    [ClientRpc]
    public void updateTeamStatusClientRpc()
    {
        if (!is_ready)
        {
            team_text.text = is_red_team.Value ? "Red Team" : "Blue Team";
        }
    }

    [ClientRpc]
    public void setDisabledClientRPC(bool is_owner)
    {
        //bool is_owner = gameObject.GetComponent<NetworkObject>().OwnerClientId == id.Value;
        player_name_field.enabled = is_owner;
        ready_button.enabled = is_owner;
        ready_button.image.color = !is_owner ? new Color(1, 1, 1, 0) : Color.white;
        team_button.enabled = is_owner;
        team_button.image.color = !is_owner ? new Color(1, 1, 1, 0) : Color.white;
        team_button_text.text = is_owner ? "Change Team" : "";
        player_name_field.readOnly = !is_owner;
        player_name_field.image.color = !is_owner ? new Color(1, 1, 1, 0) : Color.white;
    }

    public void updateID(ulong _id)
    {
        if (NetworkManager.Singleton == null) return;
        var network = NetworkManager.Singleton;
        updateIDServerRPC(_id, network.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateIDServerRPC(ulong _id, ulong _local_id)
    {
        id.Value = _id;

        bool is_owner = _id == _local_id;
        Debug.Log("Owner ID: " + _id + "  local ID: " + _local_id);
        setDisabledClientRPC(is_owner);
    }

    public void onReady()
    {
        player_card_handler.OnPlayerCardReadyUpPressedServerRpc(OwnerClientId);
    }
}

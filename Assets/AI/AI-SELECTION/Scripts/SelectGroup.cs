using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SelectGroup : MonoBehaviour
{
    //[SerializeField] private PlayerDataSO playerData;
    //[SerializeField] private GameTeamData gameTeamData;

    public GameObject select_group;
    public GameObject select_button;
    public int groupId = 0; 
    public int id = 0;
    public ulong playerId = 0;
    public Key digit;

    public int this_army_size = 15;
    public ArmyManager army_group_select;
    public UnitManager unit_group_select;

    public List<GameObject> units;

    private ColorBlock colors;
    public Color filledColor;

    public List<String> unitNames = new List<String>();

    public TextMeshProUGUI unitInfoText;
    public TextMeshProUGUI groupInfoText;

    public GameObject panel;

    public List<Sprite> icons;
    public List<Image> images;

    void Start()
    {
        this_army_size = 15;
    }
    
    public void onClickPassObject()
    {
        //army_group_select.group = select_group;
        //army_group_select.button = select_button;

        unit_group_select.activeGroups = unit_group_select.groups[id].units;

        unitInfoText.text = "";
        groupInfoText.text = "";
        groupInfoText.text = name;

        foreach(var image in images)
        {
            image.enabled = false;
        }


        StartCoroutine(IncreaseBar());

    }

    IEnumerator IncreaseBar()
    {

        foreach (var unit in units)
        {
            if (!unitNames.Contains(unit.name))
            {
                unitNames.Add(unit.name);
            }
        }

        Vector3 pos = new Vector3(119, 53 * unitNames.Count, 0);
        unitInfoText.rectTransform.anchoredPosition = pos;

        if (unitNames.Count != 0)
        {
            float t = 0;
            float max = 54 * unitNames.Count;

            while (t <= max)
            {
                panel.GetComponent<RectTransform>().offsetMax = new Vector2(panel.GetComponent<RectTransform>().offsetMax.x, t);

                yield return new WaitForSeconds(0.01f);

                t += Time.deltaTime * 1000;
            }
            //images[unitNames.Count - 1].enabled = true;
            panel.GetComponent<RectTransform>().offsetMax = new Vector2(panel.GetComponent<RectTransform>().offsetMax.x, max);
            for(int i = 0; i < unitNames.Count; i++)
            {
                images[i].enabled = true;
            }
            foreach (var unitInfo in unitNames)
            {
                int index = 0;
                foreach (var unit in units)
                {
                    if (unitInfo.Contains(unit.name))
                    {
                        index++;
                    }
                }
                unitInfoText.text += unitInfo + " X " + index.ToString() + "\n" + "\n";
            }
        }
        else
        {
            panel.GetComponent<RectTransform>().offsetMax = new Vector2(panel.GetComponent<RectTransform>().offsetMax.x, 0);
        }
    }

    private void Update()
    {
        if (playerId == 0)
        {
            foreach (var player in FindObjectsOfType<PlayerInformation>())
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                if (networkObject.OwnerClientId == networkObject.NetworkManager.LocalClient.ClientId)
                {
                    playerId = networkObject.NetworkObjectId;
                }
            }
        }

        if (GetComponent<Button>().colors.selectedColor == Color.white)
        {
            foreach (var player in FindObjectsOfType<MechCharacterController>())
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                if (networkObject.OwnerClientId == networkObject.NetworkManager.LocalClient.ClientId)
                {
                    //bool isRedTeam = playerData.GetPlayerTeam(networkObject.OwnerClientId);
                    var color = Color.white;
                    var buttonColors = GetComponent<Button>().colors;
                    buttonColors.selectedColor = color;
                    buttonColors.highlightedColor = color;
                    GetComponent<Button>().colors = buttonColors;
                    colors = buttonColors;
                }
            }
        }

        if (unit_group_select == null)
        {
            foreach (var player_ in FindObjectsOfType<NetworkObject>())
            {
                if (player_.NetworkObjectId == playerId)
                {
                    unit_group_select = player_.GetComponent<UnitManager>();
                }
            }
        }

        if (units.Count > 0)
        {
            var buttonColors = GetComponent<Button>().colors;
            buttonColors.normalColor = filledColor;
            GetComponent<Button>().colors = buttonColors;
        }
    }


}

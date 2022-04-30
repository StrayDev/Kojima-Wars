using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmyManager : MonoBehaviour
{
    public GameObject group;
    public GameObject button;
    public GameObject spawn_point;

    public GameObject ground_troop_1;
    public GameObject ground_troop_2;
    public GameObject ground_troop_3;
    public GameObject vehicle_troop_1;
    public GameObject vehicle_troop_2;
    public GameObject vehicle_troop_3;
    public GameObject air_troop_1;
    public GameObject air_troop_2;
    public GameObject air_troop_3;
    SelectGroup SG;

    public int current_group = 1;

    private void Start()
    {
        group = GameObject.Find("Group_1");
    }

    public enum UnitType
    {
        GROUNDCLASS_1,
        GROUNDCLASS_2,
        GROUNDCLASS_3,
        VICCLASS_1,
        VICCLASS_2,
        VICCLASS_3,
        AIRCLASS_1,
        AIRCLASS_2,
        AIRCLASS_3

    };
    // Update is called once per frame

    public void onClickGroundClass1()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 1)
            {
                GameObject child = GameObject.Instantiate(ground_troop_1,spawn_point.transform.position,spawn_point.transform.rotation);
                //child.transform.localPosition = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 1;
            }
        }
    }

    public SelectGroup getGroup()
    {
        return SG;
    }

    private void Update()
    {
        /*
        if(Mouse.current.middleButton.wasPressedThisFrame)
        {
            RaycastHit hit;
            Ray ray = GameObject.Find("WeaponCamera").GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());

            GameObject[] selectedAI = GameObject.Find("Group_" + current_group).GetComponent<ArmyGroupSelect>().SelectAI();

            if (Physics.Raycast(ray, out hit, 100000.0f))
            {
                foreach (var AI in selectedAI)
                {
                    AI.GetComponent<AIMovement>().move(hit.point);
                }
            }
        }
        */
    }

    public void spawnUnit(int type)
    {
        UnitType _type = (UnitType)type;
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 1)
            {
                GameObject prefab = null;
                switch (_type)
                {
                    case UnitType.GROUNDCLASS_1:
                        {
                            prefab = ground_troop_1;
                            break;
                        }
                    case UnitType.GROUNDCLASS_2:
                        {
                            prefab = ground_troop_2;
                            break;
                        }
                    case UnitType.GROUNDCLASS_3:
                        {
                            prefab = ground_troop_3;
                            break;
                        }
                    case UnitType.VICCLASS_1:
                        {
                            prefab = vehicle_troop_1;
                            break;
                        }
                    case UnitType.VICCLASS_2:
                        {
                            prefab = vehicle_troop_2;
                            break;
                        }
                    case UnitType.VICCLASS_3:
                        {
                            prefab = vehicle_troop_3;
                            break;
                        }
                    case UnitType.AIRCLASS_1:
                        {
                            prefab = air_troop_1;
                            break;
                        }
                    case UnitType.AIRCLASS_2:
                        {
                            prefab = air_troop_2;
                            break;
                        }
                    case UnitType.AIRCLASS_3:
                        {
                            prefab = air_troop_3;
                            break;
                        }
                }
                GameObject child = GameObject.Instantiate(prefab, spawn_point.transform.position, spawn_point.transform.rotation);
                child.GetComponent<BaseUnitClass>().team = BaseUnitClass.TeamType.RED;
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 1;

            }
        }
    }
    public void onClickGroundClass2()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 1)
            {
                GameObject child = GameObject.Instantiate(ground_troop_2);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 1;
            }
        }
    }

    public void onClickGroundClass3()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 1)
            {
                GameObject child = GameObject.Instantiate(ground_troop_3);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 1;
            }
        }
    }

    public void onClickVehicleClass1()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 2)
            {
                GameObject child = GameObject.Instantiate(vehicle_troop_1);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 2;
            }
        }
    }

    public void onClickVehicleClass2()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 2)
            {
                GameObject child = GameObject.Instantiate(vehicle_troop_2);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 2;
            }
        }
    }

    public void onClickVehicleClass3()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 2)
            {
                GameObject child = GameObject.Instantiate(vehicle_troop_3);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 2;
            }
        }
    }

    public void onClickAirClass1()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 3)
            {
                GameObject child = GameObject.Instantiate(air_troop_1);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 3;
            }
        }
    }

    public void onClickAirClass2()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 3)
            {
                GameObject child = GameObject.Instantiate(air_troop_2);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 3;
            }
        }
    }

    public void onClickAirClass3()
    {
        if (button != null && group != null)
        {
            SG = button.GetComponent<SelectGroup>();
            if (SG.this_army_size >= 3)
            {
                GameObject child = GameObject.Instantiate(air_troop_3);
                child.transform.position = spawn_point.transform.position;
                child.gameObject.tag = "AI_RED";
                child.transform.parent = group.gameObject.transform;
                SG.this_army_size -= 3;
            }
        }
    }
}

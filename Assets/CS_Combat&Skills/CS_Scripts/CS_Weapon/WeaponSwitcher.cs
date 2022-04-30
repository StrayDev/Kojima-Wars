using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class WeaponSwitcher : MonoBehaviour
{
    public List<WeaponStats> weaponList;
    public WeaponScript weaponScript;

    public int currentWeapon = 0;

    //Input System
    public CS_PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<CS_InputManager>().playerInput;
        playerInput.PlayerGroundMovement.ScrollWheel.performed += ScrollWheel;

        StartCoroutine(assignModel());
    }

    private void OnEnable()
    {
        //scrollWheel = playerInput.PlayerGroundMovement.ScrollWheel;
        //scrollWheel.Enable();
    }

    private void OnDisable()
    {
        //scrollWheel.Disable();
    }

    IEnumerator assignModel()
    {
        currentWeapon = 0;
        UpdateGun();
        yield break;
    }

    private void Update()
    {
        /*if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon < weaponList.Count - 1)
        {
            nextWeapon();

        }
        if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon >= 0)
        {
            previousWeapon();
        }*/

        
    }

    public void ScrollWheel (InputAction.CallbackContext context)
    {
        
        if (context.ReadValue<float>() > 0 && currentWeapon < weaponList.Count - 1)
        {
            nextWeapon();
        }
        if (context.ReadValue<float>() < 0 && currentWeapon >= 0)
        {
            previousWeapon();
        }
    }

    private void nextWeapon()
    {
        currentWeapon = (currentWeapon + 1) % weaponList.Count;
        UpdateGun();
    }
    private void previousWeapon()
    {
        currentWeapon = (currentWeapon + 1 + weaponList.Count) % weaponList.Count;
        UpdateGun();
    }
    void UpdateGun()
    {
        weaponScript.stats = weaponList[currentWeapon];
        weaponScript.bulletsLeft = weaponScript.stats.magazineSize;
        weaponScript.bulletsShot = 0;
        //weaponScript.AssignModel();
    }
}

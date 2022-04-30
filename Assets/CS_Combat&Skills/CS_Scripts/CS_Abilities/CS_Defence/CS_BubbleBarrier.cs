using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
public class CS_BubbleBarrier : NetworkBehaviour
{
    public InputActionManager inputActions;

   [SerializeField] private int maxDistance;
   [SerializeField] private int activeTime;
   [SerializeField] private int barrierTime;
    private bool eqipped;
    private bool active;
    
   private GameObject player;
   private CS_PlayerController playerController;
   
   [SerializeField] private GameObject bubbleShield;
   [SerializeField] private GameObject ActiveAbility;
   private GameObject activeObject;

   private Transform spawnpoint;
   private Transform activeIndicator;
   
   private Camera FPSCamera;
   private void Awake()
   {
       player = ServerAbilityManager.Instance.GetOwner(gameObject).gameObject;
       playerController = player.GetComponent<CS_PlayerController>();
       inputActions = player.GetComponent<InputActionManager>();
       
       spawnpoint = player.GetComponent<CS_Firepoints>().barrierSpawnPoint;
       activeIndicator = player.GetComponent<CS_Firepoints>().activeAbiliityPoint;

       StartCoroutine(ActiveTime());
       eqipped = true;
       
       SpawnObject();
   }

   private void SpawnObject()
   {
       activeObject = Instantiate(ActiveAbility, activeIndicator.position, Quaternion.identity);
       activeObject.transform.SetParent(player.transform);
       
   }

   
    private void FixedUpdate()
    {
        if (!IsServer) return;
        Ray ray = FPSCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        //Need to be switched to Player
        if (Physics.Raycast(ray, out hit, maxDistance) /*&& allied player */)
        {
           //if (inputActions.inputActions.Mech.LeftClick.IsPressed())
           // {
           //     if (eqipped)
           //     {
           //         if (activeObject != null)
           //         {
           //             Destroy(activeObject);
           //         }
           //         ApplyShield();
           //     }
           // }
        }
    }

    private void ApplyShield()
    {
        GameObject playerBarrier = Instantiate(bubbleShield, spawnpoint.position, Quaternion.identity);
        playerBarrier.transform.SetParent(player.transform);
        eqipped = false;
        active = true; 
        StartCoroutine(BarrierTime());
    }

    private IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(activeTime);

    }
    
    private IEnumerator BarrierTime()
    {
        yield return new WaitForSeconds(barrierTime);
        active = false;

    }
    
    void Update()
    {
        
        
        if (active)
        {
            //Work out incoming damage
            //Add multiplier depending on damage amount 
            
            //If (damage = 300){
            //BreakShield();
            //}
            //*/
        }
    }
    
    private void GetFPSCamRecursively(Transform obj)
    {
        //Get HUD recursively in children
        foreach (Transform child in obj)
        {
            if (child.TryGetComponent(out Camera camera))
            {
                FPSCamera = camera;
            }
            else
            {
                GetFPSCamRecursively(child);
            }
        }
    }
}

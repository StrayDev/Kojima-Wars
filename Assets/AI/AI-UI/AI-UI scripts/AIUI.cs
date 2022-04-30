using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using System;
using TMPro;

public class AIUI : MonoBehaviour
{
    public static AIUI Instance;

    public Canvas mapCanvas;
    public Canvas UI;
    //public Canvas unitTab;
    //public GameObject unitManagerTabCheck;
    public GameObject unitManagerTab;

    public Camera mainCamera;
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera interactiveMapCam;

    public List<GameObject> uiElements;


    public List<GameObject> selectedAI;

    public GameObject group1;
    public GameObject group2;
    public GameObject group3;


    public GameObject player;
    public GameObject unitMover;

    private bool runOnce = false;

    private int layerMask = 1 << 9;

    private bool mapIsOpen = false;
    private bool mapDoubleZoomed = false;
    private bool mapZoomed = false;

    int zoom = 70;
    int zoom2 = 100;
    int normal = 50;
    float smooth = 5;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        mapCanvas.enabled = false;
        //unitTab.enabled = false;
        UI.enabled = true;
        //interactiveMapCam.enabled = false;
        //playerCam.enabled = true;
        playerCam = VCameraSetup.playerCam?.GetVirtualCamera();
        interactiveMapCam = VCameraSetup.mapCam?.GetVirtualCamera();

        foreach (Transform child in unitManagerTab.transform)
        {
            child.GetComponent<Image>().enabled = false;
            child.GetComponent<Button>().enabled = false;
            foreach (Transform child_ in child.transform)
            {
                child_.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
        foreach (var elements in uiElements)
        {
            elements.SetActive(false);
        }
        //image.enabled = false;
    }

    private void OnMapButton(InputAction.CallbackContext context)
    {
        if(!mapIsOpen && !Keyboard.current.escapeKey.isPressed)
        {
             ExpandMap();
            return;
        }
        CloseMap();
    }

    public void ExpandMap()
    {
        // only works in mech mode
        if (InputManager.GetCurrentControlType() != ControlType.MECH) return;

        //InputManager.SetInputType(ControlType.MAPUI);
        mapCanvas.enabled = true;     
        playerCam.enabled = false;    
        interactiveMapCam.enabled = true;   
        UI.enabled = false;
        foreach (Transform child in unitManagerTab.transform)
        {
            child.GetComponent<Image>().enabled = true;
            child.GetComponent<Button>().enabled = true;
            child.GetComponent<Button>().onClick.AddListener(child.GetComponent<SelectGroup>().onClickPassObject);
            foreach (Transform child_ in child.transform)
            {
                child_.GetComponent<TextMeshProUGUI>().enabled = true;
            }
        }

        foreach (var elements in uiElements)
        {
            elements.SetActive(true);
        }
        //unitManagerTabCheck.SetActive(true);

        ToggleInput();
    }

    public void CloseMap()
    {
        CursorManager.DisableCursor("ai-map");

        InputManager.SetInputType(ControlType.MECH);
        //inputActionManager.inputActions.Mech.Enable();
        mapCanvas.enabled = false;
        UI.enabled = true;
        playerCam.enabled = true;
        interactiveMapCam.enabled = false;
        //unitTab.enabled = false;
        //unitManagerTab.SetActive(false);
        //unitManagerTabCheck.SetActive(false);

        foreach (Transform child in unitManagerTab.transform)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Image>().enabled = false;
            child.GetComponent<Button>().enabled = false;
            foreach (Transform child_ in child.transform)
            {
                child_.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
        
        foreach (var elements in uiElements)
        {
            elements.SetActive(false);
        }

        ToggleInput();
    }

    private void ToggleInput()
    {
        if (!mapIsOpen)
        {
            InputManager.SetInputType(ControlType.NONE);
            mapIsOpen = true;
            return;
        }
        InputManager.SetInputType(ControlType.MECH);
        mapIsOpen = false;
    }

    private void Update()
    {
        if (playerCam == null || interactiveMapCam == null || unitMover == null || mainCamera == null || player == null)
        {
            unitMover = GameObject.Find("UnitHandler");
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            playerCam = VCameraSetup.playerCam?.GetVirtualCamera();
            interactiveMapCam = VCameraSetup.mapCam?.GetVirtualCamera();
            player = VCameraSetup.player;
        }
        if (playerCam == null || interactiveMapCam == null || unitMover == null || mainCamera == null || player == null)
        {
/*            Debug.Assert(player != null);
            Debug.Assert(unitMover != null);
            Debug.Assert(playerCam != null);
            Debug.Assert(interactiveMapCam != null);
            Debug.Assert(mainCamera != null);*/
            return;
        }

        if (interactiveMapCam.enabled &&
        Mouse.current.leftButton.IsPressed() &&
        !EventSystem.current.IsPointerOverGameObject() && 
        player.GetComponent<UnitManager>().activeGroups != null)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, 100000.0f, layerMask))
            {
                foreach (var AI in player.GetComponent<UnitManager>().activeGroups)
                {
                    unitMover.GetComponent<AIUnitMover>().SyncTransformServerRpc(hit.point, AI.GetComponent<NetworkObject>().NetworkObjectId);
                }
            }
        }

        if (interactiveMapCam.enabled)
        {
            if (!mapZoomed && !mapDoubleZoomed)
            {
                interactiveMapCam.m_Lens.FieldOfView = Mathf.Lerp(interactiveMapCam.m_Lens.FieldOfView, normal, smooth * Time.deltaTime);
            }

            if (mapZoomed && !mapDoubleZoomed)
            {
                interactiveMapCam.m_Lens.FieldOfView = Mathf.Lerp(interactiveMapCam.m_Lens.FieldOfView, zoom, smooth * Time.deltaTime);
            }

            if (!mapZoomed && mapDoubleZoomed)
            {
                interactiveMapCam.m_Lens.FieldOfView = Mathf.Lerp(interactiveMapCam.m_Lens.FieldOfView, zoom2, smooth * Time.deltaTime);
            }
        }
    }

    public void SelectAIGroup(int id)
    {
        //selectedAI = GameObject.Find("Group_" + id).GetComponent<ArmyGroupSelect>().SelectAI();
    }

    public void OpenUnitTab()
    {
        /*
        if(unitTab.enabled)
        {
            unitTab.enabled = false;
        }
        else
        {
            unitTab.enabled = true;
        }
        */
        //mapCanvas.enabled = false;
    }

    public void CloseUnitTab()
    {
        //unitTab.enabled = false;
        //mapCanvas.enabled = false;
    }


    public bool IsMapOpen()
    {
        return mapIsOpen;
    }

    public void ZoomIn()
    {
        mapZoomed = true;
        mapDoubleZoomed = false;
    }

    public void ZoomOut()
    {
        mapZoomed = false;
        mapDoubleZoomed = false;
    }

    public void DoubleZoom()
    {
        mapZoomed = false;
        mapDoubleZoomed = true;
    }
}

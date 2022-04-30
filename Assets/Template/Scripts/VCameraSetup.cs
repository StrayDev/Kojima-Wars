using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public enum Mode
{
    Mech,
    Vtol,
    Map
}

public class VCameraSetup : NetworkBehaviour
{
    [SerializeField] private AppDataSO appData = default;
    [SerializeField] private CinemachineVirtualCamera vCamera = default;
    [SerializeField] private Mode mode = default;

    public static VCameraSetup playerCam;
    public static VCameraSetup mapCam;
    public static VCameraSetup vtolCam;
    public static GameObject player;

    public override void OnNetworkSpawn()
    {
        if (mode == Mode.Mech)
        {
            RegisterMechVirtualCamera();
            return;
        }
        
        if (mode == Mode.Vtol)
        {
            RegisterVtolVirtualCamera();
            return;
        }

        RegisterMapVirtualCamera();
    }

    public void SetFov()
    {
        if (appData != null)
        {
            vCamera.m_Lens.FieldOfView = appData.fieldOfView;
        }
    }
    
    private void RegisterMechVirtualCamera()
    {
        foreach (var controller in MechCharacterController.List)
        {
            if (controller.OwnerClientId != NetworkManager.LocalClient.ClientId) continue;
            controller.GetComponentInChildren<VCameraTarget>().SetTargetReference(vCamera);

            playerCam = this;
            player = controller.gameObject;
            player.GetComponentInChildren<WeaponScript>().fpsCam = vCamera;

            SetFov();

            foreach (var buttons in GameObject.FindObjectsOfType<SelectGroup>())
            {
                buttons.unit_group_select = controller.GetComponent<UnitManager>();
            }

        }
        //GetComponent<Recoil>().AssignWeaponScript();
        
    }

    private void RegisterVtolVirtualCamera()
    {
        foreach (var controller in VTOLCharacterController.List)
        {
            if (controller.OwnerClientId != NetworkManager.LocalClient.ClientId) continue;
            controller.GetComponentInChildren<VCameraTarget>().SetTargetReference(vCamera);
            vtolCam = this;

            if (appData != null)
            {
                
                //Debug.Log(appData.fieldOfView);
                vCamera.m_Lens.FieldOfView = appData.fieldOfView;
            }
        }
    }

    private void RegisterMapVirtualCamera()
    {
        foreach(var controller in MapController.List)
        {
            if (controller.OwnerClientId != NetworkManager.LocalClient.ClientId) continue;

            mapCam = this;

            controller.GetComponentInChildren<VCameraTarget>().SetLookAtReference(vCamera);
        }
    }

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        return vCamera;
    }
}
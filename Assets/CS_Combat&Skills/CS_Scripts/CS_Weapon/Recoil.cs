using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VectorSwizzles;

public class Recoil : NetworkBehaviour
{
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //References
    public GameObject weaponHolder;
    public WeaponScript weaponScript;
    public GameObject player;

    private bool weaponScriptAssigned;
    private float recoilTimer = 0.0F;

    public void AssignWeaponScript()
    {
        foreach (var controller in MechCharacterController.List)
        {
            if (controller.OwnerClientId != NetworkManager.LocalClient.ClientId) continue;

            player = controller.gameObject;
        }

        weaponScript = player.GetComponentInChildren<WeaponScript>();
        player.GetComponentInChildren<WeaponScript>().AssignFPSCam(GetComponent<CinemachineVirtualCamera>());
        weaponScriptAssigned = true;
    }

    void Update()
    {
        float recoveryRate = weaponScript.stats.recoilRecoveryCurve.Evaluate(recoilTimer) * Time.deltaTime;
        targetRotation = Vector3.MoveTowards(targetRotation, Vector3.zero, recoveryRate);
        currentRotation = Vector3.MoveTowards(currentRotation, targetRotation, weaponScript.stats.recoilSnappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        recoilTimer += Time.deltaTime;
    }

    public void RecoilFire()
    {
        Vector3 recoil = Vector3.Lerp(weaponScript.stats.hipfireRecoil, weaponScript.stats.ADSRecoil, weaponScript.ADSAmount);
        recoil.x = Random.Range(-recoil.x, recoil.x);
        recoil.y = -recoil.y;
        recoil.z = Random.Range(-recoil.z, recoil.z);
        targetRotation += recoil.yxz();
        recoilTimer = 0.0F;
    }
}

using UnityEngine;
using Cinemachine;

public class VCameraTarget : MonoBehaviour
{
    private GameObject _target = default;

    public void SetTargetReference(CinemachineVirtualCamera virtualCamera)
    {
        // sets the camera to follow the target
        virtualCamera.Follow = transform;

        // keeps a reference to sync enable and disable
        _target = virtualCamera.gameObject;

        // safety
        if (_target != null) return;
        Debug.LogWarning($"Unable to set VCameraTarget for : { transform.parent.name }");
        Destroy(this);
    }

    public void SetLookAtReference(CinemachineVirtualCamera virtualCamera)
    {
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
        virtualCamera.transform.position = transform.position + new Vector3(0, 600, 0);

        // safety
        if (_target != null) return;
        Debug.LogWarning($"Unable to set VCameraTarget for : { transform.parent.name }");
        Destroy(this);
    }

    private void OnEnable()
    {
        if (_target != null)
        {
            _target?.SetActive(true);
        }
    }
    private void OnDisable()
    {
        if(_target != null)
        {
            _target?.SetActive(false);
        }
    }
}

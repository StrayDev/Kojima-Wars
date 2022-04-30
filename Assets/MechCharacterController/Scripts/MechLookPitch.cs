using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class MechLookPitch : NetworkBehaviour
{
    [SerializeField] private AppDataSO appData = default;

    private Vector3 _inputVector;

    private float _maxAngle = 85f;
    private float _pitch;

    private Transform _transform = default;

    private void Awake()
    {
        _transform = transform;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        InputManager.MECH.Look.performed += UpdatePitchAngle;
        InputManager.MECH.Look.canceled  += UpdatePitchAngle;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        InputManager.MECH.Look.performed -= UpdatePitchAngle;
        InputManager.MECH.Look.canceled  -= UpdatePitchAngle;
    }

    private void UpdatePitchAngle(InputAction.CallbackContext context)
    {
        _inputVector.y = context.ReadValue<Vector2>().y;
    }

    private void Update()
    {
        _pitch += _inputVector.y * appData.mousePitchSensitivity;
        _pitch = Mathf.Clamp(_pitch, -_maxAngle, _maxAngle);
        _transform.localRotation = Quaternion.Euler(-_pitch, 0, 0);
    }
}


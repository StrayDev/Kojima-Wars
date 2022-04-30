using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class MechLookYaw : NetworkBehaviour
{
    [SerializeField] private AppDataSO appData = default;

    private Vector3 _inputVector;
    private float _yaw;
    private Rigidbody _rb = default;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.Log($"{this.name}, does not contain a rigidbody");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        InputManager.MECH.Look.performed += UpdateYawAngle;
        InputManager.MECH.Look.canceled  += UpdateYawAngle;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        InputManager.MECH.Look.performed -= UpdateYawAngle;
        InputManager.MECH.Look.canceled  -= UpdateYawAngle;
    }

    private void UpdateYawAngle(InputAction.CallbackContext context)
    {
        _inputVector.x = context.ReadValue<Vector2>().x;
    }

    private void Update()
    {
        _yaw += _inputVector.x * appData.mouseYawSensitivity;
        _rb.rotation = Quaternion.Euler(0, _yaw, 0);
    }
}


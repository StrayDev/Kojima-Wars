using Unity.Netcode;
using UnityEngine;

public class CS_PlayerStats : NetworkBehaviour
{
    [Header("Controls")]
    public float unscopedSensitivity = 20f;
    public float scopedSensitivity = 10f;
    public float currentSensitivity;


    [Header("Logic")]
    public float maxHealth = 100f;
    public float maxShields = 200f;
    public bool flashed = false;
    public bool hacked = false;
    public bool boostActive = false;
    public int damageBoost = 1;

    [Header("Video")]
    public int FOV = 60;

    [Header("Movement")]
    public float speed = 5f;
    public float acceleration = 1f;
    public float deceleration = 1f;
    public float airAccelerationMultiplier = 0.5f;
    public float maxRampAngle = 45f;
    public float jumpForce = 250f;
    public float gravity = -98.1f;
    public float maxStepSize = 1f;
    [Range(0,0.05F)] public float stepSmooth = 0f;
    
    /*
        private float priorGodHealth;
        private float priorGodShield;

        public void SetGodMode(bool god_mode)
        {
            SetGodModeServerRpc(god_mode);
        }

        [ServerRpc(RequireOwnership =false)]
        private void SetGodModeServerRpc(bool god_mode)
        {
            SetGodModeClientRpc(god_mode);

        }

        [ClientRpc]
        private void SetGodModeClientRpc(bool god_mode)
        {
            if (god_mode)
            {
                priorGodHealth = health.Value;
                priorGodShield = shields.Value;
                health.Value = 999999f;
                shields.Value = 999999f;
            }
            else
            {
                health.Value = priorGodHealth;
                shields.Value = priorGodShield;
            }
        }*/

}

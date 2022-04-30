using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class NetworkTransformComponent : NetworkBehaviour
    {
        [SerializeField] public Mode mode = default;

        public NetworkTransformComponent mechSwitchScript = default;
        private NetworkTransformComponent vtolSwitchScript = default;

        public void Start()
        {
            // set up component refrences
            SetUpModeReferences();

            // set vtol inactive
            if (mode == Mode.Vtol) gameObject.SetActive(false);

            // Set up for the owner ownly beyond this point
            if (!IsOwner) return;

            // add callbacks 
            if (mode == Mode.Mech)
            {
                //InputManager.SetInputType(ControlType.MECH);
                InputManager.MECH.Transform.canceled += SwitchMechMode;
            }
            else
            {
                InputManager.VTOL.Transform.canceled += SwitchMechMode;
            }
        }

        public void SwitchMechMode(InputAction.CallbackContext context)
        {
            // call server RPC
            if(!GameObject.Find("PauseManager").GetComponent<PauseManager>().paused)
            {
                SetInputType();
                SwitchModeServerRPC(OwnerClientId);
            }
        }

        public void SwitchOnCollision( )
        {
            // call server RPC
            if (!GameObject.Find("PauseManager").GetComponent<PauseManager>().paused)
            {
                SetInputType();
                SwitchModeServerRPC(OwnerClientId);
            }
        }

        public void ForceSwitchMechMode()
        {
            // call server RPC
            InputManager.SetInputType(ControlType.MECH);
            SetInputType();
            SwitchModeServerRPC(OwnerClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SwitchModeServerRPC(ulong id)
        {
            SwitchModeClientRPC(id);
        }

        [ClientRpc]
        public void SwitchModeClientRPC(ulong id)
        {
            // if you need to adjust the way the player transforms
            // do so in this function
            AlignObjectTransforms();
            SetActiveGameObjects();
        }

        private void SetInputType()
        {
            var ct = InputManager.GetCurrentControlType() == ControlType.MECH ? ControlType.VTOL : ControlType.MECH;
            InputManager.SetInputType(ct);
        }

        private void AlignObjectTransforms()
        {
            if (mode == Mode.Mech)
            {
                vtolSwitchScript.transform.position = mechSwitchScript.transform.position;
                vtolSwitchScript.transform.rotation = mechSwitchScript.transform.rotation;
            }
            else
            {
                mechSwitchScript.transform.position = vtolSwitchScript.transform.position;
                mechSwitchScript.transform.rotation = Quaternion.Euler(0, vtolSwitchScript.transform.rotation.y,0);
            }
        }

        private void SetActiveGameObjects()
        {
            if (mode == Mode.Mech)
            {
                mechSwitchScript.gameObject.SetActive(false);
                vtolSwitchScript.gameObject.SetActive(true);
            }
            else
            {
                vtolSwitchScript.gameObject.SetActive(false);
                mechSwitchScript.gameObject.SetActive(true);
            }
        }



        private void SetUpModeReferences()
        {
            // TODO : add below to state setup func
            if (mode == Mode.Mech)
            {
                mechSwitchScript = this;
                foreach (var obj in VTOLCharacterController.List)
                {
                    if (OwnerClientId != obj.OwnerClientId) continue;
                    vtolSwitchScript = obj.gameObject.GetComponent<NetworkTransformComponent>();
                }
                return;
            }

            vtolSwitchScript = this;
            foreach (var obj in MechCharacterController.List)
            {
                if (OwnerClientId != obj.OwnerClientId) continue;
                mechSwitchScript = obj.gameObject.GetComponent<NetworkTransformComponent>();
            }
        }
    }
}

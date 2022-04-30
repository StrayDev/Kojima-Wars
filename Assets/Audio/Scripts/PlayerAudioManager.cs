using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    private string currentMat;
    private bool inWater = false;

    private void Start()
    {
        //Just for playing ambient sounds etc
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.CreateFMODInstancePoolClientRpc(AudioManager.Instance.events.environmentAudioEvents.windWhistle, 1);
            AudioManager.Instance.StartFMODLoop(AudioManager.Instance.events.environmentAudioEvents.windWhistle);

            AudioManager.Instance.CreateFMODInstancePoolServerRpc(AudioManager.Instance.events.playerAudioEvents.footstep, 4);
            AudioManager.Instance.CreateFMODInstancePoolServerRpc(AudioManager.Instance.events.playerAudioEvents.jump, 4);
        }
    }

    public void PlayJump()
    {
        if (inWater)
        {
            AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 2.0f);
        }
        else
        {
            switch (currentMat)
            {
                case "Grass":
                    AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 0.0f);
                    break;
                case "Road":
                    AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 1.0f);
                    break;
                default:
                    break;
            }
        }

        AudioManager.Instance.PlayFMODOneShotServerRpc(AudioManager.Instance.events.playerAudioEvents.jump, transform.position);
    }

    private void PlayFootstep()
    {
        if(inWater)
        {
            AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 2.0f);
        }
        else
        {
            switch (currentMat)
            {
                case "Grass":
                    AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 0.0f);
                    break;
                case "Road":
                    AudioManager.Instance.SetAudioEventParameters(AudioManager.Instance.events.playerAudioEvents.footstep, "Terrain", 1.0f);
                    break;
                default:
                    break;
            }
        }

        AudioManager.Instance.PlayFMODOneShotServerRpc(AudioManager.Instance.events.playerAudioEvents.footstep, transform.position);
    }

    private void Update()
    {
        //Update Audio Positions
        AudioManager.Instance.UpdatePosition(AudioManager.Instance.events.playerAudioEvents.footstep, transform.position);
        AudioManager.Instance.UpdatePosition(AudioManager.Instance.events.playerAudioEvents.jump, transform.position);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, groundLayerMask))
        {
            MeshCollider collider = hit.collider as MeshCollider;

            if(collider != null)
            {
                Mesh mesh = collider.sharedMesh;
                int index = GetSubMeshIndex(mesh, hit.triangleIndex);

                currentMat = collider.GetComponent<MeshRenderer>().sharedMaterials[index].name;
            }
        }
    }

    private int GetSubMeshIndex(Mesh mesh, int triangleIndex)
    {
        int triangleCounter = 0;

        for(int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
        {
            int indexCount = mesh.GetSubMesh(subMeshIndex).indexCount;
            triangleCounter += indexCount / 3;

            if (triangleIndex < triangleCounter)
                return subMeshIndex;
        }

        return 0;
    }
}

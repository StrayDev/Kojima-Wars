using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CS_Flashbang : CS_NewGrenade
{
    private Image flashImage;
    private Color TempColor;
    private bool isFlashed = false;
    private float decay;
    private CS_PlayerStats PlayerStats;
    public float maxRange;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        Fuse();

        if (exploded)
        {
            SpawnParticleEffectClientRpc(transform.position, Quaternion.identity);

            Collider[] EnemiesToBlind = Physics.OverlapSphere(transform.position, maxRange);

            foreach(var obj in EnemiesToBlind)
            {
                Entity entityHit = obj.GetComponent<Entity>();
                if (entityHit != null && ServerAbilityManager.Instance.IsEnemy(gameObject, entityHit))
                {
                    ServerAbilityManager.Instance.PlayerStatusServerRPC(obj.gameObject, "Flash", 10);
                }
            }

            //flashObject.GetComponent<CS_FlashSetTrue>().player = player;
            Destroy(gameObject);
        }

        
        /*if (TempColor.a > 0.7 && isFlashed)
        {
            TempColor.a -= 0.1f * Time.deltaTime;
            flashImage.color = TempColor;
        }
        else if (TempColor.a > 0 && TempColor.a <= 0.7 && isFlashed)
        {
            TempColor.a -= 0.8f * Time.deltaTime;
            flashImage.color = TempColor;
        }
        else if (isFlashed)
        {
            playerStats.flashed = false;
            isFlashed = false;
        }

        if (playerStats.flashed && !isFlashed)
        {
            TempColor.a = 1f;
            isFlashed = true;
        }*/
    }

    [ClientRpc]
    private void SpawnParticleEffectClientRpc(Vector3 position, Quaternion rotation)
    {
        Instantiate(explosion, position, rotation);
    }
}

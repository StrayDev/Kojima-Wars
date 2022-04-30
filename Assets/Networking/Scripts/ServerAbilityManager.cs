using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerAbilityManager : NetworkBehaviour
{
    public static ServerAbilityManager Instance;

    private Dictionary<GameObject, Entity> spawnedAbilities;

    [System.Serializable]
    public class AbilityMaterial
    {
        public string materialName;
        public Material material;
    }

    [SerializeField] private List<AbilityMaterial> abilityMaterials;

    public Shader revealShader;
    public Shader defaultShader;
    public Material redTex;
    public Material blueTex;

    public float flashamount = 0.1f;
    public GameObject flashCanvas;
    private GameObject flashObj;
    private float flashDuration;

    public GameObject muzzleFlash;
    public GameObject decal;
    public GameObject sparks;
    public GameObject ripple;

    private bool hitDetected;
    CrossHair cross;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (IsServer)
        {
            spawnedAbilities = new Dictionary<GameObject, Entity>();
        }
        
    }

    private void Update()
    {
        if (IsServer)
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (GameObject spawnedObject in spawnedAbilities.Keys)
            {
                if (spawnedObject == null)
                {
                    toRemove.Add(spawnedObject);
                    continue;
                }

                UpdateTransformClientRpc(new NetworkObjectReference(spawnedObject), spawnedObject.transform.position,
                    spawnedObject.transform.rotation);
            }

            foreach (GameObject removeObject in toRemove)
            {
                spawnedAbilities.Remove(removeObject);
            }
        }
        //Flash logic
        if (flashObj != null)
        {
            Color temp = flashObj.GetComponentInChildren<Image>().color;

            if (temp.a > 0.7f)
            {
                float fadeAmount = temp.a - (0.1f * Time.deltaTime);
                temp = new Color(temp.r, temp.g, temp.b, fadeAmount);
                flashObj.GetComponentInChildren<Image>().color = temp;
            }
            else if (temp.a > 0 && temp.a <= 0.7)
            {
                float fadeAmount = temp.a - (0.8f * Time.deltaTime);
                temp = new Color(temp.r, temp.g, temp.b, fadeAmount);
                flashObj.GetComponentInChildren<Image>().color = temp;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterNewAbilityServerRpc(NetworkObjectReference spawnedAbility, NetworkObjectReference spawnedBy)
    {
        NetworkObject abilityObject;
        NetworkObject entityObject;
        spawnedAbility.TryGet(out abilityObject);
        spawnedBy.TryGet(out entityObject);
        if (abilityObject != null && entityObject != null)
        {
            GameObject spawnedAbilityObject = abilityObject.gameObject;
            Entity spawnedByEntity = entityObject.GetComponent<Entity>();
            spawnedAbilities.Add(spawnedAbilityObject, spawnedByEntity);
        }
    }

    public Entity GetOwner(GameObject ability)
    {
        return spawnedAbilities[ability];
    }

    public bool IsOther(GameObject ability, Entity otherUnit)
    {
        return spawnedAbilities[ability] != otherUnit;
    }

    public bool IsEnemy(GameObject ability, Entity otherUnit)
    {
        if (spawnedAbilities[ability].TeamName != otherUnit.TeamName)
        {
            //Debug.Log("Enemy Found: " + otherUnit.TeamName + " is team: " + otherUnit.TeamName);
            return true;
        }

        return false;
    }

    public bool IsEnemy(GameObject playerOne, GameObject playerTwo)
    {
        if (playerOne.GetComponent<Entity>().TeamName != playerTwo.GetComponent<Entity>().TeamName)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerStatusServerRPC(NetworkObjectReference player, string effect, float duration)
    {
        PlayerStatusClientRPC(player, effect, duration);
    }

    [ClientRpc]
    private void PlayerStatusClientRPC(NetworkObjectReference player, string effect, float duration)
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            if (networkObject.GetComponent<Entity>().clientId != NetworkManager.Singleton.LocalClientId) return;
            GameObject _player = networkObject.gameObject;
            StartCoroutine(EffectOverTime(effect, _player, duration));
        }
    }

    IEnumerator EffectOverTime(string effect, GameObject player, float duration)
    {
        if (effect == "Slow")
        {
            player.GetComponent<CS_PlayerStats>().speed /= 2;
        }
        else if (effect == "Flash")
        {
            flashObj = Instantiate(flashCanvas, player.transform);
            /*Color temp = flashObj.GetComponentInChildren<Image>().color;
            temp.a = 1;
            flashObj.GetComponent<Image>().color = temp;*/
        }
        else if (effect == "DamageBoost")
        {
            player.GetComponent<CS_PlayerStats>().damageBoost = 2;
            Debug.Log(player.GetComponent<CS_PlayerStats>().damageBoost);
        }
        else if (effect == "DamageDecrease")
        {
            player.GetComponent<CS_PlayerStats>().damageBoost = 1;
            Debug.Log(player.GetComponent<CS_PlayerStats>().damageBoost);
        }

        yield return new WaitForSeconds(duration);

        if (effect == "Slow")
        {
            player.GetComponent<CS_PlayerStats>().speed *= 2;
        }
        else if (effect == "Flash")
        {
            Destroy(flashObj);
        }
        else if (effect == "DamageBoost")
        {
            player.GetComponent<CS_PlayerStats>().damageBoost = 1;
            Debug.Log(player.GetComponent<CS_PlayerStats>().damageBoost);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDashServerRPC(NetworkObjectReference player, float dashForce, float duration)
    {
        ApplyDashClientRPC(player, dashForce, duration);
    }
    [ClientRpc]
    private void ApplyDashClientRPC(NetworkObjectReference player, float dashForce, float duration)
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            StartCoroutine(Dash(duration, dashForce, networkObject.gameObject));
        }
    }

    IEnumerator Dash(float time, float force, GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MechCharacterController mcc = player.GetComponent<MechCharacterController>();
        Transform camTransform = player.GetComponentInChildren<MechLookPitch>().transform;
        Vector3 velocityNorm = rb.velocity;

        if (rb.velocity != Vector3.zero)
        {
            velocityNorm = rb.velocity;
            rb.velocity = mcc.movePosition() * force;
        }
        else
        {
            rb.velocity = camTransform.transform.forward * force;
        }
        yield return new WaitForSeconds(time);
        rb.velocity = velocityNorm;
    }

    [ServerRpc(RequireOwnership = false)]
    public void LockdownDisableServerRPC(NetworkObjectReference obj)
    {
        LockdownDisableClientRPC(obj);
    }
    [ClientRpc]
    public void LockdownDisableClientRPC(NetworkObjectReference obj)
    {
        NetworkObject networkObject;
        obj.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.GetComponent<MeshCollider>().enabled = false;
            networkObject.GetComponent<MeshRenderer>().enabled = false;
            networkObject.GetComponent<CS_Lockdown>().bubble.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void LockdownBubbleServerRPC(NetworkObjectReference obj, Vector3 target, float speed, float time)
    {
        LockdownBubbleClientRPC(obj, target, speed, time);
    }
    [ClientRpc]
    public void LockdownBubbleClientRPC(NetworkObjectReference obj, Vector3 target, float speed, float time)
    {
        NetworkObject networkObject;
        obj.TryGet(out networkObject);
        if (networkObject != null)
        {
            GameObject bubble = networkObject.GetComponent<CS_Lockdown>().bubble;
            if (bubble.transform.localScale.x < target.x)
            {
                bubble.transform.localScale += new Vector3(1, 1, 1) * speed * time;
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void ApplyAnimationServerRPC(NetworkObjectReference obj, string animString, bool booleon)
    {
        ApplyAnimationClientRPC(obj, animString, booleon);
    }

    [ClientRpc]
    private void ApplyAnimationClientRPC(NetworkObjectReference obj, string animString, bool booleon)
    {
        NetworkObject networkObject;
        obj.TryGet(out networkObject);
        if (networkObject != null)
        {
            Animator anim;
            if (networkObject.GetComponent<Animator>() == null)
            {
                anim = networkObject.GetComponentInChildren<Animator>();
            }
            else
            {
                anim = networkObject.GetComponent<Animator>();
            }
            anim.SetBool(animString, booleon);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LineRenderServerRPC(NetworkObjectReference lr, Vector3 posOne, Vector3 posTwo)
    {
        LineRenderClientRPC(lr, posOne, posTwo);
    }

    [ClientRpc]
    private void LineRenderClientRPC(NetworkObjectReference lr, Vector3 posOne, Vector3 posTwo)
    {
        NetworkObject networkObject;
        lr.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.GetComponent<LineRenderer>().positionCount = 2;
            networkObject.GetComponent<LineRenderer>().SetPosition(0, posOne);
            networkObject.GetComponent<LineRenderer>().SetPosition(1, posTwo);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RecallHealthServerRPC(NetworkObjectReference player, int amount)
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.GetComponent<HealthComponent>().SetHealth(amount);
        }
    }
    

    [ServerRpc(RequireOwnership = false)]
    public void SlowEnemyServerRPC(NetworkObjectReference otherUnit, float speed)
    {
        SlowClientRPC(otherUnit, speed);
    }

    [ClientRpc]
    private void SlowClientRPC(NetworkObjectReference otherUnit, float speed)
    {
        NetworkObject networkObject;
        otherUnit.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.GetComponent<CS_PlayerStats>().speed = speed;
        }
    }



    [ClientRpc]
    public void UpdateTransformClientRpc(NetworkObjectReference abilityRef, Vector3 newPos, Quaternion newRot)
    {
        NetworkObject networkObject;
        abilityRef.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.transform.position = newPos;
            networkObject.transform.rotation = newRot;
        }
    }

    [ClientRpc]
    void MechWeaponFiredClientRpc(Vector3 WeaponFireLocation, NetworkObjectReference firingPoint)
    {
        //ScreenLog.Instance.Print("Weapon fired at " + WeaponFireLocation, Color.cyan);

        // Spawn audio, muzzle flash vfx, etc... here 
        Transform muzzlePoint;
        NetworkObject networkObject;
        firingPoint.TryGet(out networkObject);
        if (networkObject != null)
        {
            if (networkObject.GetComponentInChildren<WeaponScript>())
            {
                muzzlePoint = networkObject.GetComponentInChildren<WeaponScript>().attackPoint;
            }
            else
            {
                muzzlePoint = networkObject.transform;
            }
        }
        else
        {
            muzzlePoint = null;
            Debug.Log("Cant get muzzle point");
        }

        if (muzzlePoint == null)
        {
            Instantiate(muzzleFlash, WeaponFireLocation, Quaternion.identity);
        }
        else
        {
            Instantiate(muzzleFlash, muzzlePoint.position, Quaternion.identity,
                muzzlePoint);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMaterialServerRpc(NetworkObjectReference objectToUpdate, string materialName)
    {
        ChangeMaterialClientRpc(objectToUpdate, materialName);
    }

    [ClientRpc]
    void ChangeMaterialClientRpc(NetworkObjectReference objectToUpdate, string materialName)
    {
        NetworkObject networkObject;
        objectToUpdate.TryGet(out networkObject);
        if (!networkObject) return;

        Material newMaterial = null;
        foreach (AbilityMaterial material in abilityMaterials)
        {
            if (material.materialName == materialName)
            {
                newMaterial = material.material;
                break;
            }
        }

        if (newMaterial)
        {
            //networkObject.GetComponent<Renderer>().material = newMaterial;
            GameObject model = networkObject.GetComponent<CS_UseAbilities>().model;
            foreach (Renderer rend in model.GetComponentsInChildren<Renderer>())
            {
                rend.material = newMaterial;
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]

    public void DisableAbilitiesServerRPC(NetworkObjectReference player, float duration) 
    {
        DisableAbilitesClientRPC(player, duration);
    }

    [ClientRpc]

    public void DisableAbilitesClientRPC(NetworkObjectReference player, float duration) 
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            StartCoroutine(Disable(networkObject.GetComponent<CS_PlayerStats>(), duration));        
        }


    }

    IEnumerator Disable(CS_PlayerStats script, float time) 
    {
        script.hacked = true;
        yield return new WaitForSeconds(time);
        script.hacked = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeCloneShaderServerRPC(NetworkObjectReference obj, string material)
    {
        ChangeCloneShaderClientRPC(obj, material);
    }

    [ClientRpc]
    private void ChangeCloneShaderClientRPC(NetworkObjectReference obj, string material)
    {
        NetworkObject networkObject;
        obj.TryGet(out networkObject);
        if (!networkObject) return;
        GameObject clone = networkObject.gameObject.GetComponent<CS_CloneTrap>().clone;

        foreach (SkinnedMeshRenderer rend in clone.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (rend != null)
            {
                if (material == "red") rend.material = redTex;
                else if (material == "blue") rend.material = blueTex;
            }
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void RevealEnemyServerRPC (ulong ID, NetworkObjectReference player, float duration)
    {
        RevealEnemyClientRPC(ID, player, duration);
    }

    [ClientRpc]
    public void RevealEnemyClientRPC (ulong ID, NetworkObjectReference player, float duration)
    {
        if (ID != NetworkManager.Singleton.LocalClientId) return;

        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            foreach (Renderer rend in networkObject.GetComponentsInChildren<Renderer>())
            {
                //rend.material = abilityMaterials[0].material;
                if (rend.material.shader != revealShader)
                {
                    StartCoroutine(RevealForTime(rend.material, rend.material.shader, duration));
                }
            }
        }
    }

    IEnumerator RevealForTime(Material mat, Shader origin, float duration)
    {
        mat.shader = revealShader;
        yield return new WaitForSeconds(duration);
        mat.shader = origin;
    }

    [ClientRpc]
    void OnBulletImpactClientRpc(Vector3 ImpactLocation, Vector3 ImpactedSurfaceNormal, int surfaceID)
    {
        //ScreenLog.Instance.Print("Bullet impact at " + ImpactLocation, Color.cyan);

        // Spawn bullet impact vfx, audio, decals, etc... here

        if (surfaceID == 0)
        {
            GameObject impactGO = Instantiate(decal, ImpactLocation + ImpactedSurfaceNormal * 0.001f,
                Quaternion.LookRotation(ImpactedSurfaceNormal));
            Destroy(impactGO, 10f);
        }
        else if (surfaceID == 1)
        {
            GameObject impactGO = Instantiate(sparks, ImpactLocation + ImpactedSurfaceNormal * 0.001f,
                Quaternion.LookRotation(ImpactedSurfaceNormal));
            Destroy(impactGO, 10f);
        }
        else if (surfaceID == 2)
        {
            GameObject impactGO = Instantiate(ripple, ImpactLocation + ImpactedSurfaceNormal * 0.001f,
                Quaternion.LookRotation(ImpactedSurfaceNormal));
            Destroy(impactGO, 10f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleMechWeaponFireServerRpc(Vector3 WeaponFireLocation, Vector3 weaponFirePoint,
        Vector3 WeaponFireDirection, int WeaponDamage, NetworkObjectReference firingPoint,
        NetworkObjectReference player,
        float MaxWeaponRange = float.PositiveInfinity)
    {
        //ScreenLog.Instance.Print("Server handling mech weapon fire from location " + WeaponFireLocation, Color.green);

        // Notify clients a weapon has been fired at location
        MechWeaponFiredClientRpc(weaponFirePoint, firingPoint);

        // Change method of firing a bullet here. If you wanted to use projectiles, you would spawn one here. Raycasting is currently implemented
        // Raycast a bullet from the weapon fire location, into the scene, searching for objects with health components
        RaycastHit HitInfo;
        if(Physics.Raycast(WeaponFireLocation, WeaponFireDirection, out HitInfo, MaxWeaponRange, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            NetworkObject networkObject;
            player.TryGet(out networkObject);

            // Notify clients a bullet has impacted a surface
            if (!HitInfo.collider.isTrigger)
            {
                if (HitInfo.collider.gameObject.GetComponent<CS_PlayerStats>())
                {
                    OnBulletImpactClientRpc(HitInfo.point, HitInfo.normal, 1);
                }
                else if (HitInfo.collider.transform.parent != null && HitInfo.collider.gameObject.transform.parent.GetComponent<CS_BubbleShield>())
                {
                    OnBulletImpactClientRpc(HitInfo.point, HitInfo.normal, 2);
                }
                else if (HitInfo.collider.gameObject.GetComponent<CS_UAV>() || HitInfo.collider.gameObject.GetComponent<CS_MotionSensor>() )
                {
                    OnBulletImpactClientRpc(HitInfo.point, HitInfo.normal, 1);
                }
                else
                {
                    OnBulletImpactClientRpc(HitInfo.point, HitInfo.normal,
                        0); // Will need to create a object type integer ID to send here if you want to know what was hit (player, crate, ground, etc...)
                }
            }
            else if (HitInfo.collider.gameObject.GetComponent<CS_CloneTrap>())
            {
                if (IsEnemy(HitInfo.collider.gameObject, networkObject.GetComponent<Entity>()))
                {
                    Debug.Log("Enemy");
                    RevealEnemyClientRPC(GetOwner(HitInfo.collider.gameObject).gameObject.GetComponent<Entity>().clientId, player, 3);
                    HitInfo.collider.GetComponent<CS_CloneTrap>().Death();
                }
                else
                {
                    Debug.Log("Friendly");
                }
            }
            else if (HitInfo.collider.gameObject.GetComponent<CS_BubbleshieldEmitter>())
            {
                
                HitInfo.collider.GetComponent<IDamageable>()?.TakeDamageServerRpc(WeaponDamage);
                
            }

            // Check if the hit object contains a health component for the player
            var combatComponent = HitInfo.collider.gameObject.GetComponent<CombatComponent>();
            if (combatComponent && IsEnemy(HitInfo.collider.gameObject, networkObject.gameObject))
            {
                //ScreenLog.Instance.Print("Server detected bullet impact with object containing a health component", Color.green);
                Entity ownerEntity = networkObject.GetComponent<Entity>();
                UpdateHitDetectedServerRpc(ownerEntity.clientId, HitInfo.collider.gameObject.GetComponent<Entity>().clientId);
                combatComponent.TakeDamageServerRpc(WeaponDamage);
            }
            //Hit other objects that dont have networked health component like destroyable abilities
            else if (HitInfo.collider.GetComponent<IDamageable>() != null &&
                     IsEnemy(GetOwner(HitInfo.collider.gameObject).gameObject, networkObject.gameObject))
            {
                HitInfo.collider.GetComponent<IDamageable>()?.TakeDamageServerRpc(WeaponDamage);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void WeaponFiredServerRpc(NetworkObjectReference mccReference, Vector3 fireLocation, Vector3 fireDirection)
    {
        // Get weapon info
        if (!mccReference.TryGet(out NetworkObject mccNetObj)) return;
        if (!(mccNetObj.GetComponentInChildren<WeaponScript>() is WeaponScript weapon)) return;
        WeaponStats weaponStats = weapon.stats;

        // Notify clients that a weapon has been fired.
        WeaponFiredClientRpc(mccReference);

        LayerMask mask = Physics.IgnoreRaycastLayer;

        // Weapon firing logic. TODO: Projectile support
        if (Physics.Raycast(fireLocation, fireDirection, out RaycastHit hitInfo, Mathf.Infinity, ~mask))
        {
            // Notify clients that a bullet has impacted a surface.
            OnBulletImpactClientRpc(hitInfo.point, hitInfo.normal, hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground") ? 0 : 1);
            
            Entity attacker = weapon.mechCharacterController.GetComponent<Entity>();
            Entity target = hitInfo.collider.gameObject.GetComponent<Entity>();
            // Do Clone Trap check (this should really, really, really, be handled on the CloneTrap and not here...
            if (hitInfo.collider.gameObject.TryGetComponent(out CS_CloneTrap cloneTrap))
            {
                Entity owner = GetOwner(cloneTrap.gameObject);
                if (owner.TeamName != attacker.TeamName)
                {
                    RevealEnemyClientRPC(owner.clientId, mccNetObj, 3);
                    cloneTrap.Death();
                   
                }
            }
            // Check if the hit object contains a health component.
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                // Check if this is friendly fire.
                if (target == null || attacker.TeamName != target.TeamName)
                {
                    int damage = (int)weaponStats.damageFalloff.Evaluate(hitInfo.distance);
                    if (damage > 0) damageable.TakeDamageServerRpc(damage);
                }
            }

        }
    }

    [ClientRpc]
    public void WeaponFiredClientRpc(NetworkObjectReference mccReference)
    {
        //ScreenLog.Instance.Print("Weapon fired at " + WeaponFireLocation, Color.cyan);

        // Spawn audio, muzzle flash vfx, etc... here 
        if (!mccReference.TryGet(out NetworkObject mccNetObj)) return;
        if (!(mccNetObj.GetComponentInChildren<WeaponScript>() is WeaponScript weapon)) return;

        Instantiate(muzzleFlash, weapon.muzzleFlash.transform.position, Quaternion.identity);
    }

    [ServerRpc(RequireOwnership = false)]

    public void SpeedboostServerRPC(NetworkObjectReference player, float speedIncrease, float duration)
    {
        SpeedboostClientRPC(player,speedIncrease, duration);
    }

    [ClientRpc]

    public void SpeedboostClientRPC(NetworkObjectReference player, float speedIncrease, float duration)
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            StartCoroutine(SpeedBoostCall(networkObject.GetComponent<CS_PlayerStats>(), speedIncrease, duration));
        }
    }

    IEnumerator SpeedBoostCall(CS_PlayerStats PS, float speedIncrease, float duration)
    {
        PS.speed *= speedIncrease;
        yield return new WaitForSeconds(duration);
        PS.speed /= speedIncrease;
    }

    [ServerRpc(RequireOwnership = false)]

    public void TeleportServerRPC(NetworkObjectReference player, Vector3 newPos)
    {
        TeleportClientRPC(player, newPos);
    }

    [ClientRpc]

    public void TeleportClientRPC(NetworkObjectReference player, Vector3 newPos)
    {
        NetworkObject networkObject;
        player.TryGet(out networkObject);
        if (networkObject != null)
        {
            networkObject.transform.position = newPos;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UpdateHitDetectedServerRpc(ulong clientID, ulong hitID)
    {
        UpdateHitDetectedClientRpc(clientID, hitID);
    }
    
    [ClientRpc]
    private void UpdateHitDetectedClientRpc(ulong clientID, ulong hitID)
    {
        cross.clientID = clientID;
        //cross.personalID = hitID;
        cross.SetHitmarker();
    }
}
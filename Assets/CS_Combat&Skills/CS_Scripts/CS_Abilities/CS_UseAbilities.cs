using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CS_UseAbilities : NetworkBehaviour
{
    public enum Class
    {
        Damage,
        Movement,
        Defence,
        Recon
    }

    public Class playerClass = Class.Damage;

    [SerializeField] private PlayerClassSO playerClassSO = default;
    public PlayerClassSO GetPlayerClass() => playerClassSO;

    public GameObject model;
    public GameObject recallUIPrefab;
    private GameObject canvasObj;
    [SerializeField] private WeaponScript weaponScript;

    public float CooldownAbilityOne;
    public float CooldownAbilityTwo;
    public float CooldownAbilityThree;
    
   // public InputActionManager inputActions;
    public bool bioticGren;

    //Recall Ability (Don't delete without telling me please.)
    public float maxDuration = 10;
    public float saveInterval = 0.1f;
    public float recallSpeed = 50;
    private List<Vector3> positions;
    private List<int> healths;
    private bool recalling;
    private float saveStatsTimer;
    private float maxStatsStored;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        PlayerClassUI.OnClassAssigned += SetClass;

        InputManager.MECH.QAbility.started += OnAbilityOnePress;
        InputManager.MECH.EAbility.started += OnAbilityTwoPress;
        InputManager.MECH.FAbility.started += OnAbilityThreePress;

        maxStatsStored = maxDuration / saveInterval;
        positions = new List<Vector3>();
        healths = new List<int>();
    }

    public void SetClass(int classID) => SetClassServerRPC(OwnerClientId, classID);

    [ServerRpc(RequireOwnership = false)]
    private void SetClassServerRPC(ulong playerID, int classID) => SetClassClientRPC(playerID, classID);

    [ClientRpc]
    private void SetClassClientRPC(ulong playerID, int classID)
    {
        if (playerID != OwnerClientId) return;
        playerClassSO = PlayerClassSO.GetClassFromID(classID);
        weaponScript.AssignModel(playerClassSO.GetWeapon());
    }


    private void OnAbilityOnePress(InputAction.CallbackContext context)
    {
        if (CooldownAbilityOne <= 0)
        {
            if (!recalling && positions.Count > 0 && playerClassSO.GetAbility(0).GetAbilityName() == "Recall")
            {
                recalling = true;
                canvasObj = Instantiate(recallUIPrefab, transform);
                GetComponent<MechCharacterController>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;

                var ability = playerClassSO.GetAbility(0);
                CooldownAbilityOne = ability.GetCooldownTime();
            }
            else
            {
                var ability = playerClassSO.GetAbility(0);
                ability.CastAbility(GetComponent<Entity>(), 0);
                CooldownAbilityOne = ability.GetCooldownTime();
            }
        }
    }

    private void OnAbilityTwoPress(InputAction.CallbackContext context)
    {
        if (CooldownAbilityTwo <= 0)
        {
            var ability = playerClassSO.GetAbility(1);
            ability.CastAbility(GetComponent<Entity>(), 1);
            CooldownAbilityTwo = ability.GetCooldownTime();
        }
    }

    private void OnAbilityThreePress(InputAction.CallbackContext context)
    {
        if (CooldownAbilityThree <= 0)
        {
            var ability = playerClassSO.GetAbility(2);
            ability.CastAbility(GetComponent<Entity>(), 2);
            CooldownAbilityThree = ability.GetCooldownTime();
        }
    }

   /* public void QAbility(InputAction.CallbackContext context)
    {
        switch (playerClass)
        {
            case Class.Damage:
                if (CooldownAbilityOne <= 0)
                {
                    damageAbilityOne.CastAbility(GetComponent<Entity>());
                    CooldownAbilityOne = damageAbilityOne.cooldownTime;
                }
                break;
            case Class.Movement:
                
                if (CooldownAbilityOne <= 0)
                {
                    if (!recalling && positions.Count > 0 && movementAbilityOne.id == CS_MovementAbilities.AbilityName.Recall)
                    {
                        recalling = true;
                        canvasObj = Instantiate(canvas, transform);
                        GetComponent<MechCharacterController>().enabled = false;
                        GetComponent<CapsuleCollider>().enabled = false;
                        GetComponent<Rigidbody>().isKinematic = true;
                        //CooldownAbilityTwo = movementAbilityTwo.cooldownTime;
                    }
                    else
                    {
                        movementAbilityOne.CastAbility(GetComponent<Entity>());
                        CooldownAbilityOne = movementAbilityOne.GetCooldownTime();
                    }
                }
                break;
            case Class.Defence:
                if (CooldownAbilityOne <= 0)
                {
                    //bioticGren = false;
                    defenceAbilityOne.CastAbility(GetComponent<Entity>());
                    CooldownAbilityOne = defenceAbilityOne.cooldownTime;
                }
                break;
            case Class.Recon:
                if (CooldownAbilityOne <= 0)
                {
                    reconAbilityOne.CastAbility(GetComponent<Entity>());
                    CooldownAbilityOne = reconAbilityOne.cooldownTime;
                }
                break;
        }
    }
    public void EAbility(InputAction.CallbackContext context)
    {

        switch (playerClass)
        {
            case Class.Damage:
                if (CooldownAbilityTwo <= 0)
                {
                    damageAbilityTwo.CastAbility(GetComponent<Entity>());
                    CooldownAbilityTwo = damageAbilityTwo.cooldownTime;
                }
                break;
            case Class.Movement:
                if (CooldownAbilityTwo <= 0)
                {
                    if (!recalling && positions.Count > 0 && movementAbilityTwo.id == CS_MovementAbilities.AbilityName.Recall)
                    {
                        recalling = true;
                        canvasObj = Instantiate(canvas, transform);
                        GetComponent<MechCharacterController>().enabled = false;
                        GetComponent<CapsuleCollider>().enabled = false;
                        GetComponent<Rigidbody>().isKinematic = true;
                        //CooldownAbilityTwo = movementAbilityTwo.cooldownTime;
                    }
                    else
                    {
                        movementAbilityTwo.CastAbility(GetComponent<Entity>());
                        CooldownAbilityTwo = movementAbilityTwo.GetCooldownTime();
                    }
                    
                }
                break;
            case Class.Defence:
                 if (CooldownAbilityTwo <= 0)
                 {
                    //bioticGren = true;
                    defenceAbilityTwo.CastAbility(GetComponent<Entity>());
                    CooldownAbilityTwo = defenceAbilityTwo.cooldownTime;
                 }
                 break;
            case Class.Recon:
                if (CooldownAbilityTwo <= 0)
                {
                    reconAbilityTwo.CastAbility(GetComponent<Entity>());
                    CooldownAbilityTwo = reconAbilityTwo.cooldownTime;
                }
                break;
        }
    }
    public void FAbility(InputAction.CallbackContext context)
    {
        switch (playerClass)
        {
            case Class.Damage:
                if (CooldownAbilityThree <= 0)
                {
                    damageAbilityThree.CastAbility(GetComponent<Entity>());
                    CooldownAbilityThree = damageAbilityThree.cooldownTime;
                }
                break;
            case Class.Movement:
                if (CooldownAbilityThree <= 0)
                {
                    if (!recalling && positions.Count > 0 && movementAbilityThree.id == CS_MovementAbilities.AbilityName.Recall)
                    {
                        recalling = true;
                        canvasObj = Instantiate(canvas, transform);
                        GetComponent<MechCharacterController>().enabled = false;
                        GetComponent<CapsuleCollider>().enabled = false;
                        GetComponent<Rigidbody>().isKinematic = true;
                        //CooldownAbilityTwo = movementAbilityTwo.cooldownTime;
                    }
                    else
                    {
                        movementAbilityThree.CastAbility(GetComponent<Entity>());
                        CooldownAbilityThree = movementAbilityThree.GetCooldownTime();
                    }
                    break;
                case Class.Defence:
                if (CooldownAbilityThree <= 0)
                {
                    //bioticGren = false;
                    defenceAbilityThree.CastAbility(GetComponent<Entity>());
                    CooldownAbilityThree = defenceAbilityThree.GetCooldownTime();
                }
                break;
                case Class.Recon:
                    if (CooldownAbilityThree <= 0)
                    {
                        reconAbilityThree.CastAbility(GetComponent<Entity>());
                        CooldownAbilityThree = reconAbilityThree.GetCooldownTime();
                    }
                    break;
            }
        }*/

    private void Update()
    {
        var elapsed = Time.deltaTime;

        if (CooldownAbilityOne >= 0)
        {
            CooldownAbilityOne -= elapsed;
        }
        if (CooldownAbilityTwo>= 0)
        {
            CooldownAbilityTwo -= elapsed;
        }
        if (CooldownAbilityThree>= 0)
        {
            CooldownAbilityThree -= elapsed;
        }
        
        //Recall Ability (Don't delete without telling me please)
        if (playerClassSO != null && playerClassSO.GetAbility(0).GetAbilityName() == "Recall")
        {
            if (!recalling)
            {
                if (saveStatsTimer > 0)
                {
                    saveStatsTimer -= Time.deltaTime;
                }
                else
                {
                    StoreStats();
                }
            }
            else
            {
                if (positions.Count > 0)
                {
                    transform.position = Vector3.Lerp(transform.position, positions[0], recallSpeed * Time.deltaTime);
                    float dist = Vector3.Distance(transform.position, positions[0]);
                    if (dist < 0.25f)
                    {
                        SetStats();
                    }
                }
                else
                {
                    recalling = false;
                    Destroy(canvasObj);
                    GetComponent<MechCharacterController>().enabled = true;
                    GetComponent<CapsuleCollider>().enabled = true;
                    GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
        
    }

    void StoreStats()
    {
        saveStatsTimer = saveInterval;
        positions.Insert(0, transform.position);
        healths.Insert(0, GetComponent<HealthComponent>().GetHealth());

        if (positions.Count > maxStatsStored)
        {
            positions.RemoveAt(positions.Count - 1);
        }
        if (healths.Count > maxStatsStored)
        {
            healths.RemoveAt(positions.Count - 1);
        }
    }

    void SetStats()
    {
        ServerAbilityManager.Instance.RecallHealthServerRPC(GetComponent<NetworkObject>(), healths[0]);
        transform.position = positions[0];
        positions.RemoveAt(0);
        healths.RemoveAt(0);
    }
}

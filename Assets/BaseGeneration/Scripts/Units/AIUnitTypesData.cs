using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/Units/Unit Generation")]
public class AIUnitTypesData : ScriptableObject
{
    
    [Serializable]
    public class UnitTypeInfo
    {
        [Header("Unit information")]
        public string unitTitle;
        public EUnitTypes unitType;
        public GameObject modelPrefab;
        public int teamColorMaterialIndex;
        public int damageStrength;
        public Sprite unitImage;
        [TextArea(15,5)]
        public string unitDesc;
        
        [Header("Unit Base Inforrmation")] 
        public int unitSpawnAmount;
        public int unitCost;
        //public int unitInstantMultiplier;
        public int unitOffsetPos;
        public float unitSpawnTime;
    }
    
    [SerializeField] private List<UnitTypeInfo> m_units = new List<UnitTypeInfo>();
    private Dictionary<EUnitTypes, UnitTypeInfo> m_typeDictionary;

    public List<UnitTypeInfo> GetAllUnits()
    {
        return m_units;
    }

    private void OnValidate()
    {
        m_typeDictionary = new Dictionary<EUnitTypes, UnitTypeInfo>();
        foreach (UnitTypeInfo unit in m_units)
        {
            if (m_typeDictionary.ContainsKey(unit.unitType))
            {
                Debug.LogError("Multiple units prefabs set for a single unit type: " + unit.unitType.ToString());
            }

            m_typeDictionary[unit.unitType] = unit;
        }
    }

    //public GameObject BuildUnitOfType(EUnitTypes type, Transform spawnTransform)
    //{
        //if (m_typeDictionary == null)
        //{
        //    OnValidate();
        //}

        //if (!m_typeDictionary.ContainsKey(type))
        //{
        //    Debug.LogError("Could not find unit of type: " + type.ToString());
        //    return null;
        //}

        //return Instantiate(m_typeDictionary[type].unitPrefab, spawnTransform);
    //}

    public UnitTypeInfo GetUnitInfo(EUnitTypes type)
    {
        if (m_typeDictionary == null)
        {
            OnValidate();
        }

        if (!m_typeDictionary.ContainsKey(type))
        {
            Debug.LogError("Could not find unit of type" + type.ToString());
            return null;
        }

        return m_typeDictionary[type];
    }

    
}

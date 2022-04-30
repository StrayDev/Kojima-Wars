using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Base Defences/Defences Builder")]
public class DefenceTypesData : ScriptableObject
{
    [Serializable]
    public class DefenceTypeInfo
    {
        public EBaseDefenceTypes DefencesType;
        public BaseDefence Prefab;
        public Sprite Icon;
        public string Name;
        public string Description;
        public int Cost;
    }

    [SerializeField] private List<DefenceTypeInfo> m_defences = new List<DefenceTypeInfo>();

    private Dictionary<EBaseDefenceTypes, DefenceTypeInfo> m_typeDictionary;

    public List<DefenceTypeInfo> GetAllDefences()
    {
        return m_defences;
    }

    private void OnValidate()
    {
        m_typeDictionary = new Dictionary<EBaseDefenceTypes, DefenceTypeInfo>();
        foreach (DefenceTypeInfo defence in m_defences)
        {
            if (m_typeDictionary.ContainsKey(defence.DefencesType))
            {
                Debug.LogError("Multiple defence prefabs set for a single defence type: " + defence.DefencesType.ToString());
            }

            m_typeDictionary[defence.DefencesType] = defence;
        }
    }

    public BaseDefence BuildDefenceOfType(EBaseDefenceTypes type)
    {
        if (m_typeDictionary == null)
        {
            OnValidate();
        }

        if (!m_typeDictionary.ContainsKey(type))
        {
            Debug.LogError("Could not find defence of type: " + type.ToString() + ". Please set up the Defence builder to correctly have this defence info for this type");
        }

        return Instantiate(m_typeDictionary[type].Prefab);
    }

    public DefenceTypeInfo GetDefenceInfo(EBaseDefenceTypes type)
    {
        if (m_typeDictionary == null)
        {
            OnValidate();
        }

        if (!m_typeDictionary.ContainsKey(type))
        {
            Debug.LogError("Could not find the cost of the defence of type: " + type.ToString() + ". Please set up the Defence builder to correctly have the cost for this type");
            return null;
        }

        return m_typeDictionary[type];
    }
}
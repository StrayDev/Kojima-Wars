using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Base Generation/Generation Rates")]
public class GenerationRates : ScriptableObject
{
    [Serializable]
    public struct GenerationValues
    {
        public int amount;
        public float time;
    }

    [Serializable]
    public struct GenerationValuesPerRank
    {
        public EBaseRank rank;
        public GenerationValues generationValues;
    }

    [Serializable]
    public struct UnitTimeMultiplyerPerRank
    {
        public EBaseRank rank;
        public float timeMultiplyer;
    }

    [SerializeField] private List<GenerationValuesPerRank> currencyGenerationRates = new List<GenerationValuesPerRank>();
    [SerializeField] private List<UnitTimeMultiplyerPerRank> unitGenerationMultiplyerPerRank = new List<UnitTimeMultiplyerPerRank>();

    public GenerationValues GetCurrencyGenerationRatesOfRank(EBaseRank rank)
    {
        return currencyGenerationRates.Find(r => r.rank == rank).generationValues;
    }

    public float GetTimeMultiplyerForRank(EBaseRank rank)
    {
        return unitGenerationMultiplyerPerRank.Find(r => r.rank == rank).timeMultiplyer;
    }
}

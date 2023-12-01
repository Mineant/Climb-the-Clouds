using System.Collections;
using System.Collections.Generic;
using Kryz.CharacterStats;
using UnityEngine;

public class StatValueUIContainer : Container<StatValueUIProduct, StatValueUIProductArgs>
{
    public void Generate(BasicStats basicStats, StatFloatDictionary defaultBaseParameters = null)
    {
        DestroyAllProducts();

        foreach (var statValuePair in basicStats.StatDictionaryAllInOne.BaseValueStatDictionary)
        {
            if (defaultBaseParameters != null && defaultBaseParameters.TryGetValue(statValuePair.Key, out var value) && value == statValuePair.Value) continue;  // Continue if the value is at its default.
            GenerateNewProduct(new(statValuePair.Key, statValuePair.Value));
        }

        foreach (var statValuePair in basicStats.StatDictionaryAllInOne.FlatModifierStatDictionary)
        {
            GenerateNewProduct(new(statValuePair.Key, StatModType.Flat, statValuePair.Value));
        }

        foreach (var statValuePair in basicStats.StatDictionaryAllInOne.PercentAddModifierStatDictionary)
        {
            GenerateNewProduct(new(statValuePair.Key, StatModType.PercentAdd, statValuePair.Value));
        }

        foreach (var statValuePair in basicStats.StatDictionaryAllInOne.PercentMultModifierStatDictionary)
        {
            GenerateNewProduct(new(statValuePair.Key, StatModType.PercentMult, statValuePair.Value));
        }
    }
}

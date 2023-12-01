using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using DamageNumbersPro;
using System.Linq;

public class DamageNumberManager : MMSingleton<DamageNumberManager>
{
    [System.Serializable]
    public class DamageNumberTypePrefab
    {
        public DamageNumberType Type;
        public DamageNumber DamageNumberPrefab;
    }

    public List<DamageNumberTypePrefab> DamageNumberTypePrefabs;


    private DamageNumber FindDamageNumber(DamageNumberType type)
    {
        DamageNumber target = DamageNumberTypePrefabs.FirstOrDefault(p => p.Type == type)?.DamageNumberPrefab;
        if (target == null)
        {
            Debug.LogError($"Cannot find the prefab for type {target}");
            return null;
        }
        return target;
    }

    public virtual void GenerateDamageNumber(DamageNumberType type, Vector3 position, float value)
    {
        DamageNumber damageNumber = FindDamageNumber(type);
        if (damageNumber == null) return;

        damageNumber.Spawn(position, value);
    }

    public virtual void GenerateDamageNumber(DamageNumberType type, Vector3 position, string text)
    {
        DamageNumber damageNumber = FindDamageNumber(type);
        if (damageNumber == null) return;

        damageNumber.Spawn(position, text);
    }
}

public enum DamageNumberType
{
    Damage,
    Heal,
}

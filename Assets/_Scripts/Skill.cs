using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu]
public class Skill : BaseInventoryItem
{
    public WuXiaWeapon WuXiaWeapon;

    public List<SkillType> SkillTypes;
    public Rarity Rarity;
    public AudioClip AuraSkillAudio;

    public static StatFloatDictionary SkillDefaultParameters = new StatFloatDictionary()
    {
        {Stat.TrueDamage,0f},
        {Stat.PhysicalDamage,0f},
        {Stat.SpellDamage,0f},
        {Stat.RecoveryTime,0f},
        {Stat.SkillTime,0f},
        {Stat.BurstLength,1f},
        {Stat.ProjectilesPerShot,1f},
        {Stat.Spread,0f},
        {Stat.Range,1f},
    };

    public override int GetBuyCost()
    {
        return (int)((((int)Rarity + 1) * 60) * Mathf.Pow(1.5f, ((int)(Rarity))));
    }

    public override int GetSellRevenue()
    {
        return (int)(GetBuyCost() * 0.5f);
    }

}

public enum SkillType
{
    Sword = 0,  // Weapon Types
    Blade,
    Spear,
    Knife,
    Bow,
    Fan,
    Staff,
    DanTian,

    True = 100, // Damage Types
    Physical,
    Spell,

    Projectile = 200, // Projectile Type?


    Slash = 300, // Weapon Techniques
    Thrust,
    Slam,

    Multi = 400, // Special
    Support,
    Life,
    Continuous,

    Fire = 500, // Spell Type
    Ice,
    Thunder,


}

public enum Rarity
{
    Common, Uncommon, Rare, Legendary
}

public enum WeaponTypeForReference
{
    Sword = 1,
    BigSword = 2,
    Spear = 3,
    Blade = 4,
    Knife = 5,
    Bow = 6,
    Staff = 7,
    Fan = 8,
    Nunchucks = 9,
}

public interface IGoods
{
    int GetBuyCost();
    int GetSellRevenue();
}
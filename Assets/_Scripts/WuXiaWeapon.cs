using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(BaseStats))]
public class WuXiaWeapon : BaseWeapon
{
    [MMInspectorGroup("Wuxia", true, 25)]
    public BaseUpgradeMono AuraSkillMono;
    public MMF_Player AuraSkillFeedback;
    protected bool _isEmpoweredInput;
    protected CharacterWuXiaCombat _characterWuXiaCombat;
    protected CharacterWuXiaResource _characterWuXiaResource;

    protected override void Awake()
    {
        base.Awake();
        AuraSkillMono.Initialize(this.gameObject);
    }


    public override void Initialization()
    {
        base.Initialization();
        if (WeaponName.Length == 0) Debug.LogError("WeaponName of wuxia weapon cannot be null. Will be used for ID checking.");
    }


    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
    {
        GameObject projectile = base.SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);


        if (projectile != null && _isEmpoweredInput && projectile.GetComponent<Projectile>().TargetDamageOnTouch is BaseDamageOnTouch baseDamageOnTouch)
        {
            baseDamageOnTouch.ChangeHitStun(true);
        }


        return projectile;
    }

    public override void SetOwner(Character newOwner, CharacterHandleWeapon handleWeapon)
    {
        base.SetOwner(newOwner, handleWeapon);

        if (Owner != null)
        {
            _characterWuXiaCombat = Owner.FindAbility<CharacterWuXiaCombat>();
            _characterWuXiaResource = Owner.FindAbility<CharacterWuXiaResource>();
        }

    }

    public override void WeaponInputStart()
    {
        if (Owner != null && _characterWuXiaResource != null)
            Debug.LogError("Wuxia Weapons cannot should use WuXiaWeaponinputStart!");

        _isEmpoweredInput = false;
        base.WeaponInputStart();
    }

    public void WuXiaWeapoonInputStart(bool empowered)
    {
        if (empowered)
        {
            AuraSkillFeedback?.PlayFeedbacks();
            AuraSkillMono.TriggerAllAbilities();
        }
        _isEmpoweredInput = empowered;
        base.WeaponInputStart();
    }

    public override void OnProjectileHit()
    {
        base.OnProjectileHit();

        if (_characterWuXiaCombat != null)
        {
            _characterWuXiaCombat.OnProjectileHit();
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class BaseProjectile : Projectile
{
    // public float DamageDelay = 0f;
    public bool ClampSpeed;
    public Vector2 SpeedLimit;
    private BaseWeapon _baseWeapon;


    public override void SetWeapon(Weapon newWeapon)
    {
        base.SetWeapon(newWeapon);
        if (_weapon && _weapon is BaseWeapon baseWeapon)
        {
            _baseWeapon = baseWeapon;
        }
    }

    public override void SetOwner(GameObject newOwner)
    {
        _owner = newOwner;
        DamageOnTouch damageOnTouch = this.gameObject.MMGetComponentNoAlloc<DamageOnTouch>();
        if (damageOnTouch != null)
        {
            damageOnTouch.Owner = newOwner;
            if (!DamageOwner)
            {
                damageOnTouch.ClearIgnoreList();
                damageOnTouch.IgnoreGameObject(newOwner);
            }

            if (damageOnTouch is BaseDamageOnTouch baseDamageOnTouch)
            {
                baseDamageOnTouch.ClearDamagedTargets();
            }
        }
    }

    public override void Movement()
    {
        base.Movement();
        if (ClampSpeed)
        {
            Speed = Mathf.Clamp(Speed, SpeedLimit.x, SpeedLimit.y);
        }
    }

    private void HitDamageable(Health target)
    {
        if (_baseWeapon != null)
        {
            _baseWeapon.OnProjectileHit();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_damageOnTouch) _damageOnTouch.HitDamageableEvent.AddListener(HitDamageable);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_damageOnTouch) _damageOnTouch.HitDamageableEvent.RemoveListener(HitDamageable);
    }



}

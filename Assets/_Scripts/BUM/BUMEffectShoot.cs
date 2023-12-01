using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMEffectShoot : BaseUpgradeMonoEffect
{
    public enum UseType { UseWeapon, SpawnProjectile }

    public BaseWeapon WeaponPrefab;
    public UseType Type;
    public int ProjectileSpawnCount;
    protected BaseWeaponHandler _wuXiaWeaponHandler;
    protected BaseWeapon _weaponInstance;

    protected int _useTimes = 0;
    protected float _usedFrame;

    void Awake()
    {
        _wuXiaWeaponHandler = GetComponentInParent<BaseWeaponHandler>();
    }

    void Start()
    {
        _weaponInstance = _wuXiaWeaponHandler.AddWeapon(WeaponPrefab);
    }

    void Update()
    {
        if (_useTimes > 0 && Time.time > _usedFrame + 0.1f)
        {
            _usedFrame = Time.time;
            _useTimes--;
            _wuXiaWeaponHandler.StartShooting(_weaponInstance);
        }
    }

    public override void Execute(Transform target)
    {
        if (Type == UseType.UseWeapon)
        {
            _useTimes++;
        }
        else
        {
            for (int i = 0; i < ProjectileSpawnCount; i++)
            {
                _weaponInstance.SpawnProjectileAtTargetPosition(target.position, i, ProjectileSpawnCount);
            }
        }
    }

    private void OnDestroy()
    {
        if (_weaponInstance != null)
        {
            _wuXiaWeaponHandler.RemoveWeapon(_weaponInstance);
            _weaponInstance = null;
        }
    }
}

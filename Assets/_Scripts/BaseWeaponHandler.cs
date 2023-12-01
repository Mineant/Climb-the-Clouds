using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BaseWeaponHandler : MonoBehaviour
{
    public Transform WeaponAttachment;
    public List<BaseWeapon> WeaponInstances;

    public BaseWeapon AddWeapon(BaseWeapon weapon)
    {
        var instance = Instantiate(weapon, WeaponAttachment);
        instance.transform.localPosition = Vector3.zero;
        InitializeWeapon(instance);
        WeaponInstances.Add(instance);

        return instance;
    }

    public bool RemoveWeapon(BaseWeapon weaponInstance)
    {
        if (WeaponInstances.Remove(weaponInstance))
        {
            Destroy(weaponInstance.gameObject);
            return true;
        }

        return false;
    }

    public BaseWeapon AddSceneWeapon(BaseWeapon weaponInstance)
    {
        weaponInstance.transform.parent = WeaponAttachment;
        weaponInstance.transform.localPosition = Vector3.zero;
        InitializeWeapon(weaponInstance);
        return weaponInstance;
    }

    /// <summary>
    /// Sets the weapon owner.
    /// </summary>
    public void InitializeWeapon(BaseWeapon instance)
    {
        instance.SetOwner(GetComponent<Character>(), null);
        instance.Initialization();
    }

    public virtual void StartShooting(BaseWeapon weapon)
    {
        if (weapon == null) Debug.LogError("Weapon Is null, cannot shoot.");
        if (!weapon.Initialized) Debug.LogError("Weapon is not initialized. May still be a prefab.");

        if (weapon is WuXiaWeapon wuXiaWeapon)
            wuXiaWeapon.WuXiaWeapoonInputStart(false);
        else
            weapon.WeaponInputStart();
    }

    public virtual void StopShooting(BaseWeapon weapon)
    {
        if (weapon == null) Debug.LogError("Weapon Is null, cannot shoot.");
        if (!weapon.Initialized) Debug.LogError("Weapon is not initialized. May still be a prefab.");

        weapon.WeaponInputStop();
    }

    public virtual void StopAllShooting()
    {
        foreach (var weaponInstance in WeaponInstances)
        {
            weaponInstance.TurnWeaponOff();
        }
    }
}

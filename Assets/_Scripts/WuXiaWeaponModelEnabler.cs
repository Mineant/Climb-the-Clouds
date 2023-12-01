using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class WuXiaWeaponModelEnabler : TopDownMonoBehaviour
{
    public WeaponModelBindings[] Bindings;

    protected CharacterWuXiaCombat _characterWuXiaCombat;

    void Awake()
    {
        _characterWuXiaCombat = GetComponent<CharacterWuXiaCombat>();
    }
    protected virtual void Update()
    {
        if (Bindings.Length <= 0)
        {
            return;
        }

        if (_characterWuXiaCombat.CurrentWeapon == null)
        {
            return;
        }

        foreach (WeaponModelBindings binding in Bindings)
        {
            if (binding.WeaponAnimationID == _characterWuXiaCombat.CurrentWeapon.WeaponAnimationID)
            {
                binding.WeaponModel.SetActive(true);
            }
            else
            {
                binding.WeaponModel.SetActive(false);
            }
        }
    }
}

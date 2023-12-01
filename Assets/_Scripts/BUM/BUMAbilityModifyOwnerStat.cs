using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMAbilityModifyOwnerStat : BaseUpgradeMonoAbility
{
    public BUMEffectModifyOwnerStat BUMEffectModifyOwnerStat { get { return GetComponent<BUMEffectModifyOwnerStat>(); } }

    public override string GetDescription()
    {
        return StatHelper.GetAllInOneStatString(BUMEffectModifyOwnerStat.ModifyStats, ", ");
    }
}

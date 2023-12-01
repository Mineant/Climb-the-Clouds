using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;


[CreateAssetMenu]
public class StatModifyItem : BaseInventoryItem
{
    [Header("Stats")]
    public StatDictionaryAllInOne StatModifiers;

    public override bool Pick(string playerID)
    {
        StageManager.Instance.Player.GetComponent<BaseStats>().AddBasicStatModifiers(StatModifiers, this);  // This wont update the max health of TDE.Health on the player.
        StageManager.Instance.Player.GetComponent<Health>().ReceiveHealth(0f, null);    // This will update the max health.

        return base.Pick(playerID);
    }
}

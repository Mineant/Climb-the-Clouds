using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

[CreateAssetMenu]

public class HealItem : BaseInventoryItem
{
    [Header("Heal")]
    public float HealAmount;

    public override bool Pick(string playerID)
    {
        StageManager.Instance.Player.GetComponent<Health>().ReceiveHealth(HealAmount, null);

        return base.Pick(playerID);
    }
}

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;


[CreateAssetMenu]
public class BaseUpgradeMonoIdentity : ScriptableObject
{
    // Used for identifying different Base Upgrade Monos
}

public enum BaseUpgradeMonoEventType
{
    Add,
    Remove,
    Used,
}

public struct BaseUpgradeMonoEvent
{
    public BaseUpgradeMonoIdentity Identity;
    public BaseUpgradeMonoEventType EventType;

    public BaseUpgradeMonoEvent(BaseUpgradeMonoIdentity identity, BaseUpgradeMonoEventType eventType)
    {
        Identity = identity;
        EventType = eventType;
    }

    static BaseUpgradeMonoEvent e;

    public static void Trigger(BaseUpgradeMonoIdentity identity, BaseUpgradeMonoEventType eventType)
    {
        e.Identity = identity;
        e.EventType = eventType;

        MMEventManager.TriggerEvent(e);
    }
}

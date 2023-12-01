using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class MyTriggerAndCollision : MMTriggerAndCollision
{
    protected override void OnTriggerEnter(Collider collider)
    {
        if (TriggerLayerMask.MMContains(collider.gameObject))
        {
            ReliableOnTriggerExit.NotifyTriggerEnter(collider, gameObject, OnTriggerExit);
            Target = collider.gameObject;
            OnTriggerEnterEvent.Invoke();
        }
    }

    protected override void OnTriggerExit(Collider collider)
    {
        if (TriggerLayerMask.MMContains(collider.gameObject))
        {
            ReliableOnTriggerExit.NotifyTriggerExit(collider, gameObject);
            Target = collider.gameObject;
            OnTriggerExitEvent.Invoke();
        }
    }


}

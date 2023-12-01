using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTriggerer : MonoBehaviour
{
    public GameEventType TargetGameEventType;
    public void TriggerGameEvent()
    {
        GameEvent.Trigger(TargetGameEventType);
    }
}

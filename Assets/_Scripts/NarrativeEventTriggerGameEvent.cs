using System.Collections;
using System.Collections.Generic;
using System.Resources;
using DialogueQuests;
using MoreMountains.Tools;
using UnityEngine;

public class NarrativeEventTriggerGameEvent : MonoBehaviour, MMEventListener<GameEvent>
{
    public GameEventType TargetGameEventType;
    
    public void OnMMEvent(GameEvent eventType)
    {
        if (eventType.GameEventType == TargetGameEventType)
        {
            GetComponent<NarrativeEvent>().TriggerIfConditionsMet(Actor.GetPlayerActor());
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<GameEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<GameEvent>();
    }

}

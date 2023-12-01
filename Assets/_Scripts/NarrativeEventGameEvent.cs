using System.Collections;
using System.Collections.Generic;
using DialogueQuests;
using UnityEngine;

[CreateAssetMenu]
public class NarrativeEventGameEvent : CustomEffect
{
    public GameEventType GameEventType;

    public override void DoEffect(Actor player)
    {
        base.DoEffect(player);
        GameEvent.Trigger(GameEventType);
    }
}

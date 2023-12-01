using System.Collections;
using System.Collections.Generic;
using DialogueQuests;
using UnityEngine;

    [CreateAssetMenu]
public class NarrativeEffectIncreaseConversationTime : CustomEffect
{
    public ConversationID ID;

    public override void DoEffect(Actor player)
    {
        base.DoEffect(player);

        ConversationManager.IncreaseConversationTime(ID);
    }
}

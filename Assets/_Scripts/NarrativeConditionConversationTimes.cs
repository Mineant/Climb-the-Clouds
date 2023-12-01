using System.Collections;
using System.Collections.Generic;
using DialogueQuests;
using UnityEngine;
[CreateAssetMenu]
public class NarrativeConditionConversationTimes : CustomCondition
{
    public ConversationID ID;
    public int Target;
    public Helpers.ComparasonType ComparasonType = Helpers.ComparasonType.EqualTo;

    public override void Start()
    {
        base.Start();
    }

    public override bool IsMet(Actor player)
    {
        return Helpers.Compare(ComparasonType, ConversationManager.GetConversationTime(ID), Target);
    }
}

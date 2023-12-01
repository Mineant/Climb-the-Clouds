using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConversationManager
{
    public static int GetConversationTime(ConversationID id)
    {
        return PlayerPrefs.GetInt(id.ToString(), 0);
    }
    
    public static void IncreaseConversationTime(ConversationID id)
    {
        int times = GetConversationTime(id);
        PlayerPrefs.SetInt(id.ToString(), times + 1);
    }
}

public enum ConversationID
{
    PlayerGameStart,
    PlayerDie,
    SnakeNagaBeforeBattle,
    TrainingDummyBeforeBattle,
    ClanHeadTopOfMountain,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIContainer : Container<SkillUIProduct, SkillUIProductArgs>
{
    public string PlayerID;
    public bool MatchPlayerID(string playerID)
    {
        return PlayerID == playerID;
    }

    
}

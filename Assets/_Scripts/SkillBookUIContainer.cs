using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBookUIContainer : Container<SkillBookUIProduct, SkillBookUIProductArgs>, IPlayerID
{
    public string PlayerID;
    public bool MatchPlayerID(string playerID)
    {
        return PlayerID == playerID;
    }

}

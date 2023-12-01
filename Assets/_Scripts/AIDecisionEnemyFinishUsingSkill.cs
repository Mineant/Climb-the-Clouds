using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionEnemyFinishUsingSkill : AIDecision
{
    public bool FinishedUsingSomeSkill;
    protected EnemySkillManager _enemySkillManager;

    protected override void Awake()
    {
        base.Awake();
        _enemySkillManager = GetComponentInChildren<EnemySkillManager>();
        _enemySkillManager.OnSomeSkillIsFiniished += () => { FinishedUsingSomeSkill = true; };
    }

    public override bool Decide()
    {
        if (FinishedUsingSomeSkill)
        {
            FinishedUsingSomeSkill = false;
            return true;
        }
        return false;
    }
}

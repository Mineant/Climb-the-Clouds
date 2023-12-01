using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIActionEnemySkillManagerUseSkill : AIAction
{
    protected EnemySkillManager _enemySkillManager;
    protected override void Awake()
    {
        base.Awake();
        _enemySkillManager = GetComponentInChildren<EnemySkillManager>();
    }

    public override void PerformAction()
    {
        _enemySkillManager.UseASkill();
    }
}

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIDecisionHaveEnemySkill : AIDecision
{
    protected EnemySkillManager _enemySkillManager;
    protected Character _character;
    protected override void Awake()
    {
        base.Awake();
        _enemySkillManager = GetComponentInChildren<EnemySkillManager>();
        _character = GetComponentInParent<Character>();
    }

    public override bool Decide()
    {
        return _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal && _enemySkillManager.CanUseEnemySkills();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;

public class EnemySkillManager : MonoBehaviour
{
    protected List<EnemySkill> _enemySkills;
    public Action OnSomeSkillIsFiniished;
    protected bool _usingSkill;

    void Awake()
    {
        _usingSkill = false;
        _enemySkills = GetComponentsInChildren<EnemySkill>().ToList();
        foreach (var item in _enemySkills)
            item.OnFinishedUsingSkill += OnFinishedUsingSkill;
    }




    public bool CanUseEnemySkills()
    {
        return _enemySkills.Any(s => s.CanUseSkill());
    }

    public void UseASkill()
    {
        if (_usingSkill) return;

        var enemySkill = _enemySkills.Find(s => s.CanUseSkill());
        if (enemySkill == null) return;

        enemySkill.UseSkill();
        _usingSkill = true;
    }

    private void OnFinishedUsingSkill()
    {
        if (!_usingSkill) return;
        StartCoroutine(OnFinishedUsingSkillCoroutine());
    }

    private IEnumerator OnFinishedUsingSkillCoroutine()
    {
        yield return new WaitForEndOfFrame();

        _usingSkill = false;
        if (OnSomeSkillIsFiniished != null) OnSomeSkillIsFiniished.Invoke();
    }
}

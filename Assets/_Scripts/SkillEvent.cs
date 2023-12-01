using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public struct SkillEvent
{
    public SkillEventType EventType;
    public Skill Skill;
    public SkillBook SkillBook;
    public int SkillIndex;
    public WuXiaWeapon Weapon;
    public bool Empowered;

    public SkillEvent(SkillEventType eventType, Skill skill, SkillBook skillBook = null, int skillIndex = -1, WuXiaWeapon weapon = null, bool empowered = false)
    {
        EventType = eventType;
        Skill = skill;
        SkillBook = skillBook;
        SkillIndex = skillIndex;
        Weapon = weapon;
        Empowered = empowered;
    }

    static SkillEvent e;

    public static void Trigger(SkillEvent skillEvent)
    {
        e.EventType = skillEvent.EventType;
        e.Skill = skillEvent.Skill;
        e.SkillBook = skillEvent.SkillBook;
        e.SkillIndex = skillEvent.SkillIndex;
        e.Weapon = skillEvent.Weapon;
        e.Empowered = skillEvent.Empowered;

        MMEventManager.TriggerEvent(e);
    }

    public static void Trigger(SkillEventType eventType, Skill skill, SkillBook skillBook = null, int skillIndex = -1, WuXiaWeapon weapon = null, bool empowered = false)
    {
        e.EventType = eventType;
        e.Skill = skill;
        e.SkillBook = skillBook;
        e.SkillIndex = skillIndex;
        e.Weapon = weapon;
        e.Empowered = empowered;

        MMEventManager.TriggerEvent(e);
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this).ToString();
    }

}

public enum SkillEventType
{
    AddSkill,
    RemoveSkill,
    MoveSkill,
    UseSkill,


}
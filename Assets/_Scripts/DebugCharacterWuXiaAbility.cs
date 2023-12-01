using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugCharacterWuXiaAbility : MonoBehaviour
{

    [Header("Debug")]
    public SkillBook DebugSkillBook;
    public int DebugRemoveBookIndex;
    public Skill DebugAddSkill;
    private Character _character;
    private CharacterWuXiaHandleSkillBook _characterWuXiaHandleSkillBook;

    void Start()
    {
        _character = GetComponentInParent<Character>();
        _characterWuXiaHandleSkillBook = _character.FindAbility<CharacterWuXiaHandleSkillBook>();
    }


    [Button]
    public void DebugAddSkillBook()
    {
        _characterWuXiaHandleSkillBook.EquipSkillBook(DebugSkillBook);
    }

    [Button]
    public void DebugRemoveSkillBook()
    {
        _characterWuXiaHandleSkillBook.UnequipSkillBook(DebugRemoveBookIndex);
    }

    [Button]
    public void DebugAddSkillToSkillBag()
    {
        _characterWuXiaHandleSkillBook.AddSkillToSkillBag(DebugAddSkill);
    }
}

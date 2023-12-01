using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BaseStats))]
public class CharacterWuXiaCombat : CharacterAbility
{
    [Header("WuXia Combat")]
    [ReadOnly]
    public List<WuXiaWeapon> WeaponsCache;

    [Header("WuXia Weapon Feedbacks")]
    public MMF_Player QiNotEnoughFeedback;
    public MMF_Player EmpoweredSuccessFeedback;
    public MMF_Player EmpoweredFailFeedback;
    public MMF_Player AuraSkillUIAnnouceFeedback;

    public bool IsUsingSkill { get { return _currentSkillTime > 0f; } }
    public WuXiaWeapon CurrentWeapon { get; protected set; }
    public WeaponAim CurrentWeapanAim { get { return CurrentWeapon.GetComponent<WeaponAim>(); } }
    public Action OnRecover;
    public Action<SkillEvent> OnUseSkill;
    protected SkillBookEquipment _currentSkillBookEquipment;
    protected CharacterWuXiaHandleSkillBook _wuXiaHandleSkillBook;
    protected CharacterWuXiaResource _wuXiaResource;
    private BaseStats _baseStats;
    private CharacterWuXiaInput _wuXiaInput;
    protected BaseWeaponHandler _weaponHandler;
    protected int _currentSkillIndex = 0;
    protected float _currentSkillTime = 0f;
    protected float _maxSkillTime = 0f;
    protected float _currentRecoveryTime = 0f;
    protected float _maxRecoveryTime = 0f;
    protected float _currentSkillBookResetTime = 0f;
    protected float _maxSkillBookResetTime = 5f;
    // protected BaseUpgradeMono _currentSkillBookSpecialEffect;
    protected float _lastSkillAuraRegen = 0f;


    protected override void PreInitialization()
    {
        base.PreInitialization();

        _weaponHandler = GetComponent<BaseWeaponHandler>();
        _wuXiaHandleSkillBook = GetComponent<CharacterWuXiaHandleSkillBook>();
        _wuXiaResource = GetComponent<CharacterWuXiaResource>();
        _baseStats = GetComponent<BaseStats>();
        _wuXiaInput = GetComponent<CharacterWuXiaInput>();
        CurrentWeapon = null;
    }


    protected override void Initialization()
    {
        base.Initialization();

        UpdateQiBarStats();
        _wuXiaResource.UpdateQiBarUI();
        _wuXiaResource.UpdateAuraBarUI();
    }


    private void UpdateQiBarStats()
    {
        _wuXiaResource.QiBar.SetMaxValue(_baseStats.GetStat(Stat.MaxQi));
        _wuXiaResource.QiBar.SetRegenerationRate(_baseStats.GetStat(Stat.QiRegen));
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (_currentSkillTime > 0) _currentSkillTime -= Time.deltaTime;
        if (_currentRecoveryTime > 0)
        {
            _currentRecoveryTime -= Time.deltaTime;
            if (_currentRecoveryTime <= 0 && OnRecover != null) OnRecover.Invoke();
        }
        if (_currentSkillBookEquipment != null && _currentSkillIndex > 0 && _currentSkillTime <= 0 && _currentRecoveryTime <= 0)
        {
            if (_currentSkillBookResetTime <= 0)
            {
                _currentSkillBookResetTime = _maxSkillBookResetTime;
            }
            else if (_currentSkillBookResetTime > 0)
            {
                _currentSkillBookResetTime -= Time.deltaTime;

                if (_currentSkillBookResetTime <= 0)
                {
                    EnterSkillBookCooldown();
                }
            }
        }

        if (UIManager.HasInstance)
        {
            UIManager.Instance.SetSkillTimeBar(Mathf.Max(_currentSkillTime, 0f), _maxSkillTime, _character.PlayerID);
            UIManager.Instance.SetRecoveryTimeBar(Mathf.Max(_currentRecoveryTime, 0f), _maxRecoveryTime, _character.PlayerID);
            UIManager.Instance.SetSkillBookResetTimeBar(Mathf.Max(_currentSkillBookResetTime, 0f), _maxSkillBookResetTime, _character.PlayerID);
        }
    }

    public void UpdateUI()
    {
        foreach (var skillBook in _wuXiaHandleSkillBook.EquippedSkillBooks)
        {
            SkillBookEvent.Trigger(SkillBookEventType.HighlightedSkillBookSlot, skillBook.TargetInventory.name, null, 0, 0, -1);
        }

        SkillBookEvent.Trigger(SkillBookEventType.HighlightedSkillBookSlot, _currentSkillBookEquipment.TargetInventory.name, null, 0, 0, 0);
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        for (int i = 0; i < _wuXiaHandleSkillBook.EquippedSkillBooks.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeSkillBookEquipment(i);
                break;
            }
        }

        if (_wuXiaInput.GetNormalAttackButtonDown())
        {
            if (UseSkillBookEquipment(false))
            {
                _wuXiaInput.ResetBuffer();
            }
        }
        if (_wuXiaInput.GetAuraAttackButtonDown())
        {
            if (UseSkillBookEquipment(true))
            {
                _wuXiaInput.ResetBuffer();
            }
        }
    }


    public void ChangeCurrentSkillIndex(int index)
    {
        _currentSkillIndex = index;
        if (_currentSkillBookEquipment != null && _currentSkillBookEquipment.Skills.IndexWithinRange(index))
        {
            int numberOfSkill = 0;
            int skillInventoryIndex = -1;
            for (int i = 0; i < _currentSkillBookEquipment.TargetInventory.Content.Length; i++)
            {
                if (_currentSkillBookEquipment.TargetInventory.Content[i] != null)
                {
                    if (numberOfSkill == index)
                    {
                        skillInventoryIndex = i;
                        break;
                    }
                    else
                    {
                        numberOfSkill += 1;
                    }
                }
            }

            if (skillInventoryIndex == -1) Debug.LogError("The above calculation is wronged.");

            SkillBookEvent.Trigger(SkillBookEventType.HighlightedSkillBookSlot, _currentSkillBookEquipment.TargetInventory.name, null, 0, 0, skillInventoryIndex);
        }
    }



    /// <summary>
    /// Unequip if index == -1.
    /// </summary>
    /// <param name="index"></param>
    public void ChangeSkillBookEquipment(int index)
    {
        if (index == -1 && _currentSkillBookEquipment != null)
        {
            _baseStats.RemoveAllModifiersFromSource(_currentSkillBookEquipment);
            _currentSkillBookEquipment.RemoveSpecialEffect();
            _currentSkillBookEquipment = null;
            return;
        }

        if (!_wuXiaHandleSkillBook.EquippedSkillBooks.IndexWithinRange(index)) return;
        if (_currentSkillBookEquipment == _wuXiaHandleSkillBook.EquippedSkillBooks[index]) return;

        if (_currentSkillBookEquipment != null)
        {
            _baseStats.RemoveAllModifiersFromSource(_currentSkillBookEquipment);
            _currentSkillBookEquipment.RemoveSpecialEffect();
        }

        _currentSkillBookEquipment = _wuXiaHandleSkillBook.EquippedSkillBooks[index];
        _baseStats.AddBasicStatModifiers(_currentSkillBookEquipment.TargetSkillBook.BasicStats.StatDictionaryAllInOne, _currentSkillBookEquipment);
        _currentSkillBookEquipment.AddSpecialEffect();

        UpdateUI();
        ChangeCurrentSkillIndex(0);
        UpdateQiBarStats();
    }


    private void EnterSkillBookCooldown()
    {
        _maxRecoveryTime = _baseStats.GetStat(Stat.RecoveryTime); // TEMP, get stat
        if (_currentSkillBookEquipment != null)
        {
            float sum = 0f;
            foreach (var skill in _currentSkillBookEquipment.Skills)
            {
                if (skill.WuXiaWeapon.GetComponent<BaseStats>().BasicStats.StatDictionaryAllInOne.BaseValueStatDictionary.TryGetValue(Stat.RecoveryTime, out float cooldown)) sum += cooldown;
            }

            _maxRecoveryTime += sum;
        }

        _currentRecoveryTime = _maxRecoveryTime;
        ChangeCurrentSkillIndex(0);
    }

    public bool UseSkillBookEquipment(bool empowered)
    {
        if (_currentSkillBookEquipment == null) return false;
        if (_currentSkillTime > 0f) return false;
        if (_currentRecoveryTime > 0f) return false;
        if (_currentSkillBookEquipment.Skills.Count == 0) return false;

        // Find the skill to use
        Skill skill = _currentSkillBookEquipment.Skills[_currentSkillIndex];
        WuXiaWeapon weapon = GetWeapon(skill);

        if (!CheckUseSkillWithResource(weapon.BaseStats, empowered)) return false;

        SkillEvent skillEvent = new(SkillEventType.UseSkill, skill, _currentSkillBookEquipment.TargetSkillBook, _currentSkillIndex, weapon, empowered);
        SkillEvent.Trigger(skillEvent);
        if (OnUseSkill != null) OnUseSkill.Invoke(skillEvent);

        CurrentWeapon = weapon;
        _maxSkillTime = weapon.BaseStats.GetStat(Stat.SkillTime);
        _currentSkillTime = _maxSkillTime;
        ChangeCurrentSkillIndex(_currentSkillIndex + 1);
        if (_currentSkillIndex >= _currentSkillBookEquipment.Skills.Count)
        {
            EnterSkillBookCooldown();
        }

        if (empowered)
        {
            AuraSkillUIAnnouceFeedback.GetFeedbackOfType<MMF_TMPText>().NewText = skill.ItemName;
            AuraSkillUIAnnouceFeedback.GetFeedbackOfType<MMF_MMSoundManagerSound>().Sfx = skill.AuraSkillAudio;
            if (AuraSkillUIAnnouceFeedback.IsPlaying) AuraSkillUIAnnouceFeedback.StopFeedbacks();
            AuraSkillUIAnnouceFeedback.PlayFeedbacks();
        }

        weapon.WuXiaWeapoonInputStart(empowered);
        UpdateAuraBarHighlight();
        
        return true;
    }

    private WuXiaWeapon GetWeapon(Skill skill)
    {
        // Find a weapon that matches the weapon id
        WuXiaWeapon weapon = WeaponsCache.FirstOrDefault(weapon => weapon.WeaponName == skill.WuXiaWeapon.WeaponName && weapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle);

        // if cannot find one that is idle, spawn one, 
        if (weapon == null)
        {
            weapon = (WuXiaWeapon)_weaponHandler.AddWeapon(skill.WuXiaWeapon);
            WeaponsCache.Add(weapon);
        }

        return weapon;
    }


    protected bool CheckUseSkillWithResource(BaseStats weaponBaseStat, bool empowered)
    {
        float qiRequirement = Math.Max(weaponBaseStat.GetStat(Stat.QiCost), 0);
        int empowerAuraCellRequirement = (int)weaponBaseStat.GetStat(Stat.AuraSkillCost);
        float auraRegen = weaponBaseStat.GetStat(Stat.AuraRegen);

        if (_wuXiaResource.QiBar.CurrentValue < qiRequirement)
        {
            PlayQiNotEnoughFeedback();
            return false; // Cannot use weapon
        }

        if (empowered)
        {
            if (_wuXiaResource.GetAuraCell() >= empowerAuraCellRequirement)
            {
                PlayEmpoweredSuccessFeedback();
            }
            else
            {
                PlayEmpoweredFailFeedback();
                return false; // Cannot use empowered attack
            }
        }

        _wuXiaResource.QiBar.ChangeValue(-qiRequirement);
        if (!empowered) _lastSkillAuraRegen = auraRegen;
        if (empowered)
        {
            _wuXiaResource.ChangeAuraCell(-empowerAuraCellRequirement);
            _wuXiaResource.AuraBar.ChangeValue(1f);
        }

        return true;
    }

    public void PlayQiNotEnoughFeedback()
    {
        QiNotEnoughFeedback?.PlayFeedbacks();
        UIManager.Instance.ShowWarning("Not enough Energy");
    }

    public void PlayEmpoweredSuccessFeedback()
    {
        EmpoweredSuccessFeedback?.PlayFeedbacks();
    }

    public void PlayEmpoweredFailFeedback()
    {
        EmpoweredFailFeedback?.PlayFeedbacks();
        UIManager.Instance.ShowWarning("Not enough Aura");
    }


    private void OnSkillBookEquipmentsChanged()
    {
        bool isChanged = false;

        // Check if empty
        if (_wuXiaHandleSkillBook.EquippedSkillBooks.Count == 0)
        {
            ChangeSkillBookEquipment(-1);
            // _currentSkillBookEquipment = null;
            isChanged = true;
        }

        // Check if skill book is still within
        if (!_wuXiaHandleSkillBook.EquippedSkillBooks.Contains(_currentSkillBookEquipment))
        {
            ChangeSkillBookEquipment(0);
            isChanged = true;
        }

        if (isChanged)
        {
            ChangeCurrentSkillIndex(0);
        }
    }

    private void OnSkillBookEquipmentContentChanged(SkillBookEquipment equipment)
    {
        if (_currentSkillBookEquipment == equipment && _currentSkillIndex > 0 && _currentRecoveryTime == 0)
        {
            EnterSkillBookCooldown();
        }
    }


    public void OnProjectileHit()
    {
        if (_lastSkillAuraRegen > 0f)
        {
            _wuXiaResource.AuraBar.ChangeValue(_lastSkillAuraRegen);
            _lastSkillAuraRegen = 0f;
            UpdateAuraBarHighlight();
        }
    }

    protected void UpdateAuraBarHighlight()
    {
        // See if next skill can be empowered
        if (UIManager.HasInstance)
        {
            int empowerAuraCellRequirement = (int)GetWeapon(_currentSkillBookEquipment.Skills[_currentSkillIndex]).BaseStats.GetStat(Stat.AuraSkillCost);
            if (_wuXiaResource.GetAuraCell() >= empowerAuraCellRequirement)
            {
                UIManager.Instance.HighlightPlayerAuraBar(empowerAuraCellRequirement);
            }
            else
            {
                UIManager.Instance.HighlightPlayerAuraBar(0);
            }
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        _wuXiaHandleSkillBook.OnSkillBookEquipmentsChanged += OnSkillBookEquipmentsChanged;
        _wuXiaHandleSkillBook.OnSkillBookEquipmentContentChanged += OnSkillBookEquipmentContentChanged;
    }



    protected override void OnDisable()
    {
        base.OnDisable();
        _wuXiaHandleSkillBook.OnSkillBookEquipmentsChanged -= OnSkillBookEquipmentsChanged;
        _wuXiaHandleSkillBook.OnSkillBookEquipmentContentChanged -= OnSkillBookEquipmentContentChanged;
    }


}

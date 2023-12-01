using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using Kryz.CharacterStats;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseHealth : Health
{
    [Header("Base")]
    [Tooltip("Normally, will automatically character component from parent. Enabling this will only get the character from this object. This is to prevent nesting health objects inside a character, like nesting a projectile inside a character that might die, and trigger the character's death method.")]
    public bool GetComponentsInSelf;

    [MMInspectorGroup("Damage Numbers", true, 15)]
    public bool ShowDamageNumbers;
    public DamageNumberType DamageTakenDM = DamageNumberType.Damage;
    public DamageNumberType HealDM = DamageNumberType.Heal;
    protected BaseStats _baseStats;

    public override void Initialization()
    {
        _character = this.GetComponentOrInParent<Character>(!GetComponentsInSelf);
        _baseStats = this.GetComponent<BaseStats>();

        if (Model != null)
        {
            Model.SetActive(true);
        }

        if (this.GetComponentOrInParent<Renderer>(!GetComponentsInSelf) != null)
        {
            _renderer = this.GetComponentOrInParent<Renderer>(!GetComponentsInSelf);
        }
        if (_character != null)
        {
            _characterMovement = _character.FindAbility<CharacterMovement>();
            if (_character.CharacterModel != null)
            {
                if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
                {
                    _renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();
                }
            }
        }
        if (_renderer != null)
        {
            if (UseMaterialPropertyBlocks && (_propertyBlock == null))
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            if (ResetColorOnRevive)
            {
                if (UseMaterialPropertyBlocks)
                {
                    if (_renderer.sharedMaterial.HasProperty(ColorMaterialPropertyName))
                    {
                        _hasColorProperty = true;
                        _initialColor = _renderer.sharedMaterial.GetColor(ColorMaterialPropertyName);
                    }
                }
                else
                {
                    if (_renderer.material.HasProperty(ColorMaterialPropertyName))
                    {
                        _hasColorProperty = true;
                        _initialColor = _renderer.material.GetColor(ColorMaterialPropertyName);
                    }
                }
            }
        }

        _interruptiblesDamageOverTimeCoroutines = new List<InterruptiblesDamageOverTimeCoroutine>();
        _damageOverTimeCoroutines = new List<InterruptiblesDamageOverTimeCoroutine>();
        _initialLayer = gameObject.layer;

        _autoRespawn = this.GetComponentOrInParent<AutoRespawn>(!GetComponentsInSelf);
        _healthBar = this.GetComponentOrInParent<MMHealthBar>(!GetComponentsInSelf);
        _controller = this.GetComponentOrInParent<TopDownController>(!GetComponentsInSelf);
        _characterController = this.GetComponentOrInParent<CharacterController>(!GetComponentsInSelf);
        _collider2D = this.GetComponentOrInParent<Collider2D>(!GetComponentsInSelf);
        _collider3D = this.GetComponentOrInParent<Collider>(!GetComponentsInSelf);

        DamageMMFeedbacks?.Initialization(this.gameObject);
        DeathMMFeedbacks?.Initialization(this.gameObject);

        StoreInitialPosition();
        _initialized = true;

        DamageEnabled();
    }

    protected override void Start()
    {
        base.Start();

        ////////// MINE //////////
        ResetHealthToMaxHealth();
        InitialHealth = MaximumHealth;
        ////////// END //////////
    }

    public override float Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        float damageTaken = base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);

        if (damageTaken > 0)
        {
            if (_baseStats != null)
            {
                _baseStats.SetBaseValue(Stat.BaseDamageReceived, damageTaken);
                damageTaken = _baseStats.GetStat(Stat.BaseDamageReceived);
            }

            if (ShowDamageNumbers) DamageNumberManager.Instance.GenerateDamageNumber(DamageTakenDM, this.transform.position, damageTaken);
        }

        return damageTaken;
    }

    public override void ReceiveHealth(float health, GameObject instigator)
    {
        ////////// MINE //////////
        if (_baseStats)
        {
            float maxHealth = _baseStats.GetStat(Stat.Health);
            if (maxHealth > MaximumHealth) health += maxHealth - MaximumHealth; // Increase the healing if max health increase
            MaximumHealth = maxHealth;
        }
        ////////// END //////////

        // this function adds health to the character's Health and prevents it to go above MaxHealth.
        if (MasterHealth != null)
        {
            MasterHealth.SetHealth(Mathf.Min(CurrentHealth + health, MaximumHealth));
        }
        else
        {
            SetHealth(Mathf.Min(CurrentHealth + health, MaximumHealth));
        }
        UpdateHealthBar(true);
    }

    /// <summary>
    /// Resets the character's health to its max value
    /// </summary>
    public override void ResetHealthToMaxHealth()
    {
        ////////// MINE //////////
        if (_baseStats) MaximumHealth = _baseStats.GetStat(Stat.Health);
        ////////// END //////////

        SetHealth(MaximumHealth);
    }


    public override void UpdateHealthBar(bool show)
    {
        base.UpdateHealthBar(show);

        if (_character != null && _character.CharacterType == Character.CharacterTypes.Player && UIManager.HasInstance) UIManager.Instance.SetHealthBar(CurrentHealth, MaximumHealth, _character.PlayerID);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        UpdateHealthBar(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

}

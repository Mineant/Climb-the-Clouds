using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.VisualScripting;

public class BaseWeapon : Weapon, MMEventListener<TopDownEngineEvent>
{
    [MMInspectorGroup("Projectiles", true, 22)]
    /// the offset position at which the projectile will spawn
    [Tooltip("the offset position at which the projectile will spawn")]
    public Vector3 ProjectileSpawnOffset = Vector3.zero;
    /// in the absence of a character owner, the default direction of the projectiles
    [Tooltip("in the absence of a character owner, the default direction of the projectiles")]
    public Vector3 DefaultProjectileDirection = Vector3.forward;
    /// the number of projectiles to spawn per shot
    [Tooltip("the number of projectiles to spawn per shot")]
    public int ProjectilesPerShot = 1;

    [Header("Spawn Transforms")]
    /// a list of transforms that can be used a spawn points, instead of the ProjectileSpawnOffset. Will be ignored if left emtpy
    [Tooltip("a list of transforms that can be used a spawn points, instead of the ProjectileSpawnOffset. Will be ignored if left emtpy")]
    public List<Transform> SpawnTransforms = new List<Transform>();
    /// a list of modes the spawn transforms can operate on
    public enum SpawnTransformsModes { Random, Sequential }
    /// the selected mode for spawn transforms. Sequential will go through the list sequentially, while Random will pick a random one every shot
    [Tooltip("the selected mode for spawn transforms. Sequential will go through the list sequentially, while Random will pick a random one every shot")]
    public SpawnTransformsModes SpawnTransformsMode = SpawnTransformsModes.Sequential;

    [Header("Spread")]
    /// the spread (in degrees) to apply randomly (or not) on each angle when spawning a projectile
    [Tooltip("the spread (in degrees) to apply randomly (or not) on each angle when spawning a projectile")]
    public Vector3 Spread = Vector3.zero;
    /// whether or not the weapon should rotate to align with the spread angle
    [Tooltip("whether or not the weapon should rotate to align with the spread angle")]
    public bool RotateWeaponOnSpread = false;
    /// whether or not the spread should be random (if not it'll be equally distributed)
    [Tooltip("whether or not the spread should be random (if not it'll be equally distributed)")]
    public bool RandomSpread = true;
    /// the projectile's spawn position
    [MMReadOnly]
    [Tooltip("the projectile's spawn position")]
    public Vector3 SpawnPosition = Vector3.zero;

    /// the object pooler used to spawn projectiles, if left empty, this component will try to find one on its game object
    [Tooltip("the object pooler used to spawn projectiles, if left empty, this component will try to find one on its game object")]
    public MMObjectPooler ObjectPooler;

    [Header("Spawn Feedbacks")]
    public List<MMFeedbacks> SpawnFeedbacks = new List<MMFeedbacks>();

    [MMInspectorGroup("Animation Triggers", true, 23)]
    public string StartAnimTrigger;

    [MMInspectorGroup("Base", true, 24)]
    public BaseStats BaseStats;
    public bool AdvancedPreventMovementWhileInUse;
    public bool SetMovementWhenUsed;
    protected Vector3 _flippedProjectileSpawnOffset;
    protected Vector3 _randomSpreadDirection;
    protected bool _poolInitialized = false;
    protected Transform _projectileSpawnTransform;
    protected int _spawnArrayIndex = 0;
    protected DamageThingContainer _damageCacheThing;
    protected Dictionary<DamageType, float> _typedDamageCacheDictionary = new();
    protected List<StatusEffectField> _oneTimeDamageStatusEffects;
    protected Vector3 _projectileSize;
    public bool Initialized { get; protected set; }

    [MMInspectorButton("TestShoot")]
    /// a button to test the shoot method
    public bool TestShootButton;

    protected bool _addedOneTimeStatModifiers;
    public const string ONE_TIME_STAT_ID = "THISISTHEONETIMESTATIDWHICHWILLNEVERBEANYSAME";

    /// <summary>
    /// A test method that triggers the weapon
    /// </summary>
    protected virtual void TestShoot()
    {
        if (WeaponState.CurrentState == WeaponStates.WeaponIdle)
        {
            WeaponInputStart();
        }
        else
        {
            WeaponInputStop();
        }
    }

    protected virtual void Awake()
    {
        _weaponAim = GetComponent<WeaponAim>();
        BaseStats = GetComponent<BaseStats>();
        _oneTimeDamageStatusEffects = new();
    }

    /// <summary>
    /// Initialize this weapon
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();


        if (!_poolInitialized)
        {
            if (ObjectPooler == null)
            {
                ObjectPooler = GetComponent<MMObjectPooler>();
            }
            if (ObjectPooler == null)
            {
                Debug.LogWarning(this.name + " : no object pooler (simple or multiple) is attached to this Projectile Weapon, it won't be able to shoot anything.");
                return;
            }
            if (FlipWeaponOnCharacterFlip)
            {
                _flippedProjectileSpawnOffset = ProjectileSpawnOffset;
                _flippedProjectileSpawnOffset.y = -_flippedProjectileSpawnOffset.y;
            }
            _poolInitialized = true;
        }

        Initialized = true;
    }

    public override void SetOwner(Character newOwner, CharacterHandleWeapon handleWeapon)
    {
        Owner = newOwner;
        if (Owner != null)
        {
            CharacterHandleWeapon = handleWeapon;
            _characterMovement = Owner.GetComponent<Character>()?.FindAbility<CharacterMovement>();
            _controller = Owner.GetComponent<TopDownController>();

            _controllerIs3D = Owner.GetComponent<TopDownController3D>() != null;

            // if (CharacterHandleWeapon != null && CharacterHandleWeapon.AutomaticallyBindAnimator)
            // {
            // if (CharacterHandleWeapon.CharacterAnimator != null)
            // {
            //     _ownerAnimator = CharacterHandleWeapon.CharacterAnimator;
            // }
            // if (_ownerAnimator == null)
            // {
            //     _ownerAnimator = CharacterHandleWeapon.gameObject.GetComponentInParent<Character>().CharacterAnimator;
            // }
            // if (_ownerAnimator == null)
            // {
            //     _ownerAnimator = CharacterHandleWeapon.gameObject.GetComponentInParent<Animator>();
            // }
            // }
            _ownerAnimator = newOwner.CharacterAnimator;
        }

        if (Owner != null)
        {
            if (Owner.TryGetComponent<BaseStats>(out var baseStats)) BaseStats.AddRelatedBaseStats(baseStats);
        }
    }

    protected void TriggerAnimationParameter(string paramName)
    {
        if (_ownerAnimator != null && paramName.Length > 0)
        {
            _ownerAnimator.SetTrigger(paramName);
        }
    }

    public override void CaseWeaponStart()
    {
        base.CaseWeaponStart();
        TriggerAnimationParameter(StartAnimTrigger);
    }

    public override void TurnWeaponOn()
    {
        if (SetMovementWhenUsed)
        {
            _characterMovement.SetMovement(_weaponAim.CurrentAim);
        }
        base.TurnWeaponOn();
        if (AdvancedPreventMovementWhileInUse && _characterMovement is BaseCharacterMovement baseCharacterMovement)
        {
            baseCharacterMovement.ForbiddenMovement(this);
        }
    }

    public override void TurnWeaponOff()
    {
        base.TurnWeaponOff();
        if (AdvancedPreventMovementWhileInUse && _characterMovement is BaseCharacterMovement baseCharacterMovement)
        {
            baseCharacterMovement.UnforbiddenMovement(this);
        }

        if (_addedOneTimeStatModifiers)
        {
            BaseStats.RemoveAllModifiersFromSource(ONE_TIME_STAT_ID);
            _addedOneTimeStatModifiers = false;
        }

        _oneTimeDamageStatusEffects.Clear();
    }

    /// <summary>
    /// Called everytime the weapon is used
    /// </summary>
    public override void WeaponUse()
    {
        //////////// ORIGINAL /////////////
        ApplyRecoil();
        TriggerWeaponUsedFeedback();
        //////////// END ///////////

        DetermineSpawnPosition();
        Spread = Vector3.up * BaseStats.GetStat(Stat.Spread);
        int projectilesPerShot = Math.Max((int)BaseStats.GetStat(Stat.ProjectilesPerShot), 1);


        // Determine the damage here. Move all Get stat cache to Shoot Request Co if too laggy.
        CalculateProjectileDamageAndSize();

        for (int i = 0; i < projectilesPerShot; i++)
        {
            SpawnProjectile(SpawnPosition, i, projectilesPerShot, true);
            PlaySpawnFeedbacks();
        }
    }

    private void CalculateProjectileDamageAndSize()
    {
        _damageCacheThing = new(BaseStats.GetStat(Stat.TrueDamage));
        _typedDamageCacheDictionary.Clear();
        foreach (var statDamageKeyPair in ResourceSystem.Instance.StatDamageTypeTable)
            if (BaseStats.HasStat(statDamageKeyPair.Key))
                _typedDamageCacheDictionary[statDamageKeyPair.Value] = BaseStats.GetStat(statDamageKeyPair.Key);

        _projectileSize = Vector3.one * BaseStats.GetStat(Stat.Range);
    }


    public void SpawnProjectileAtTargetPosition(Vector3 spawnPosition, int projectileIndex, int totalProjectiles)
    {
        CalculateProjectileDamageAndSize();
        SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, true);
    }

    protected override void ApplyRecoil()
    {
        RecoilForce = BaseStats.GetStat(Stat.Recoil);
        base.ApplyRecoil();
    }

    public override IEnumerator ShootRequestCo()
    {
        if (Time.time - _lastShootRequestAt < TimeBetweenUses)
        {
            yield break;
        }

        int remainingShots = UseBurstMode ? Math.Max((int)BaseStats.GetStat(Stat.BurstLength), 1) : 1;

        float extraShotChance = remainingShots % 1;
        if (extraShotChance > 0f && UnityEngine.Random.Range(0f, 1f) < extraShotChance) remainingShots += 1;

        float interval = UseBurstMode ? BurstTimeBetweenShots : 1;

        while (remainingShots > 0)
        {
            ShootRequest();
            _lastShootRequestAt = Time.time;
            remainingShots--;
            yield return MMCoroutine.WaitFor(interval);
        }
    }

    /// <summary>
    /// Spawns a new object and positions/resizes it
    /// </summary>
    public virtual GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
    {
        if (ObjectPooler == null) return null;

        /// we get the next object in the pool and make sure it's not null
        GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

        // mandatory checks
        if (nextGameObject == null) { return null; }
        if (nextGameObject.GetComponent<MMPoolableObject>() == null)
        {
            throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
        }
        // we position the object
        nextGameObject.transform.position = spawnPosition;
        if (_projectileSpawnTransform != null)
        {
            nextGameObject.transform.position = _projectileSpawnTransform.position;
        }
        // we set its direction

        Projectile projectile = nextGameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetWeapon(this);
            if (Owner != null)
            {
                projectile.SetOwner(Owner.gameObject);
            }
        }
        // we activate the object
        nextGameObject.gameObject.SetActive(true);

        if (projectile != null)
        {
            if (RandomSpread)
            {
                _randomSpreadDirection.x = UnityEngine.Random.Range(-Spread.x, Spread.x);
                _randomSpreadDirection.y = UnityEngine.Random.Range(-Spread.y, Spread.y);
                _randomSpreadDirection.z = UnityEngine.Random.Range(-Spread.z, Spread.z);
            }
            else
            {
                if (totalProjectiles > 1)
                {
                    _randomSpreadDirection.x = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.x, Spread.x);
                    _randomSpreadDirection.y = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.y, Spread.y);
                    _randomSpreadDirection.z = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.z, Spread.z);
                }
                else
                {
                    _randomSpreadDirection = Vector3.zero;
                }
            }

            Quaternion spread = Quaternion.Euler(_randomSpreadDirection);

            if (Owner == null)
            {
                projectile.SetDirection(spread * transform.rotation * DefaultProjectileDirection, transform.rotation, true);
            }
            else
            {
                if (Owner.CharacterDimension == Character.CharacterDimensions.Type3D) // if we're in 3D
                {
                    projectile.SetDirection(spread * transform.forward, transform.rotation, true);
                }
                else // if we're in 2D
                {
                    Vector3 newDirection = (spread * transform.right) * (Flipped ? -1 : 1);
                    if (Owner.Orientation2D != null)
                    {
                        projectile.SetDirection(newDirection, spread * transform.rotation, Owner.Orientation2D.IsFacingRight);
                    }
                    else
                    {
                        projectile.SetDirection(newDirection, spread * transform.rotation, true);
                    }
                }
            }

            if (RotateWeaponOnSpread)
            {
                this.transform.rotation = this.transform.rotation * spread;
            }
        }

        ////////// MINE //////////
        if (projectile != null && projectile.TryGetComponent<BaseDamageOnTouch>(out var damageOnTouch))
        {
            // float damageCaused = _baseStats.GetStat(Stat.Damage);
            damageOnTouch.MaxDamageCaused = _damageCacheThing.BaseDamage;
            damageOnTouch.MinDamageCaused = _damageCacheThing.BaseDamage;

            damageOnTouch.TypedDamages.Clear();
            foreach (var cache in _typedDamageCacheDictionary)
            {
                damageOnTouch.TypedDamages.Add(new TypedDamage()
                {
                    AssociatedDamageType = cache.Key,
                    MinDamageCaused = cache.Value,
                    MaxDamageCaused = cache.Value,
                });
            }

            if (_oneTimeDamageStatusEffects.Count > 0)
            {
                foreach (var effect in _oneTimeDamageStatusEffects)
                {
                    damageOnTouch.AddOneTimeDamageStatusEffects(effect);
                }
            }
        }

        if (projectile != null)
        {
            projectile.transform.localScale = _projectileSize;
        }
        ////////// END //////////

        if (triggerObjectActivation)
        {
            if (nextGameObject.GetComponent<MMPoolableObject>() != null)
            {
                nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
            }
        }
        return (nextGameObject);
    }

    /// <summary>
    /// This method is in charge of playing feedbacks on projectile spawn
    /// </summary>
    protected virtual void PlaySpawnFeedbacks()
    {
        if (SpawnFeedbacks.Count > 0)
        {
            SpawnFeedbacks[_spawnArrayIndex]?.PlayFeedbacks();
        }

        _spawnArrayIndex++;
        if (_spawnArrayIndex >= SpawnTransforms.Count)
        {
            _spawnArrayIndex = 0;
        }
    }

    /// <summary>
    /// Sets a forced projectile spawn position
    /// </summary>
    /// <param name="newSpawnTransform"></param>
    public virtual void SetProjectileSpawnTransform(Transform newSpawnTransform)
    {
        _projectileSpawnTransform = newSpawnTransform;
    }

    /// <summary>
    /// Determines the spawn position based on the spawn offset and whether or not the weapon is flipped
    /// </summary>
    public virtual void DetermineSpawnPosition()
    {
        if (Flipped)
        {
            if (FlipWeaponOnCharacterFlip)
            {
                SpawnPosition = this.transform.position - this.transform.rotation * _flippedProjectileSpawnOffset;
            }
            else
            {
                SpawnPosition = this.transform.position - this.transform.rotation * ProjectileSpawnOffset;
            }
        }
        else
        {
            SpawnPosition = this.transform.position + this.transform.rotation * ProjectileSpawnOffset;
        }

        if (WeaponUseTransform != null)
        {
            SpawnPosition = WeaponUseTransform.position;
        }

        if (SpawnTransforms.Count > 0)
        {
            if (SpawnTransformsMode == SpawnTransformsModes.Random)
            {
                _spawnArrayIndex = Random.Range(0, SpawnTransforms.Count);
                SpawnPosition = SpawnTransforms[_spawnArrayIndex].position;
            }
            else
            {
                SpawnPosition = SpawnTransforms[_spawnArrayIndex].position;
            }
        }
    }

    /// <summary>
    /// When the weapon is selected, draws a circle at the spawn's position
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        DetermineSpawnPosition();

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(SpawnPosition, 0.2f);
    }

    public void OnMMEvent(TopDownEngineEvent engineEvent)
    {
        switch (engineEvent.EventType)
        {
            case TopDownEngineEventTypes.LevelStart:
                _poolInitialized = false;
                Initialization();
                break;
        }
    }

    /// <summary>
    /// On enable we start listening for events
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    /// <summary>
    /// On disable we stop listening for events
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<TopDownEngineEvent>();
    }

    ////////// MINE //////////

    public void AddOneTimeStatModifier(StatDictionaryAllInOne statDictionaryAllInOne)
    {
        BaseStats.AddBasicStatModifiers(statDictionaryAllInOne, ONE_TIME_STAT_ID);
        _addedOneTimeStatModifiers = true;
    }

    public void AddOneTimeDamageApplyStatusEffect(StatusEffectField statusEffectField)
    {
        _oneTimeDamageStatusEffects.Add(statusEffectField);
    }

    public virtual void OnProjectileHit()
    {
        // To be implemented.
    }

    ////////// END //////////

}

public class DamageThingContainer
{
    public float BaseDamage;

    public DamageThingContainer(float baseDamage)
    {
        BaseDamage = baseDamage;
    }
}
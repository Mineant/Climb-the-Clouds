using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BaseDamageOnTouch : DamageOnTouch
{
    public enum OnHitEffectModeType { None, Small, Medium, Large, Extreme }

    [System.Serializable]
    public class OnHitEffectAndType { public OnHitEffectModeType Type; public GameObject Effect; }

    [MMInspectorGroup("Targets", true, 3)]
    public OnHitEffectModeType OnHitEffectMode;
    public GameObject CustomOnHitEffect;
    public List<OnHitEffectAndType> OnHitEffectList;
    public bool ApplyHitStun;
    public List<StatusEffectField> ApplyStatusEffects;
    public List<StatusEffectField> OneTimeStatusEffects;

    [MMInspectorGroup("Damage Hhits", true, 4)]
    public float DamageDelay = 0f;
    public bool RefreshDamageTargets;
    public float RefreshInterval;



    protected HashSet<GameObject> _damagedTargets;
    protected float _clearDamageTargetTimestamp;
    protected bool _defaultApplyHitStun;
    protected GameObject _onHitEffect;

    public override void Initialization()
    {
        base.Initialization();
        _damagedTargets = new();
        _defaultApplyHitStun = ApplyHitStun;
        _onHitEffect = null;
        if (CustomOnHitEffect != null) _onHitEffect = CustomOnHitEffect;
        else if (OnHitEffectMode != OnHitEffectModeType.None)
        {
            _onHitEffect = OnHitEffectList.Find(e => e.Type == OnHitEffectMode).Effect;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (RefreshDamageTargets && Time.time > _clearDamageTargetTimestamp + RefreshInterval)
        {
            ClearDamagedTargets();
        }
    }

    public void AddDamagedTarget(GameObject target)
    {
        if (_damagedTargets == null) return;
        _damagedTargets.Add(target);
    }

    public void ClearDamagedTargets()
    {
        if (_damagedTargets == null) return;
        _damagedTargets.Clear();
        _clearDamageTargetTimestamp = Time.time;
    }

    protected override bool EvaluateAvailability(GameObject collider)
    {
        return base.EvaluateAvailability(collider) && !_damagedTargets.Contains(collider);
    }

    protected override void OnCollideWithDamageable(Health health)
    {
        _collidingHealth = health;

        if (health.CanTakeDamageThisFrame())
        {
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            HitDamageableEvent?.Invoke(_colliderHealth);

            // we apply the damage to the thing we've collided with
            float randomDamage =
                UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

            ApplyKnockback(randomDamage, TypedDamages);

            DetermineDamageDirection();

            if (RepeatDamageOverTime)
            {
                _colliderHealth.DamageOverTime(randomDamage, gameObject, InvincibilityDuration,
                    InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats,
                    DamageOverTimeInterruptible, RepeatedDamageType);
            }
            else
            {
                _colliderHealth.Damage(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration,
                    _damageDirection, TypedDamages);
            }

            ////////// MINE //////////
            AddDamagedTarget(health.gameObject);

            if (ApplyHitStun && health.gameObject.activeInHierarchy && health.TryGetComponent<CharacterStun>(out var stun))
            {
                stun.StunFor(0.6f);
            }

            if ((ApplyStatusEffects.Count > 0 || OneTimeStatusEffects.Count > 0) && health.TryGetComponent<StatusEffectHandler>(out var statusEffectHandler))
            {
                foreach (var effect in ApplyStatusEffects) statusEffectHandler.ApplyEffect(effect, Owner);
                foreach (var effect in OneTimeStatusEffects) statusEffectHandler.ApplyEffect(effect, Owner);
            }

            if (_onHitEffect != null)
            {
                var obj = Instantiate(_onHitEffect, health.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
                if (_onHitEffect.TryGetComponent<ParticleSystem>(out var particle))
                {
                    var main = particle.main;
                    main.stopAction = ParticleSystemStopAction.Destroy;
                }
            }
            ////////// END //////////
        }

        // we apply self damage
        if (DamageTakenEveryTime + DamageTakenDamageable > 0 && !_colliderHealth.PreventTakeSelfDamage)
        {
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }
    }



    ////////////// MINE /////////////
    public void AddOneTimeDamageStatusEffects(StatusEffectField field)
    {
        OneTimeStatusEffects.Add(field);
    }

    public void ChangeHitStun(bool apply)
    {
        ApplyHitStun = apply;
    }
    ////////////// END //////////////////

    private IEnumerator _DelayEnableCollider(float delay, Collider collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (DamageDelay > 0f && TryGetComponent<Collider>(out var collider))
            StartCoroutine(_DelayEnableCollider(DamageDelay, collider));

        ApplyHitStun = _defaultApplyHitStun;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OneTimeStatusEffects.Clear();
    }
}

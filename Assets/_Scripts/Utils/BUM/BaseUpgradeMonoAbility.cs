using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class BaseUpgradeMonoAbility : MonoBehaviour
{
    [InfoBox("$InfoString")]
    [Tooltip("If an identity is set, then BaseUpgradeMonoEvents will be triggered when using the effect is triggered.")]
    public BaseUpgradeMonoIdentity Identity;
    
    [TextArea]
    public string Description;

    [Tooltip("If true, conditions will be evalute as 'or', if one is true, then the condition will pass.")]
    public bool UseOrConditions;

    [Header("Events")]

    [Tooltip("This is called when the ability is triggered and the condition evaluated is false")]
    public UnityEvent FailedEvent;

    [Tooltip("If the event is success, this will return true.")]
    public UnityEvent SuccessEvent;



    [Tooltip("Triggers will directly trigger the effect. Having multiple triggers will not make the object require more than one trigger to activate, beause that is like really impossible. However, it still needs to go through the regular process of conditions. ")]
    protected List<BaseUpgradeMonoTrigger> _triggers;

    [Tooltip("Conditions are things that constantly checks for some type of condition, like player health etc. Will only check conditions if the triggers are checked.")]
    protected List<BaseUpgradeMonoCondition> _conditions;

    [Tooltip("Will filter the targets provided by the components. Filter out targets or add additional targets.")]
    protected List<BaseUpgradeMonoFilter> _filters;

    protected List<BaseUpgradeMonoEffect> _effects;

    public bool IsInitialized { get; protected set; }

    protected BaseUpgradeMonoComponent[] _monoComponents;
    protected List<Transform> _targetsCache;

    string InfoString()
    {
        string msg = "BUM Components \n";
        GetBUMComponents();
        msg += GetComponentLabelStrings("Triggers", _triggers);
        msg += GetComponentLabelStrings("Conditions", _conditions);
        msg += GetComponentLabelStrings("Filters", _filters);
        msg += GetComponentLabelStrings("Effects", _effects);


        return msg;


        string GetComponentLabelStrings(string prefix, IEnumerable<BaseUpgradeMonoComponent> components)
        {
            string s = prefix + "\n";

            if (components == null) return s;

            foreach (var item in components)
            {
                s += $"<{item.GetType()}>: {item.Label}{"\n"}";
                // s += "\n";
            }
            s += "\n";
            return s;
        }
    }


    protected void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        if (IsInitialized) return;

        IsInitialized = true;

        _targetsCache = new();

        // Get all bum components
        GetBUMComponents();

        // Get mono from this object
        _monoComponents = GetComponents<BaseUpgradeMonoComponent>();

        // _monoComponents = GetComponentsInChildren<BaseUpgradeMonoComponent>();
        foreach (var comp in _monoComponents)
        {
            comp.Initialize();
        }

        foreach (BaseUpgradeMonoTrigger trigger in _triggers)
        {
            trigger.OnTriggered += OnTriggered;
        }
    }

    public void Trigger()
    {
        TryUseAbility();
    }

    protected virtual void GetBUMComponents()
    {
        _triggers = GetComponents<BaseUpgradeMonoTrigger>().ToList();
        _conditions = GetComponents<BaseUpgradeMonoCondition>().ToList();
        _filters = GetComponents<BaseUpgradeMonoFilter>().ToList();
        _effects = GetComponents<BaseUpgradeMonoEffect>().ToList();
    }

    protected virtual void Update()
    {
    }


    protected virtual void OnTriggered(BaseUpgradeMonoTrigger trigger)
    {
        TryUseAbility();
    }

    protected virtual bool TryUseAbility()
    {
        if (!CheckConditions())
        {
            FailedEvent?.Invoke();
            return false;
        }

        // Get them targets
        _targetsCache.Clear();
        foreach (BaseUpgradeMonoComponent comp in _monoComponents)
        {
            _targetsCache.AddRange(comp.GetTargets());
        }

        // Filter them targets
        foreach (BaseUpgradeMonoFilter filter in _filters)
        {
            filter.FilterTargets(_targetsCache);
        }

        // Apply effects to them targets
        if (_targetsCache.Count == 0)
        {
            Debug.Log($"{gameObject.name} 's BUM Ability will not trigger because of no targets.");
            FailedEvent?.Invoke();
            return false;
        }

        // Execute the effecs.
        foreach (Transform target in _targetsCache.Distinct())
        {
            foreach (BaseUpgradeMonoEffect effect in _effects)
            {
                effect.Execute(target);
            }
        }

        // Trigger them events
        if (Identity != null)
        {
            BaseUpgradeMonoEvent.Trigger(Identity, BaseUpgradeMonoEventType.Used);
        }

        if (SuccessEvent != null) SuccessEvent.Invoke();

        return true;
    }

    protected virtual bool CheckConditions()
    {
        if (UseOrConditions)
        {
            return _conditions.Any(c => c.Evaluate());
        }
        else
        {
            return _conditions.All(c => c.Evaluate());
        }
        // bool result = false;
        // foreach (var condition in _conditions)
        // {
        //     result = !condition.Evaluate();
        //     if (!condition.Evaluate()) return false;
        // }
        // return true;
    }

    public virtual string GetDescription()
    {
        return Description;
    }

    public virtual void ResetAbility()
    {
        foreach (var component in _monoComponents)
        {
            component.Reset();
        }
    }

    protected virtual void OnEnable()
    {
        ResetAbility();
    }


}

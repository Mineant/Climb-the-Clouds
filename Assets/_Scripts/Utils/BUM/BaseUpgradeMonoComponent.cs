using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeMonoComponent : MonoBehaviour
{
    public string Label;

    [Tooltip("Will trigger Debug Log Error on important functions for checking purpose.")]
    public bool DebugMode;
    protected List<Transform> _targets = new();

    // protected virtual void Awake()
    // {
    //     // _targets
    // }

    public virtual void Initialize()
    {
        // _targets = new();
    }

    protected virtual void LateUpdate()
    {
        _targets.Clear();
    }


    protected void AddTarget(Transform target)
    {
        _targets.Add(target);
    }

    public virtual IEnumerable<Transform> GetTargets()
    {
        foreach (var target in _targets)
        {
            yield return target;
        }
    }

    public virtual void Reset() { }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }
}

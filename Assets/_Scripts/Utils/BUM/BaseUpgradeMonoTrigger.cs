using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseUpgradeMonoTrigger : BaseUpgradeMonoComponent
{

    public Action<BaseUpgradeMonoTrigger> OnTriggered;

    protected void Trigger()
    {
        if (DebugMode) Debug.LogError($"Debug Mode Triggered. Component: {this.name}");
        if (OnTriggered != null) OnTriggered.Invoke(this);
    }


}

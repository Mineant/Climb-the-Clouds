using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeMonoFilter : BaseUpgradeMonoComponent
{
    public abstract void FilterTargets(List<Transform> targetsCache);
}

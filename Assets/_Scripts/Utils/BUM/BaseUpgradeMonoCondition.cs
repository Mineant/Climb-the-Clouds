using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeMonoCondition : BaseUpgradeMonoComponent
{
    // The condition should give the desired target if any
    public abstract bool Evaluate();
}

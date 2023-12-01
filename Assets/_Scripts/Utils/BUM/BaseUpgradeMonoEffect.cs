using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeMonoEffect : BaseUpgradeMonoComponent
{
    public abstract void Execute(Transform target);
}

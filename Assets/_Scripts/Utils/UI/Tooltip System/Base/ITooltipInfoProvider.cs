using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITooltipInfoProvider
{

}

public interface ITooltipInfoProvider<TInfo> : ITooltipInfoProvider where TInfo : TooltipInfo
{
    TInfo GetTooltipInfo();
}

[System.Serializable]
public abstract class TooltipInfo
{

}
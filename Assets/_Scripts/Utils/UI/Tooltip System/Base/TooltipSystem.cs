using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MoreMountains.Tools;

public class TooltipSystem : MMSingleton<TooltipSystem>
{
    // private static TooltipSystem _instance;

    public List<Tooltip> Tooltips;


    // void Awake()
    // {
    //     _instance = this;
    // }

    public static void Show<T>(T tooltipProvider) where T : ITooltipInfoProvider
    {
        Instance.Tooltips.First(t => t.Show(tooltipProvider));
    }

    public static void Hide<T>(T tooltipProvider) where T : ITooltipInfoProvider
    {
        Instance.Tooltips.First(t => t.Hide(tooltipProvider));
    }
}

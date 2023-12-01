using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectDisplay : MonoBehaviour
{
    public TMP_Text StatusEffectText;
    public List<StatusEffectData> StatusEffectDatas;

    void Awake()
    {
        List<string> effects = new();
        foreach (var data in StatusEffectDatas)
        {
            string effect = "";
            effect = $"<b><color=#{ColorSettings.Orange.ToHexString()}>{data.Name}:</b></color> {data.Description}";
            effects.Add(effect);
        }

        StatusEffectText.text = String.Join("\n", effects);
    }
}

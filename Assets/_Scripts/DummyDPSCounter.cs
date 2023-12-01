using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;

public class DummyDPSCounter : MonoBehaviour
{
    public TMP_Text DPSText;
    public float TotalDamage;
    public float AverageDamage;
    private Health _health;


    void Awake()
    {
        _health = GetComponentInParent<Health>();
        _health.OnHit += OnHit;
    }

    void Update()
    {
        AverageDamage = TotalDamage / 10f;
        DPSText.text = $"DPS: {AverageDamage}";
    }

    private void OnHit()
    {
        float damage = _health.LastDamage;
        StartCoroutine(_CountDamageRoutine(damage));
    }

    private IEnumerator _CountDamageRoutine(float damage)
    {
        TotalDamage += damage;
        yield return new WaitForSeconds(10);
        TotalDamage -= damage;
    }

}

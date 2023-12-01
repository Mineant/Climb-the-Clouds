using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterWuXiaResource : CharacterAbility
{
    [Header("Resources")]
    public Bar QiBar;
    public Bar AuraBar;

    [Tooltip("How many aura will become one cell")]
    public int AuraToCellAmount = 100;
    public int MaxAuraCell = 5;

    [Header("Progress Bars")]
    public MMProgressBar QiProgressBar;
    public MMProgressBar AuraProgressBar;

    protected override void Initialization()
    {
        base.Initialization();

        QiBar.OnChange += UpdateQiBarUI;
        AuraBar.OnChange += UpdateAuraBarUI;

        AuraBar.SetMaxValue(MaxAuraCell * AuraToCellAmount);

        QiBar.ChangeValue(0f);   // Update the bar.
        AuraBar.ChangeValue(0f);
    }

    public int GetAuraCell()
    {
        return (int)(AuraBar.CurrentValue / AuraToCellAmount);
    }

    public void ChangeAuraCell(int amount)
    {
        AuraBar.ChangeValue(amount * AuraToCellAmount);
    }


    public void UpdateQiBarUI()
    {
        if (QiProgressBar != null)
        {
            QiProgressBar.UpdateBar(QiBar.CurrentValue, 0, QiBar.MaxValue);
        }

        if (UIManager.HasInstance && _character.CharacterType == Character.CharacterTypes.Player)
        {
            UIManager.Instance.SetQiBar(QiBar.CurrentValue, QiBar.MaxValue, _character.PlayerID);
        }
    }

    public void UpdateAuraBarUI()
    {
        if (AuraProgressBar != null)
        {
            AuraProgressBar.UpdateBar(AuraBar.CurrentValue, 0, AuraBar.MaxValue);
        }

        if (UIManager.HasInstance && _character.CharacterType == Character.CharacterTypes.Player)
        {
            UIManager.Instance.SetPlayerAuraBar(AuraBar.CurrentValue, AuraBar.MaxValue, MaxAuraCell);
        }
    }



    public override void ProcessAbility()
    {
        base.ProcessAbility();

        QiBar.Update();
        AuraBar.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        QiBar.SetValue(QiBar.MaxValue);
        AuraBar.SetValue(0);
        UpdateQiBarUI();
        UpdateAuraBarUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}

[System.Serializable]
public class Bar
{
    public enum RegenerationPauseType { None, AfterIncrease, AfterDecrease }

    [field: SerializeField]
    public float MaxValue { get; protected set; }

    [field: SerializeField]
    public float CurrentValue { get; protected set; }

    [Header("Regeneration Rate")]
    public float RegenerationRate;
    public RegenerationPauseType PauseType;
    public float PauseDuration;

    public Action OnChange;
    protected float _decreasedTimestamp = Mathf.NegativeInfinity;
    protected float _increasedTimestamp = Mathf.NegativeInfinity;

    /// <summary>
    /// Update call
    /// </summary>
    public void Update()
    {
        AutoRegen();
    }

    private void AutoRegen()
    {
        switch (PauseType)
        {
            case (RegenerationPauseType.AfterIncrease):
                if (Time.time < _increasedTimestamp + PauseDuration) return;
                break;
            case (RegenerationPauseType.AfterDecrease):
                if (Time.time < _decreasedTimestamp + PauseDuration) return;
                break;
        }

        ChangeValue(RegenerationRate * Time.deltaTime);
    }


    public void ChangeValue(float value)
    {
        if (value == 0) return;

        SetValue(Mathf.Clamp(CurrentValue + value, 0f, MaxValue));
    }

    public void SetValue(float newValue)
    {
        if (newValue != CurrentValue)
        {
            if (newValue > CurrentValue) _increasedTimestamp = Time.time;
            else _decreasedTimestamp = Time.time;
            CurrentValue = newValue;
            if (OnChange != null) OnChange.Invoke();

        }
    }

    public void SetMaxValue(float maxValue)
    {
        MaxValue = maxValue;
    }

    public void SetRegenerationRate(float rate) => RegenerationRate = rate;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class UIManager : MMSingleton<UIManager>, MMEventListener<MMInventoryEvent>
{
    public GameObject HUD;
    public List<MMProgressBar> HealthBars;
    public List<MMProgressBar> QiBars;
    public List<MMProgressBar> SkillTimeBars;
    public List<MMProgressBar> RecoveryTimeBars;
    public List<MMProgressBar> SkillBookResetTimeBars;

    public List<MMProgressBar> PlayerAuraBars;
    public GameObject RightClickTooltip;

    public List<SkillUIContainer> SkillBags;

    public MMF_Player WarningMessageFeedback;


    [Header("Tooltip")]
    public SkillUIProduct SkillTooltip;
    public SkillBookUIProduct SkillBookTooltip;


    protected MMProgressBar _cacheBar;


    private void UpdateBarMaxValue(MMProgressBar bar, float current, float min, float max)
    {
        if (bar)
        {
            bar.TextValueMultiplier = max;
            bar.SetBar(current, 0f, max);
        }
    }


    public void SetQiBar(float current, float max, string id)
    {
        UpdateBarMaxValue(QiBars.Find(b => b.PlayerID == id), current, 0f, max);
    }


    public void SetPlayerAuraBar(float current, float max, int maxAuraCell)
    {
        if (PlayerAuraBars.Count < maxAuraCell) Debug.LogError("NO");

        float interval = max / maxAuraCell;
        int currentBars = (int)(current / interval);
        float remainder = current - currentBars * interval;

        for (int i = 0; i < PlayerAuraBars.Count; i++)
        {
            if (i >= maxAuraCell) UpdateBarMaxValue(PlayerAuraBars[i], 0f, 0f, 1f);
            if (i < currentBars)
            {
                UpdateBarMaxValue(PlayerAuraBars[i], 1f, 0f, 1f);
            }
            else if (i == currentBars)
            {
                UpdateBarMaxValue(PlayerAuraBars[i], remainder, 0f, interval);
            }
            else
            {
                UpdateBarMaxValue(PlayerAuraBars[i], 0f, 0f, 1f);
            }
        }
    }

    public void HighlightPlayerAuraBar(int count)
    {
        for (int i = 0; i < PlayerAuraBars.Count; i++)
        {
            PlayerAuraBars[i].transform.GetChild(0).gameObject.SetActive(i < count);
        }
        RightClickTooltip.SetActive(count > 0);
    }

    public void SetSkillTimeBar(float current, float max, string id)
    {
        UpdateBarMaxValue(SkillTimeBars.Find(b => b.PlayerID == id), current, 0f, max);
    }

    public void SetRecoveryTimeBar(float current, float max, string id)
    {
        UpdateBarMaxValue(RecoveryTimeBars.Find(b => b.PlayerID == id), current, 0f, max);
    }

    public void SetSkillBookResetTimeBar(float current, float max, string id)
    {
        UpdateBarMaxValue(SkillBookResetTimeBars.Find(b => b.PlayerID == id), current, 0f, max);
    }

    public void SetHealthBar(float current, float max, string id)
    {
        UpdateBarMaxValue(HealthBars.Find(b => b.PlayerID == id), current, 0f, max);
    }

    internal void SetSkillBag(Skill[] skillBag, string playerID)
    {
        var skillContainer = SkillBags.FirstOrDefault(s => s.MatchPlayerID(playerID));
        if (skillContainer == null) return;

        skillContainer.DestroyAllProducts();
        foreach (var skill in skillBag)
        {
            skillContainer.GenerateNewProduct(new(skill));
        }
    }

    public void OnMMEvent(MMInventoryEvent eventType)
    {
        if (eventType.InventoryEventType == MMInventoryEventType.Select && eventType.EventItem != null)
        {
            HighlightSkillOrSkillBookFromItem(eventType.EventItem);
        }
        else if (eventType.InventoryEventType == MMInventoryEventType.Select && eventType.EventItem == null)
        {
            SkillTooltip.Hide();
            SkillBookTooltip.Hide();
        }
    }


    public void HighlightSkillOrSkillBookFromItem(InventoryItem item)
    {
        SkillTooltip.Hide();
        SkillBookTooltip.Hide();

        if (item is Skill skill) SkillTooltip.Generate(new(skill));
        else if (item is SkillBook skillBook) SkillBookTooltip.Generate(new(skillBook));
    }

    protected void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();
    }

    protected void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();
    }

    public void ShowHUD()
    {
        HUD.GetOrAddComponent<CanvasGroup>().EnableCanvasGroup(true);
    }

    public void HideHUD()
    {
        HUD.GetOrAddComponent<CanvasGroup>().EnableCanvasGroup(false);
    }


    public void ShowWarning(string warningMessage)
    {
        var tmpText = WarningMessageFeedback.GetFeedbackOfType<MMF_TMPText>();
        tmpText.NewText = warningMessage;
        // if(WarningMessageFeedback.IsPlaying) WarningMessageFeedback.StopFeedbacks();
        WarningMessageFeedback.PlayFeedbacks();
    }


    // internal void ResetRuntimeSkillBooks(string playerID)
    // {
    //     foreach (var product in SkillBooks.Where(s => s.MatchPlayerID(playerID)))
    //     {
    //         product.Hide();
    //     }
    // }

    // internal void SetRuntimeSkillBook(RuntimeSkillBook runtimeSkillBook, int index, string playerID)
    // {
    //     var skillBookProduct = SkillBooks.FirstOrDefault(s => s.MatchPlayerID(playerID) && s.InventoryDisplay.TargetInventoryName == runtimeSkillBook.InventoryName);
    //     if (skillBookProduct == null) return;

    //     skillBookProduct.Generate(new(runtimeSkillBook));
    // }





}

public interface IPlayerID
{
    bool MatchPlayerID(string playerID);
}
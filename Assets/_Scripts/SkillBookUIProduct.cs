using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookUIProduct : Product<SkillBookUIProductArgs>, ISkillBookTooltipInfoProvider
{
    [Header("Information")]
    public Image IconImage;
    public TMP_Text NameText;
    public TMP_Text RarityText;
    public TMP_Text SkillTypeText;
    public TMP_Text SpecialEffectDescriptionText;
    public StatValueUIContainer StatValueUIContainer;

    [Header("Interactive")]
    public bool ShowTooltip;
    public GameObject HighlightOverlay;

    public SkillBook SkillBook { get; protected set; }

    public override void Generate(SkillBookUIProductArgs productArgs)
    {
        base.Generate(productArgs);

        SkillBook = productArgs.SkillBook;
        if (IconImage) IconImage.sprite = SkillBook.Icon;
        if (NameText) NameText.text = SkillBook.ItemName;
        if (RarityText) RarityText.text =StatHelper.GetRarityString(SkillBook.Rarity);
        if (SkillTypeText != null) SkillTypeText.text = SkillBook.RelatedSkillTypes.Count > 0 ? String.Join(", ", SkillBook.RelatedSkillTypes) : "";
        if (SpecialEffectDescriptionText && SkillBook.SpecialEffect != null) SpecialEffectDescriptionText.text = $"{SkillBook.SpecialEffect.GetDescription()}";
        if (StatValueUIContainer) StatValueUIContainer.Generate(SkillBook.BasicStats);

        SetHighlight(false);
    }



    public override void Hide()
    {
        base.Hide();
        SkillBook = null;
    }

    public void SetHighlight(bool active)
    {
        if (HighlightOverlay) HighlightOverlay.SetActive(active);
    }


    public SkillBookTooltipInfo GetTooltipInfo()
    {
        return new(SkillBook);
    }

    protected override void ProductInteractorClick()
    {
        base.ProductInteractorClick();

        if (ShowTooltip)
        {
            if (UIManager.HasInstance && SkillBook != null)
            {
                UIManager.Instance.HighlightSkillOrSkillBookFromItem(SkillBook);
            }
        }
    }
}

public class SkillBookUIProductArgs : ProductArgs
{
    public SkillBook SkillBook { get; protected set; }

    public SkillBookUIProductArgs(SkillBook skillBook)
    {
        SkillBook = skillBook;
    }
}
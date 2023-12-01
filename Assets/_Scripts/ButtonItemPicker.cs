using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonActivated))]
public class ButtonItemPicker : ItemPicker
{
    [System.Serializable]
    public class RarityEffect { public Rarity Rarity; public GameObject Effect; }

    [Header("Button")]
    public Image PreviewIconImage;

    [Header("Skill and skill book")]
    public List<RarityEffect> RarityEffects;

    [Header("Goods")]
    public GameObject CostObject;
    public TMP_Text CostText;
    public int Cost;

    public ButtonActivated ButtonActivated { get; protected set; }

    public Action OnPicked;

    void Awake()
    {
        ButtonActivated = GetComponent<ButtonActivated>();
        ButtonActivated.OnActivation.AddListener(OnActivation);

        MyIntialization();
    }

    public void MyIntialization()
    {
        if (Item == null) return;

        SetPreviewIconImage();
        Rarity rarity = (Rarity)9999;
        if (Item is Skill skill)
            rarity = skill.Rarity;
        else if (Item is SkillBook skillBook)
            rarity = skillBook.Rarity;
        if ((int)rarity != 9999)
        {
            foreach (RarityEffect rarityEffect in RarityEffects)
                rarityEffect.Effect.SetActive(false);
            RarityEffects.Find(e => e.Rarity == rarity).Effect.SetActive(true);
        }
        else
        {
            foreach (RarityEffect rarityEffect in RarityEffects)
                rarityEffect.Effect.SetActive(false);
        }

        if (Cost > 0)
        {
            CostObject.SetActive(true);
            CostText.text = Cost.ToString();
        }
        else
        {
            CostObject.SetActive(false);
        }
    }

    private void OnActivation()
    {
        if (Cost > 0)
        {
            if (GameManager.Instance.Points < Cost)
            {
                Debug.Log($"Cost: {Cost}. Points: {GameManager.Instance.Points}");
                if (UIManager.HasInstance) UIManager.Instance.ShowWarning("Not enough Energy Stones");
                return;
            }
        }

        bool success = Pick(Item.TargetInventoryName);
        if (OnPicked != null) OnPicked.Invoke();
        TopDownEnginePointEvent.Trigger(PointsMethods.Add, -Cost);
    }

    [ContextMenu("Set Preview Icon Image")]
    public void SetPreviewIconImage()
    {
        if (Item != null)
        {
            PreviewIconImage.sprite = Item.Icon;
        }
    }

    public override bool Pickable()
    {
        if (!PickableIfInventoryIsFull && _targetInventory.NumberOfFreeSlots == 0)
        {
            // we make sure that there isn't a place where we could store it
            int spaceAvailable = 0;
            List<int> list = _targetInventory.InventoryContains(Item.ItemID);
            if (list.Count > 0)
            {
                foreach (int index in list)
                {
                    spaceAvailable += (Item.MaximumStack - _targetInventory.Content[index].Quantity);
                }
            }

            if (Item.Quantity <= spaceAvailable)
            {
                return true;
            }
            else
            {
                ////// MINE //////
                if (UIManager.HasInstance) UIManager.Instance.ShowWarning("Inventory is full");
                ////// END //////
                return false;
            }
        }

        return true;
    }

    // [ContextMenu("Set Icon")]
    // private void SetIcon()
    // {
    //     if (Item is Skill || Item is SkillBook)
    //     {
    //         Rarity rarity = Rarity.Common;
    //         if (Item is Skill skill) rarity = skill.Rarity;
    //         if (Item is SkillBook skillBook) rarity = skillBook.Rarity;

    //         ShapeIcon icon = ShapeIcon.CircleGray;
    //         switch (rarity)
    //         {
    //             case (Rarity.Common):
    //                 icon = ShapeIcon.DiamondGreen;
    //                 break;
    //             case (Rarity.Uncommon):
    //                 icon = ShapeIcon.DiamondBlue;
    //                 break;
    //             case (Rarity.Rare):
    //                 icon = ShapeIcon.DiamondPurple;
    //                 break;
    //             case (Rarity.Legendary):
    //                 icon = ShapeIcon.DiamondRed;
    //                 break;
    //         }
    //         IconManager.SetIcon(gameObject, icon);
    //     }
    // }


    public override void OnTriggerEnter(Collider collider)
    {
        // Shouldn't involve base, since will auto pick
        // base.OnTriggerEnter(collider);

        if ((!collider.CompareTag("Player"))) return;

        if (Item is Skill || Item is SkillBook)
        {
            if (UIManager.HasInstance) UIManager.Instance.HighlightSkillOrSkillBookFromItem(Item);
        }

    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        // base.OnTriggerEnter2D(collider);
    }


}

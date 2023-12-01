using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<Transform> GoodsPoints;
    public RarityLootTable GoodsRarityLootTable;
    public ButtonItemPicker RefreshGoodsItemPicker;
    public ButtonItemPicker SellGoodsItemPicker;
    public int CurrentNumberOfRefreshes;
    public MMTriggerAndCollision SellItemHandler;
    public TMP_Text SellGoodsExtimatedRevenueText;

    public List<ButtonItemPicker> SpawnedGoods;
    public List<ButtonItemPicker> GoodsToBeSold;

    void Awake()
    {
        SellItemHandler.OnTriggerEnterEvent.AddListener(OnColliderEnterSellItemZone);
        SellItemHandler.OnTriggerExitEvent.AddListener(OnColliderExitSellItemZone);
        RefreshGoodsItemPicker.OnPicked += OnRefreshItems;
        SellGoodsItemPicker.OnPicked += OnSellGoods;

        SetUnableToMove(RefreshGoodsItemPicker);
        SetUnableToMove(SellGoodsItemPicker);
    }


    void Start()
    {
    }


    private void OnSellGoods()
    {
        var tempList = new List<ButtonItemPicker>(GoodsToBeSold);
        foreach (var goods in tempList)
        {
            if (goods.Item is BaseInventoryItem baseInventoryItem)
            {
                TopDownEnginePointEvent.Trigger(PointsMethods.Add, baseInventoryItem.GetSellRevenue());
            }

            Destroy(goods.gameObject);
        }
        GoodsToBeSold.Clear();
        UpdateSellGoodsText();
    }

    private void OnColliderEnterSellItemZone()
    {
        var picker = SellItemHandler.Target.GetComponentInParent<ButtonItemPicker>();
        if (picker != null && !GoodsToBeSold.Contains(picker))
        {
            GoodsToBeSold.Add(picker);
            UpdateSellGoodsText();
        }
    }


    private void OnColliderExitSellItemZone()
    {
        var picker = SellItemHandler.Target.GetComponentInParent<ButtonItemPicker>(true);
        if (picker != null)
        {
            GoodsToBeSold.Remove(picker);
        }

        UpdateSellGoodsText();
    }


    public void Initialization(RarityLootTable rarityLootTable)
    {
        GoodsRarityLootTable = rarityLootTable;
        CurrentNumberOfRefreshes = 0;
        SpawnedGoods = new();

        UpdateSellGoodsText();
        RefreshGoods();
    }

    private void UpdateSellGoodsText()
    {
        SellGoodsExtimatedRevenueText.text = GoodsToBeSold.Select(s => s.Item as BaseInventoryItem).Sum(i => i.GetSellRevenue()) + "";
    }


    private void OnRefreshItems()
    {
        CurrentNumberOfRefreshes += 1;
        RefreshGoods();
    }

    public void RefreshGoods()
    {
        // Destroy the spawned items
        foreach (var goods in SpawnedGoods)
        {
            Destroy(goods.gameObject);
        }
        SpawnedGoods.Clear();

        // Update the refresh cost
        RefreshGoodsItemPicker.Cost = (int)((StageManager.Instance.CurrentStageIndex * 1.2) * Mathf.Pow(1.05f, CurrentNumberOfRefreshes * 2));
        RefreshGoodsItemPicker.MyIntialization();

        // Spawn goods
        foreach (Transform point in GoodsPoints)
        {
            BaseInventoryItem item = ResourceSystem.Instance.GetSkillAndSkillBooks(GoodsRarityLootTable.GetLoot().Loot).PickRandom<BaseInventoryItem>();
            if (item is SkillBook skillBook) item = skillBook.GetInstance();
            ButtonItemPicker itemPicker = item.SpawnPrefab(point.position).GetComponent<ButtonItemPicker>();
            itemPicker.transform.position = point.position;
            itemPicker.Cost = item.GetBuyCost();
            itemPicker.MyIntialization();
            SpawnedGoods.Add(itemPicker);
            SetUnableToMove(itemPicker);
        }
    }

    public void SetUnableToMove(ButtonItemPicker buttonItemPicker)
    {
        Destroy(buttonItemPicker.GetComponent<Rigidbody>());
    }
}

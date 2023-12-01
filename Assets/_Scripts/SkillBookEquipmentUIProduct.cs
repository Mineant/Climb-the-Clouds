using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookEquipmentUIProduct : Product<SkillBookEquipmentUIProductArgs>, MMEventListener<SkillBookEvent>
{
    public InventoryDisplay InventoryDisplay;
    public CanvasGroup CanvasGroup;
    public LayoutElement LayoutElement;
    public SkillBookUIProduct SkillBookUIProduct;
    public Button UnequipButton;
    public GameObject DropMeObject;

    protected string _inventoryName;

    protected override void Initialize()
    {
        base.Initialize();

        _inventoryName = InventoryDisplay.TargetInventoryName;
        UnequipButton.onClick.AddListener(Unequip);
    }

    private void Unequip()
    {
        SkillBookEvent.Trigger(SkillBookEventType.UnequipSkillBook, _inventoryName, null, 0, 0, 0);
    }


    public override void Generate(SkillBookEquipmentUIProductArgs productArgs)
    {
        base.Generate(productArgs);

    }



    public void OnMMEvent(SkillBookEvent eventType)
    {
        if (eventType.InventoryName != _inventoryName) return;

        switch (eventType.SkillBookEventType)
        {
            case (SkillBookEventType.ShowedSkillBook):
                CanvasGroup.EnableCanvasGroup(true);
                LayoutElement.ignoreLayout = false;
                break;
            case (SkillBookEventType.HidedSkillBook):
                CanvasGroup.EnableCanvasGroup(false);
                LayoutElement.ignoreLayout = true;
                break;
            case (SkillBookEventType.EquippedSkillBook):
                InventoryDisplay.NumberOfRows = eventType.NumberOfRows;
                InventoryDisplay.NumberOfColumns = eventType.NumberOfColumns;
                SkillBookUIProduct.Generate(new(eventType.SkillBook));
                break;
            case (SkillBookEventType.UnequippedSkillBook):
                InventoryDisplay.NumberOfRows = 0;
                InventoryDisplay.NumberOfColumns = 0;
                break;
            case (SkillBookEventType.HighlightedSkillBookSlot):
                foreach (var slot in InventoryDisplay.SlotContainer)
                {
                    slot.GetComponent<SkillUIProduct>().SetHighlight(false);
                }
                if (eventType.Index >= 0)
                {
                    InventoryDisplay.SlotContainer[eventType.Index].GetComponent<SkillUIProduct>().SetHighlight(true);
                    SkillBookUIProduct.SetHighlight(true);
                }
                else
                {
                    SkillBookUIProduct.SetHighlight(false);
                }
                break;
            case (SkillBookEventType.Rearranged):
                transform.SetSiblingIndex(eventType.Index);
                break;

        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<SkillBookEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SkillBookEvent>();
    }
}

public class SkillBookEquipmentUIProductArgs : ProductArgs
{
}

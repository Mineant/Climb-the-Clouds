using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWuXiaHandleSkillBook : CharacterAbility, MMEventListener<MMInventoryEvent>, MMEventListener<SkillBookEvent>, MMEventListener<InventoryDragAndDropEvent>
{
    [Header("Skill Book")]
    public int MaxSkillBookEquipments = 3;
    public string SkillInventoryName = "SkillInventory";
    public string SkillBookInventoryName = "SkillBookInventory";
    public string EquipSkillBookInventoryName = "EquipSkillBookInventory";

    [Tooltip("Must be equal to skill book amount. Should have SkillBookUIProduct attached to it.")]
    public List<string> SkillBookEquipmentInventoryNames;
    public List<SkillBookEquipment> EquippedSkillBooks;
    public SkillBook DefaultSkillBook;
    public List<Skill> DefaultSkills;


    public Action OnSkillBookEquipmentsChanged;

    protected Inventory _skillInventory;
    protected Inventory _skillBookInventory;
    protected Inventory _equipSkillBookInventory;
    protected List<Inventory> _skillBookInventories;
    public Action<SkillBookEquipment> OnSkillBookEquipmentContentChanged;


    protected override void Initialization()
    {
        base.Initialization();

        _skillInventory = Inventory.FindInventory(SkillInventoryName, _character.PlayerID);
        _skillInventory.TargetTransform = _character.transform;
        _equipSkillBookInventory = Inventory.FindInventory(EquipSkillBookInventoryName, _character.PlayerID);
        _equipSkillBookInventory.TargetTransform = _character.transform;
        _equipSkillBookInventory.ResizeArray(1);
        _skillBookInventory = Inventory.FindInventory(SkillBookInventoryName, _character.PlayerID);
        _skillBookInventory.TargetTransform = _character.transform;

        // find the inventories
        if (MaxSkillBookEquipments != SkillBookEquipmentInventoryNames.Count) Debug.LogError("Must be the same.");

        _skillBookInventories = new();
        foreach (string inventoryName in SkillBookEquipmentInventoryNames)
        {
            Inventory inventory = Inventory.FindInventory(inventoryName, _character.PlayerID);
            inventory.TargetTransform = _character.transform;
            SkillBookEvent.Trigger(SkillBookEventType.HidedSkillBook, inventoryName, null, 0, 0, 0);
            _skillBookInventories.Add(inventory);
        }

        if (DefaultSkillBook != null)
        {
            EquipSkillBook(DefaultSkillBook);
            foreach (var skill in DefaultSkills)
            {
                EquippedSkillBooks[0].TargetInventory.AddItem(skill, 1);
            }

        }
    }

    public void UpdateUI()
    {
        // Hide all skill book equipments
        foreach (string inventoryName in SkillBookEquipmentInventoryNames)
        {
            SkillBookEvent.Trigger(SkillBookEventType.HidedSkillBook, inventoryName, null, 0, 0, 0);
        }

        // Show equipped skill book
        for (int i = 0; i < EquippedSkillBooks.Count; i++)
        {
            var skillBookEquipment = EquippedSkillBooks[i];
            SkillBookEvent.Trigger(SkillBookEventType.ShowedSkillBook, skillBookEquipment.TargetInventory.name, null, 0, 0, 0);
            SkillBookEvent.Trigger(SkillBookEventType.EquippedSkillBook, skillBookEquipment.TargetInventory.name, skillBookEquipment.TargetSkillBook, 1, skillBookEquipment.TargetSkillBook.MaxSkillSlots, 0);
            SkillBookEvent.Trigger(SkillBookEventType.Rearranged, skillBookEquipment.TargetInventory.name, null, 0, 0, i);
        }

        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, SkillInventoryName, null, 0, 0, _character.PlayerID);
        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, SkillBookInventoryName, null, 0, 0, _character.PlayerID);
        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, EquipSkillBookInventoryName, null, 0, 0, _character.PlayerID);
    }

    protected void SkillBookChanged()
    {
        if (EquippedSkillBooks.Count < MaxSkillBookEquipments)
        {
            _equipSkillBookInventory.ResizeArray(1);
        }
        else
        {
            _equipSkillBookInventory.ResizeArray(0);
        }

        for (int i = 0; i < EquippedSkillBooks.Count; i++)
        {
            SkillBookEquipment skillBookEquipment = EquippedSkillBooks[i];
            SkillBookEvent.Trigger(SkillBookEventType.Rearranged, skillBookEquipment.TargetInventory.name, null, 0, 0, i);
        }

        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, EquipSkillBookInventoryName, null, 0, 0, _character.PlayerID);

        if (OnSkillBookEquipmentsChanged != null) OnSkillBookEquipmentsChanged.Invoke();
    }

    public bool EquipSkillBook(SkillBook skillBook)
    {
        if (EquippedSkillBooks.Count >= MaxSkillBookEquipments) return false;

        Inventory emptyInventory = null;
        foreach (var inventory in _skillBookInventories)
        {
            if (!EquippedSkillBooks.Select(b => b.TargetInventory).Contains(inventory))
            {
                emptyInventory = inventory;
                break;
            }
        }

        emptyInventory.ResizeArray(skillBook.MaxSkillSlots);
        SkillBookEvent.Trigger(SkillBookEventType.ShowedSkillBook, emptyInventory.name, null, 0, 0, 0);
        SkillBookEvent.Trigger(SkillBookEventType.EquippedSkillBook, emptyInventory.name, skillBook, 1, skillBook.MaxSkillSlots, 0);
        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, emptyInventory.name, null, 0, 0, _character.PlayerID);

        SkillBookEquipment skillBookEquipment = new(skillBook, emptyInventory, this.gameObject);
        EquippedSkillBooks.Add(skillBookEquipment);
        SkillBookChanged();

        return true;
    }

    public bool UnequipSkillBook(int index)
    {
        if (!EquippedSkillBooks.IndexWithinRange(index)) return false;

        SkillBookEquipment skillBookEquipment = EquippedSkillBooks[index];

        int contentLength = skillBookEquipment.TargetInventory.Content.Length;
        for (int i = 0; i < contentLength; i++)
        {
            skillBookEquipment.TargetInventory.DropItem(skillBookEquipment.TargetInventory.Content[i], i);
        }

        skillBookEquipment.TargetInventory.EmptyInventory();
        skillBookEquipment.TargetInventory.ResizeArray(0);
        SkillBookEvent.Trigger(SkillBookEventType.HidedSkillBook, skillBookEquipment.TargetInventory.name, null, 0, 0, 0);
        SkillBookEvent.Trigger(SkillBookEventType.UnequippedSkillBook, skillBookEquipment.TargetInventory.name, null, 0, 0, 0);
        MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, skillBookEquipment.TargetInventory.name, null, 0, 0, _character.PlayerID);

        EquippedSkillBooks.Remove(skillBookEquipment);
        skillBookEquipment.TargetSkillBook.SpawnPrefab(_character.PlayerID);
        SkillBookChanged();

        return true;
    }

    public void AddSkillToSkillBag(Skill skill)
    {
        _skillInventory.AddItem(skill, 1);
    }

    protected void SkillBookInventoryContentChanged(Inventory inventory)
    {
        var equippedSkillBook = EquippedSkillBooks.Find(e => e.TargetInventory == inventory);
        equippedSkillBook.SetSkills(inventory.Content.Cast<Skill>().ToList());

        if (OnSkillBookEquipmentContentChanged != null) OnSkillBookEquipmentContentChanged.Invoke(equippedSkillBook);
    }

    public void OnMMEvent(MMInventoryEvent eventType)
    {
        if (eventType.PlayerID == _character.PlayerID && eventType.InventoryEventType == MMInventoryEventType.ContentChanged && SkillBookEquipmentInventoryNames.Contains(eventType.TargetInventoryName))
        {
            SkillBookInventoryContentChanged(_skillBookInventories.Find(d => d.name == eventType.TargetInventoryName));
        }

        if (eventType.PlayerID == _character.PlayerID && eventType.InventoryEventType == MMInventoryEventType.ContentChanged && EquipSkillBookInventoryName == eventType.TargetInventoryName)
        {
            if (_equipSkillBookInventory.Content.Length > 0 && _equipSkillBookInventory.Content[0] != null)
            {
                SkillBook skillBook = _equipSkillBookInventory.Content[0] as SkillBook;
                _equipSkillBookInventory.DestroyItem(0);

                EquipSkillBook(skillBook);
            }
        }
    }


    public void OnMMEvent(SkillBookEvent eventType)
    {
        switch (eventType.SkillBookEventType)
        {
            case (SkillBookEventType.UnequipSkillBook):
                UnequipSkillBook(EquippedSkillBooks.Select(b => b.TargetInventory.name).ToList().IndexOf(eventType.InventoryName));
                break;

        }
    }


    public void OnMMEvent(InventoryDragAndDropEvent eventType)
    {
        if (eventType.Item is SkillBook skillBook && EquippedSkillBooks.Count < MaxSkillBookEquipments)
        {
            if (eventType.EventType == InventoryDragAndDropEventType.BeginDrag)
            {
                SkillBookEvent.Trigger(SkillBookEventType.StartDrag, null, null, 0, 0, 0);
            }
            else if (eventType.EventType == InventoryDragAndDropEventType.EndDrag)
            {
                SkillBookEvent.Trigger(SkillBookEventType.EndDrag, null, null, 0, 0, 0);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        this.MMEventStartListening<MMInventoryEvent>();
        this.MMEventStartListening<SkillBookEvent>();
        this.MMEventStartListening<InventoryDragAndDropEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        this.MMEventStopListening<MMInventoryEvent>();
        this.MMEventStopListening<SkillBookEvent>();
        this.MMEventStopListening<InventoryDragAndDropEvent>();
    }

}


[System.Serializable]
public class SkillBookEquipment
{
    [field: SerializeField]
    public SkillBook TargetSkillBook { get; protected set; }

    [field: SerializeField]
    public List<Skill> Skills { get; protected set; }

    [field: SerializeField]
    public Inventory TargetInventory { get; protected set; }

    [field: SerializeField]
    public BaseUpgradeMono SpecialEffectInstance { get; protected set; }

    [field: SerializeField]
    public GameObject Owner { get; protected set; }

    public SkillBookEquipment(SkillBook skillBook, Inventory inventory, GameObject owner)
    {
        TargetSkillBook = skillBook;
        TargetInventory = inventory;
        Owner = owner;
        SpecialEffectInstance = null;
        Skills = new();
    }

    public void SetSkills(List<Skill> skills)
    {
        if (skills.Count > TargetSkillBook.MaxSkillSlots)
        {
            Debug.LogError("this shoudln't happen.");
            return;
        }

        Skills = skills;
        Skills.RemoveAll(d => d == null);
    }

    public bool AddSpecialEffect()
    {
        if (TargetSkillBook.SpecialEffect == null) return false;
        if (SpecialEffectInstance != null) return false;

        SpecialEffectInstance = GameObject.Instantiate(TargetSkillBook.SpecialEffect, Owner.transform);
        SpecialEffectInstance.Initialize(Owner);

        return true;
    }

    public bool RemoveSpecialEffect()
    {
        if (SpecialEffectInstance == null) return false;

        GameObject.Destroy(SpecialEffectInstance.gameObject);
        SpecialEffectInstance = null;

        return true;
    }
}

public enum SkillBookEventType
{
    EquipSkillBook, EquippedSkillBook, UnequipSkillBook, UnequippedSkillBook, ShowSkillBook, ShowedSkillBook, HideSkillBook, HidedSkillBook, HighlightSkillBookSlot, HighlightedSkillBookSlot, StartDrag, EndDrag,
    Rearranged,

}

public struct SkillBookEvent
{
    public SkillBookEventType SkillBookEventType;
    public string InventoryName;
    public SkillBook SkillBook;
    public int NumberOfRows;
    public int NumberOfColumns;
    public int Index;

    public SkillBookEvent(SkillBookEventType skillBookEventType, string inventoryName, SkillBook skillBook, int numberOfRows, int numberOfColumns, int highlightSlot)
    {
        SkillBookEventType = skillBookEventType;
        InventoryName = inventoryName;
        SkillBook = skillBook;
        NumberOfRows = numberOfRows;
        NumberOfColumns = numberOfColumns;
        Index = highlightSlot;
    }

    static SkillBookEvent e;

    public static void Trigger(SkillBookEventType skillBookEventType, string inventoryName, SkillBook skillBook, int numberOfRows, int numberOfColumns, int highlightSlot)
    {
        e.SkillBookEventType = skillBookEventType;
        e.InventoryName = inventoryName;
        e.SkillBook = skillBook;
        e.NumberOfRows = numberOfRows;
        e.NumberOfColumns = numberOfColumns;
        e.Index = highlightSlot;

        MMEventManager.TriggerEvent(e);
    }

}
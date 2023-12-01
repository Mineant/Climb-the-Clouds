using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Video;

public class BaseInventoryItem : InventoryItem, IGoods
{
    [Header("Base")]
    public InventoryItemType ItemType;



    public override GameObject SpawnPrefab(string playerID)
    {
        if (TargetInventory(playerID) != null && TargetInventory(playerID).TargetTransform != null)
        {
            return SpawnPrefab(TargetInventory(playerID).TargetTransform.position);
        }
        return null;
    }

    public GameObject SpawnPrefab(Vector3 position)
    {
        if (Prefab != null)
        {
            GameObject droppedObject = (GameObject)Instantiate(Prefab);
            if (droppedObject.GetComponent<ItemPicker>() != null)
            {
                if (ForcePrefabDropQuantity)
                {
                    droppedObject.GetComponent<ItemPicker>().Quantity = PrefabDropQuantity;
                    droppedObject.GetComponent<ItemPicker>().RemainingQuantity = PrefabDropQuantity;
                }
                else
                {
                    droppedObject.GetComponent<ItemPicker>().Quantity = Quantity;
                    droppedObject.GetComponent<ItemPicker>().RemainingQuantity = Quantity;
                }
                //////////// MINE ////////////
                if (droppedObject.GetComponent<ButtonItemPicker>() != null)
                {
                    droppedObject.GetComponent<ButtonItemPicker>().Item = this;
                    droppedObject.GetComponent<ButtonItemPicker>().MyIntialization();
                }
                //////////// END ////////////
            }

            MMSpawnAround.ApplySpawnAroundProperties(droppedObject, DropProperties,
                position);

            return droppedObject;
        }

        return null;
    }


    public virtual int GetBuyCost()
    {
        return 0;
    }

    public virtual int GetSellRevenue()
    {
        return 0;
    }
}

public enum InventoryItemType
{
    Normal, Equipment, Skill, SkillBook
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public Sprite icon;
    public enum ItemType { Weapon, Armor, Consumable }
    public ItemType itemType;

    public Item()
    {

    }

    public Item(Item _item)
    {
        itemName = _item.itemName;
        itemID = _item.itemID;
        icon = _item.icon;
        itemType = _item.itemType;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Item other = (Item)obj;
        return itemName == other.itemName && itemID == other.itemID && itemType == other.itemType;
        // 아이콘의 동등성 비교는 필요에 따라 추가할 수 있습니다.
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode() ^ itemID.GetHashCode() ^ itemType.GetHashCode();
        // 아이콘의 해시 코드는 필요에 따라 추가할 수 있습니다.
    }
}
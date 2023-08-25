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
        // �������� ��� �񱳴� �ʿ信 ���� �߰��� �� �ֽ��ϴ�.
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode() ^ itemID.GetHashCode() ^ itemType.GetHashCode();
        // �������� �ؽ� �ڵ�� �ʿ信 ���� �߰��� �� �ֽ��ϴ�.
    }
}
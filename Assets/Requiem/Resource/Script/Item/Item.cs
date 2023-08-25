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

    public Item(Item _item)
    {
        itemName = _item.itemName;
        itemID = _item.itemID;
        icon = _item.icon;
        itemType = _item.itemType;
    }
}
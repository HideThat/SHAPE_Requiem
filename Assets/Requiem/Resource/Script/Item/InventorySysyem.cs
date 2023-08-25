using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventorySysyem : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxInventorySize = 20;
    public GameObject ItemPanel;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();

    private void Start()
    {
        int count = ItemPanel.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            ItemSlot itemslot = ItemPanel.transform.GetChild(i).GetComponent<ItemSlot>();

            if (itemslot != null)
            {
                itemSlots.Add(itemslot);
            }
        }

        maxInventorySize = count;
    }

    public bool AddItem(Item item)
    {
        if (items.Count < maxInventorySize)
        {
            Item invenItem = new Item(item);

            items.Add(invenItem);

            // 해당 슬롯에 아이템 설정
            itemSlots[items.Count - 1].SetItem(invenItem);

            return true; // 아이템 추가 성공
        }
        return false; // 인벤토리 공간 부족
    }

    public void RemoveItem(Item item)
    {
        int index = items.IndexOf(item);
        if (index != -1)
        {
            items.RemoveAt(index);
            itemSlots[index].ClearSlot(); // 해당 슬롯의 아이템 제거

            // 나머지 아이템들을 위로 이동
            for (int i = index; i < items.Count; i++)
            {
                itemSlots[i].SetItem(items[i]);
            }

            // 마지막 슬롯 비우기
            if (items.Count < itemSlots.Count)
            {
                itemSlots[items.Count].ClearSlot();
            }
        }
    }

    public bool ContainsItem(Item item)
    {
        return items.Contains(item);
    }
}

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

            // �ش� ���Կ� ������ ����
            itemSlots[items.Count - 1].SetItem(invenItem);

            return true; // ������ �߰� ����
        }
        return false; // �κ��丮 ���� ����
    }

    public void RemoveItem(Item item)
    {
        int index = items.IndexOf(item);
        if (index != -1)
        {
            items.RemoveAt(index);
            itemSlots[index].ClearSlot(); // �ش� ������ ������ ����

            // ������ �����۵��� ���� �̵�
            for (int i = index; i < items.Count; i++)
            {
                itemSlots[i].SetItem(items[i]);
            }

            // ������ ���� ����
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

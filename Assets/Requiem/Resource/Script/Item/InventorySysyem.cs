using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mono.Cecil.Cil;

public class InventorySysyem : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxInventorySize = 20;
    public GameObject ItemPanel;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();

    private void Start()
    {
        int count = ItemPanel?.transform.childCount ?? 0;

        for (int i = 0; i < count; i++)
        {
            ItemSlot itemslot = ItemPanel.transform.GetChild(i).GetComponent<ItemSlot>();
            if (itemslot != null)
            {
                itemSlots.Add(itemslot);
                itemslot.ClearSlot(); // 초기 상태를 정리합니다.
            }
        }
        maxInventorySize = count;

        InvenOnOff(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (ItemPanel.activeInHierarchy)
            {
                InvenOnOff(false);
            }
            else
            {
                InvenOnOff(true);
            }
        }
    }

    public void InvenOnOff(bool _TF)
    {
        ItemPanel.SetActive(_TF);
    }

    public bool AddItem(Item item)
    {
        bool wasOpen = ItemPanel.activeInHierarchy; // 초기 활성화 상태 저장

        if (items.Count < maxInventorySize && item != null)
        {
            if (!wasOpen)
                InvenOnOff(true); // 패널 활성화

            items.Add(item);

            if (itemSlots[items.Count - 1].gameObject.activeInHierarchy)
            {
                itemSlots[items.Count - 1].SetItem(item);
            }
            else
            {
                Debug.Log("itemSlots error");
            }

            if (!wasOpen)
                InvenOnOff(false); // 초기에 비활성화되어 있었다면 다시 비활성화

            return true; // 아이템 추가 성공
        }

        return false; // 아이템 추가 실패
    }


    public void RemoveItem(Item item)
    {
        int index = items.IndexOf(item);
        if (index != -1)
        {
            items.RemoveAt(index);
            itemSlots[index].ClearSlot();

            // 나머지 아이템들 재배치
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
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Equals(item))
            {
                return true;
            }
        }

        return false;
    }
}

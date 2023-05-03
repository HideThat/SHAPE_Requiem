// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Transform[] inventoryBlocks = new Transform[32]; // �κ��丮 ����� Transform �迭
    [SerializeField] private PlayerInventorySystem playerInventoryData; // �÷��̾� �κ��丮 ������ ����


    private void Start()
    {
        InitializeVariables(); // ���� ����
        gameObject.SetActive(false); // ���� �� �κ��丮 �����
    }

    // ���� �ʱ�ȭ
    private void InitializeVariables()
    {
        // �÷��̾��� �κ��丮 �ý��� ������Ʈ ��������
        playerInventoryData = PlayerData.PlayerObj.GetComponent<PlayerInventorySystem>();

        // �κ��丮 ��� �Ҵ�
        for (int i = 0; i < 32; i++)
        {
            inventoryBlocks[i] = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i);
        }
    }

    // �κ��丮 ������Ʈ
    public void UpdateInventory()
    {
        ClearAllItems(); // ��� ������ ����

        for (int i = 0; i < 32; i++)
        {
            if (playerInventoryData.items[i] != null)
            {
                AddItem(playerInventoryData.items[i].m_ID, i); // ������ �߰�
            }
        }
    }

    // ��� ������ �����
    private void ClearAllItems()
    {
        for (int i = 0; i < 32; i++)
        {
            DeleteItem(i);
        }
    }

    // ������ �߰�
    private void AddItem(int id, int index)
    {
        inventoryBlocks[index].GetChild(0).GetComponent<Image>().sprite =
            DataController.ItemSprites[id];

        inventoryBlocks[index].GetChild(0).GetComponent<Image>().color =
            playerInventoryData.items[index].GetComponent<SpriteRenderer>().color;
    }

    // ������ ����
    public void DeleteItem(int index)
    {
        inventoryBlocks[index].GetChild(0).GetComponent<Image>().sprite = null;
        inventoryBlocks[index].GetChild(0).GetComponent<Image>().color = Color.white;
    }
}

// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private int keyID; // Ű ID

    // Ʈ���ſ� �ٸ� ������Ʈ�� ���� �� ó���ϴ� �Լ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            PlayerInventorySystem inven = GetPlayerInventorySystem(collision);
            OpenAndSearchInventory(inven);
        }
    }

    // �÷��̾����� Ȯ���ϴ� �Լ�
    private bool IsPlayer(Collider2D collision)
    {
        return collision.gameObject.layer == (int)LayerName.Player;
    }

    // �÷��̾� �κ��丮 �ý����� �������� �Լ�
    private PlayerInventorySystem GetPlayerInventorySystem(Collider2D collision)
    {
        return collision.GetComponent<PlayerInventorySystem>();
    }

    // �κ��丮�� ���� Ű�� ã�� �Լ�
    private void OpenAndSearchInventory(PlayerInventorySystem inven)
    {
        inven.OpenInven();

        for (int i = 0; i < inven.m_index; i++)
        {
            if (HasKey(inven, i))
            {
                UseKeyAndDestroyDoor(inven, i);
                break;
            }
        }
        UpdateAndCloseInventory(inven);
    }

    // �κ��丮�� Ű�� �ִ��� Ȯ���ϴ� �Լ�
    private bool HasKey(PlayerInventorySystem inven, int index)
    {
        return inven.m_items[index].m_ID == keyID;
    }

    // Ű�� ����ϰ� ���� �����ϴ� �Լ�
    private void UseKeyAndDestroyDoor(PlayerInventorySystem inven, int index)
    {
        inven.UseItem(index);
        inven.CloseInven();
        Destroy(gameObject);
    }

    // �κ��丮�� ������Ʈ�ϰ� �ݴ� �Լ�
    private void UpdateAndCloseInventory(PlayerInventorySystem inven)
    {
        inven.m_playerInven.GetComponent<InventorySystem>().UpdateInventory();
        inven.CloseInven();
    }
}

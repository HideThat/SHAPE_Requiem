using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon; // ������ �������� ǥ���� Image ������Ʈ

    private Item item; // ���� ���Կ� �Ҵ�� ������

    // �������� ���Կ� ����
    public void SetItem(Item newItem)
    {
        item = newItem;

        if (newItem.icon != null)
        {
            icon.sprite = newItem.icon;
            icon.enabled = true;
        }
        else
        {
            Debug.LogWarning("Icon sprite is null!");
            icon.enabled = false;
        }
    }


    // �������� ���Կ��� ����
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // ������ �������� ��� (���������� ���� ����)
    public void UseItem()
    {
        if (item != null)
        {
            // ������ ��� ����
        }
    }
}

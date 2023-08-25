using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item; // ���� ���Կ� �Ҵ�� ������
    public Image icon;

    // �������� ���Կ� ����
    public void SetItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
    }


    // �������� ���Կ��� ����
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
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

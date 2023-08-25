using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item; // 현재 슬롯에 할당된 아이템
    public Image icon;

    // 아이템을 슬롯에 설정
    public void SetItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
    }


    // 아이템을 슬롯에서 제거
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
    }

    // 슬롯의 아이템을 사용 (선택적으로 구현 가능)
    public void UseItem()
    {
        if (item != null)
        {
            // 아이템 사용 로직
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon; // 아이템 아이콘을 표시할 Image 컴포넌트

    private Item item; // 현재 슬롯에 할당된 아이템

    // 아이템을 슬롯에 설정
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


    // 아이템을 슬롯에서 제거
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
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

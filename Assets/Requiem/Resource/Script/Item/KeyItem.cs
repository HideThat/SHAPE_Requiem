using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public Item itemData;
    public BoxCollider2D myCollider;
    public LayerMask layerMask;
    public Collider2D hitInfo;
    public bool isActive = false;

    public bool isCooldown = false; // 상호작용 지연 상태

    private void FixedUpdate()
    {
        if (isCooldown) return; // 지연 시간 동안 상호작용 비활성화

        hitInfo = Physics2D.OverlapBox(transform.position, myCollider.size, 0f, layerMask);

        if (hitInfo != null && !isActive)
        {
            hitInfo.GetComponent<InventorySysyem>().AddItem(itemData);
            isActive = true;
            Destroy(gameObject);
        }
    }

    public IEnumerator InteractionCooldown(float _coolTime)
    {
        isCooldown = true; // 상호작용 지연 시작
        yield return new WaitForSeconds(_coolTime); // 지정된 시간 동안 대기
        isCooldown = false; // 상호작용 지연 종료
    }

    public KeyItem()
    {
        itemData = new Item();
        itemData.itemID = 0;
        itemData.itemName = "Key";
        itemData.itemType = Item.ItemType.Consumable;
    }

}

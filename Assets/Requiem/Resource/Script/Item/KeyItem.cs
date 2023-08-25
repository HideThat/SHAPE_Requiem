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

    public bool isCooldown = false; // ��ȣ�ۿ� ���� ����

    private void FixedUpdate()
    {
        if (isCooldown) return; // ���� �ð� ���� ��ȣ�ۿ� ��Ȱ��ȭ

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
        isCooldown = true; // ��ȣ�ۿ� ���� ����
        yield return new WaitForSeconds(_coolTime); // ������ �ð� ���� ���
        isCooldown = false; // ��ȣ�ۿ� ���� ����
    }

    public KeyItem()
    {
        itemData = new Item();
        itemData.itemID = 0;
        itemData.itemName = "Key";
        itemData.itemType = Item.ItemType.Consumable;
    }

}

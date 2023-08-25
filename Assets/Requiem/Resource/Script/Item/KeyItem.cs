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

    private void FixedUpdate()
    {
        hitInfo = Physics2D.OverlapBox(transform.position, myCollider.size, 0f, layerMask);

        if (hitInfo != null && !isActive)
        {
            hitInfo.GetComponent<InventorySysyem>().AddItem(itemData);
            isActive = true;
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDownCheck : MonoBehaviour
{
    public Slime slime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == slime.gameObject)
        {
            slime.transform.position = new Vector2(slime.transform.position.x, 0f);
            slime.rigid2D.velocity = Vector2.zero;
        }
    }
}

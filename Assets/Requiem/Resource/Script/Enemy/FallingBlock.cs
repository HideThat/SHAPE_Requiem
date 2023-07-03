using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : Enemy_Static
{
    Rigidbody2D rigid;
    Collider2D col;
    public bool isActive = false;
    public bool isRanding = false;

    private void Start()
    {
        damage = 1;
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isActive && !isRanding)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
        else if (!isActive && !isRanding)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else if (isRanding)
        {
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Static;
            col.isTrigger = false;
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    public void SetRanding(bool isRanding)
    {
        this.isRanding = isRanding;
    }

    private void OnDestroy()
    {
        rigid.velocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Static;
        col.isTrigger = false;
    }
}

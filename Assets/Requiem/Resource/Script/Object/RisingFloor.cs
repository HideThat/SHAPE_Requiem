using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RISINGFLOORSTATE
{
    UP,
    DOWN
}

public class RisingFloor : MonoBehaviour
{
    [SerializeField] private float changeSpeed = 3f;
    [SerializeField] public RISINGFLOORSTATE state = RISINGFLOORSTATE.UP;
    [SerializeField] public bool isActive = false;
    [SerializeField] public WallTrigger wallTrigger;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        transform.GetChild(0).parent = null;

        ChangeCollider();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            ChangeSprite();
            ChangeCollider();
        }

        isActive = wallTrigger.isActive;
    }


    void ChangeSprite()
    {
        if (spriteRenderer.size.y > -1f)
        {
            state = RISINGFLOORSTATE.UP;
        }

        switch (state)
        {
            case RISINGFLOORSTATE.UP:
                spriteRenderer.size += -Vector2.up * Time.deltaTime * changeSpeed;
                break;

            case RISINGFLOORSTATE.DOWN:
                spriteRenderer.size += Vector2.up * Time.deltaTime * changeSpeed;
                break;

            default:
                break;
        }
    }

    void ChangeCollider()
    {
        // 콜라이더의 Offset 값을 조정하여 스프라이트의 크기 변경에 따라 항상 꼭대기에 위치하도록 유지
        boxCollider.offset = new Vector2(0f, spriteRenderer.bounds.extents.y);
        boxCollider.size = new Vector2(boxCollider.size.x, -spriteRenderer.size.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RisingFloorTrigger"))
        {
            state++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("RisingFloorTrigger"))
        {
            state++;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}

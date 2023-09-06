using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP;
    public int damage; // 적이 입히는 피해량을 저장하는 변수
    public Collider2D m_collider2D; // 적의 충돌체를 저장하는 변수
    public ColliderType myType;

    public bool runeIn;
    public LayerMask layerMask;

    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxCollider;
    private CircleCollider2D CircleCollider;

    public enum ColliderType
    {
        Box,
        Circle,
        Capsule
    }

    protected virtual void Awake()
    {
        switch (myType)
        {
            case ColliderType.Box:
                boxCollider = GetComponent<BoxCollider2D>();
                break;

            case ColliderType.Circle:
                CircleCollider = GetComponent<CircleCollider2D>();
                break;

            case ColliderType.Capsule:
                capsuleCollider = GetComponent<CapsuleCollider2D>();
                break;

            default:
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        switch (myType)
        {
            case ColliderType.Box:
                if (Physics2D.OverlapBox(transform.position, boxCollider.size, layerMask))
                {
                    runeIn = true;
                }
                else
                {
                    runeIn = false;
                }
                break;

            case ColliderType.Circle:
                if (Physics2D.OverlapCircle(transform.position, CircleCollider.radius, layerMask))
                {
                    runeIn = true;
                }
                else
                {
                    runeIn = false;
                }
                break;

            case ColliderType.Capsule:
                if (Physics2D.OverlapCapsule(transform.position, capsuleCollider.size, CapsuleDirection2D.Vertical, layerMask))
                {
                    runeIn = true;
                }
                else
                {
                    runeIn = false;
                }
                break;

            default:
                break;
        }
        
    }

    public virtual void ResetEnemy()
    {

    }

}

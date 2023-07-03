// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Unity.Mathematics;

public class ArrowScript : Enemy_Dynamic
{
    public bool isActive = false;
    [SerializeField] float mass = 1f;
    [SerializeField] float shootForce = 10f;
    [SerializeField] float disappearTime = 2f;
    [SerializeField] ArrowTrigger arrowTrigger;
    [SerializeField] TrailRenderer trail;
    Rigidbody2D rigid;

    private bool isDestroyed = false;
    Vector2 originPos;
    quaternion originRoate;

    [Serializable]
    public class ResetTrigger
    {
        public bool ShouldReset;
    }

    public ResetTrigger resetTrigger = new ResetTrigger();


    private void Start()
    {
        originPos = transform.position;
        originRoate = transform.rotation;
        damage = 0;
        rigid = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (resetTrigger.ShouldReset)
        {
            ResetEnemy();
            resetTrigger.ShouldReset = false;
        }

        if (isActive == true)
        {
            isActive = false;
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.gravityScale = 0f;
            rigid.angularDrag = 0f;
            rigid.mass = mass;
            arrowTrigger.gameObject.SetActive(false);

            ApplyForceBasedOnRotation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            transform.parent = collision.transform;
            ArrowDestroy();
        }
    }

    private void ApplyForceBasedOnRotation()
    {
        // ���� ��ü�� ȸ������ ������� ���� ���͸� ���
        float rotationInRadians = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad; // ����Ƽ�� �ð���� ȸ���� �����̹Ƿ� ��ȣ�� ����
        Vector2 direction = new Vector2(Mathf.Sin(rotationInRadians), Mathf.Cos(rotationInRadians));

        // ���� �������� ���� ���Ѵ�
        rigid.AddForce(direction.normalized * shootForce, ForceMode2D.Impulse);
    }

    // ȭ�� �ı� �޼ҵ�
    public void ArrowDestroy()
    {
        isDestroyed = true;

        Color endColor = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0f);
        GetComponent<SpriteRenderer>().DOColor(endColor, disappearTime);
        trail.gameObject.SetActive(false);

        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0f;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        Invoke("ColliderFalse", disappearTime);
    }

    void ColliderFalse()
    {
        m_collider2D.enabled = false;
    }

    public override void ResetEnemy()
    {
        // Cancel any existing DOColor tween
        GetComponent<SpriteRenderer>().DOKill();

        transform.position = originPos;
        transform.rotation = originRoate;

        // Reset color to full opacity
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 255f);


        arrowTrigger.gameObject.SetActive(true);
        trail.gameObject.SetActive(true);
        m_collider2D.enabled = true;

        // Make sure the arrow is not active
        isActive = false;

        // Detach from any parent
        transform.parent = null;
    }
}

// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowScript : Enemy_Dynamic
{
    public bool isActive = false;
    [SerializeField] float mass = 1f;
    [SerializeField] float shootForce = 10f;
    [SerializeField] float disappearTime = 2f;
    Rigidbody2D rigid;
    Collider2D colli;
    

    private void Start()
    {
        damage = 0;
        rigid = GetComponent<Rigidbody2D>();
        colli = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isActive == true)
        {
            isActive = false;
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.gravityScale = 0f;
            rigid.angularDrag = 0f;
            rigid.mass = mass;

            ApplyForceBasedOnRotation();

            Invoke("ObjDestroy", 10f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            transform.parent = collision.transform;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;

            ArrowDestroy();
        }
    }

    private void ApplyForceBasedOnRotation()
    {
        // 현재 객체의 회전값을 기반으로 방향 벡터를 계산
        float rotationInRadians = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad; // 유니티는 시계방향 회전이 음수이므로 부호를 반전
        Vector2 direction = new Vector2(Mathf.Sin(rotationInRadians), Mathf.Cos(rotationInRadians));

        // 계산된 방향으로 힘을 가한다
        rigid.AddForce(direction.normalized * shootForce, ForceMode2D.Impulse);
    }

    // 화살 파괴 메소드
    public void ArrowDestroy()
    {
        Color endColor = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0f);
        GetComponent<SpriteRenderer>().DOColor(endColor, disappearTime);
        Invoke("ObjDestroy", disappearTime);
    }

    void ObjDestroy()
    {
        Destroy(gameObject);
    }
}

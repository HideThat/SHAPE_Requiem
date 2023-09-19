using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float force = 10f;
    [SerializeField] Vector2 direction = new Vector2(1f, 1f);
    [SerializeField] EffectDestroy destroyEffect;
    [SerializeField] Transform effectPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            // 이펙트를 생성합니다.
            EffectDestroy effect = Instantiate(destroyEffect);
            effect.transform.position = effectPoint.position;
            effect.transform.localScale = transform.localScale;
            effect.SetDestroy(0.5f);

            // 이 게임 오브젝트를 파괴합니다.
            Destroy(gameObject);
        }
    }

    public void SetBlood(Vector2 _size, Vector2 _dir, float _force)
    {
        StartCoroutine(SetBloodCoroutine(_size, _dir, _force));
    }

    IEnumerator SetBloodCoroutine(Vector2 _size, Vector2 _dir, float _force)
    {
        transform.localScale = _size;
        direction = _dir;
        force = _force;

        // 리지드바디에 힘을 가합니다.
        rigid.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        // 초기 방향에 따라 회전각을 설정합니다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90은 세로로 긴 캡슐이 위를 향하게 하기 위함입니다.

        while (true) 
        {
            // 현재 속도의 방향을 얻습니다.
            Vector2 currentDirection = rigid.velocity;
            angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

            // 회전각을 현재 속도의 방향에 맞춰 갱신합니다.
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90은 세로로 긴 캡슐이 위를 향하게 하기 위함입니다.
            yield return null;
        }
    }

}

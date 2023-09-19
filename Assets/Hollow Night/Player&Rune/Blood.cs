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
            // ����Ʈ�� �����մϴ�.
            EffectDestroy effect = Instantiate(destroyEffect);
            effect.transform.position = effectPoint.position;
            effect.transform.localScale = transform.localScale;
            effect.SetDestroy(0.5f);

            // �� ���� ������Ʈ�� �ı��մϴ�.
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

        // ������ٵ� ���� ���մϴ�.
        rigid.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        // �ʱ� ���⿡ ���� ȸ������ �����մϴ�.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90�� ���η� �� ĸ���� ���� ���ϰ� �ϱ� �����Դϴ�.

        while (true) 
        {
            // ���� �ӵ��� ������ ����ϴ�.
            Vector2 currentDirection = rigid.velocity;
            angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

            // ȸ������ ���� �ӵ��� ���⿡ ���� �����մϴ�.
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90�� ���η� �� ĸ���� ���� ���ϰ� �ϱ� �����Դϴ�.
            yield return null;
        }
    }

}

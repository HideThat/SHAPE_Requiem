// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : Enemy_Dynamic
{
    private void Start()
    {
        m_name = EnemyData.DynamicEnemyNameArr[1];
        damage = 1;
    }

    // ȭ�� ���� �޼ҵ�
    public void SetArrow(float _destroyTime, float _speed, Quaternion _quaternion)
    {
        transform.rotation = _quaternion;

        GetComponent<Rigidbody2D>().velocity = transform.right * _speed; // ȭ���� �ӵ� ����
        Invoke("ArrowDestroy", _destroyTime); // ���� �ð� �� ȭ�� �ı�
    }

    // ȭ�� �ı� �޼ҵ�
    public void ArrowDestroy()
    {
        Destroy(gameObject);
    }
}

// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapScript : Enemy_Static
{
    [SerializeField] Transform firepoint; // �߻� ��ġ
    [SerializeField] GameObject arrow; // ȭ�� ������Ʈ
    [SerializeField] float shootingDelay = 0; // �߻� �ӵ�

    private void Start()
    {
        m_name = EnemyData.StaticEnemyNameArr[2];
        arrow = EnemyData.ProjectileArr[0];
        firepoint = transform.Find("FirePoint");

        if (m_name == null) Debug.Log("name == null");
        if (arrow == null) Debug.Log("Arrow == null");
        if (firepoint == null) Debug.Log("firepoint == null");

        StartCoroutine(Shoot());
    }

    // �ڷ�ƾ�� ����� �߻� �Լ�
    IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingDelay); // �߻� �ӵ���ŭ ���
            Instantiate(arrow, firepoint.position, firepoint.rotation); // ȭ�� �߻�
        }
    }
}

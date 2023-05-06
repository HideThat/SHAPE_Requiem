// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapScript : MonoBehaviour
{
    [SerializeField] Transform firepoint; // �߻� ��ġ
    [SerializeField] GameObject arrow; // ȭ�� ������Ʈ
    [SerializeField] float destroyTime; // ȭ���� ������� �ð�
    [SerializeField] float speed; // ȭ���� �ӵ�
    [SerializeField] float shootingDelay = 0; // �߻� �ӵ�

    private void Start()
    {
        arrow = EnemyData.ProjectileArr[0];
        firepoint = transform.Find("FirePoint");

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
            ArrowScript newArrow = 
                Instantiate(arrow, firepoint.position, firepoint.rotation).GetComponent<ArrowScript>();

            newArrow.SetArrow(destroyTime, speed, transform.rotation);
        }
    }
}

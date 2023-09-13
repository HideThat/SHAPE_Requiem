using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] List<Transform> waveList;
    [SerializeField] List<Transform> pointList;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float waveSpeed = 5.0f; // ���̺� �ӵ�
    [SerializeField] float moveSpeed = 5.0f; // ��ü �ӵ�
    [SerializeField] float decreaseSpeed = 0.5f; // ��ü �ӵ�
    [SerializeField] float destroyTime = 4.0f;

    void Start()
    {
        // �ڷ�ƾ ����
        StartCoroutine(MakeWave());
        SetMoveWave();
    }

    IEnumerator MakeWave()
    {
        while (true)
        {
            // ���̺� ����Ʈ�� ��ü�� ���� �ӵ��� ������ 1:1 �������� ����Ʈ ����Ʈ�� �������� �̵�
            for (int i = 0; i < waveList.Count; i++)
            {
                if (i >= pointList.Count) // pointList�� waveList���� ª�� ��츦 ���
                {
                    break;
                }

                float decreasingFactor = i * decreaseSpeed; // i�� ���� �ӵ� ���� ���
                float step = (waveSpeed - decreasingFactor) * Time.deltaTime; // �ӵ� ���� ����

                waveList[i].position = Vector3.MoveTowards(waveList[i].position, pointList[i].position, step);
            }
            yield return null;
        }
    }


    public void SetMoveWave()
    {
        StartCoroutine(MoveWave());
        StartCoroutine(destroyWave());
    }

    IEnumerator MoveWave()
    {
        while (true) 
        {
            if (transform.rotation.y != 0)
            {
                rigid.velocity = new Vector2(1 * moveSpeed, 0f);
            }
            else
            {
                rigid.velocity = new Vector2(-1 * moveSpeed, 0f);
            }
            yield return null;
        }
    }

    IEnumerator destroyWave()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] List<Transform> waveList;
    [SerializeField] List<Transform> pointList;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float waveSpeed = 5.0f; // 웨이브 속도
    [SerializeField] float moveSpeed = 5.0f; // 객체 속도
    [SerializeField] float decreaseSpeed = 0.5f; // 객체 속도
    [SerializeField] float destroyTime = 4.0f;

    void Start()
    {
        // 코루틴 시작
        StartCoroutine(MakeWave());
        SetMoveWave();
    }

    IEnumerator MakeWave()
    {
        while (true)
        {
            // 웨이브 리스트의 객체가 일정 속도를 가지고 1:1 대응으로 포인트 리스트의 지점으로 이동
            for (int i = 0; i < waveList.Count; i++)
            {
                if (i >= pointList.Count) // pointList가 waveList보다 짧을 경우를 대비
                {
                    break;
                }

                float decreasingFactor = i * decreaseSpeed; // i에 따른 속도 감소 계수
                float step = (waveSpeed - decreasingFactor) * Time.deltaTime; // 속도 감소 적용

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

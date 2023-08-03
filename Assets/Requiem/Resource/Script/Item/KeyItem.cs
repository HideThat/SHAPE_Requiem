// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{
    [SerializeField] public float distance = 1.0f;  // 움직일 거리
    [SerializeField] public float speed = 1.0f;  // 움직임 속도

    private Vector2 startPos;

    void Start()
    {
        gameObject.layer = (int)LayerName.Item; // 아이템 레이어 설정
        m_collider = GetComponent<Collider2D>(); // 자신의 콜라이더
        m_animator = GetComponent<Animator>(); // 자신의 애니매이터
        startPos = transform.position;  // 시작 위치 저장

        if (m_collider == null) Debug.Log("m_collider == null");
        if (m_animator == null) Debug.Log("m_animator == null");
        if (startPos == null) Debug.Log("startPos == null");

        StartCoroutine(MoveUpDown());
    }

    IEnumerator MoveUpDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            // 삼각함수를 이용하여 움직임을 구현
            float newY = startPos.y + Mathf.PingPong(Time.time * speed, distance * 2) - distance;
            transform.position = new Vector3(startPos.x, newY, 0f);
        }
    }

    public void KeyActive(bool _TF)
    {
        gameObject.SetActive(_TF);
    }
}

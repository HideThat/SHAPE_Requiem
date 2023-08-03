// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{
    [SerializeField] public float distance = 1.0f;  // ������ �Ÿ�
    [SerializeField] public float speed = 1.0f;  // ������ �ӵ�

    private Vector2 startPos;

    void Start()
    {
        gameObject.layer = (int)LayerName.Item; // ������ ���̾� ����
        m_collider = GetComponent<Collider2D>(); // �ڽ��� �ݶ��̴�
        m_animator = GetComponent<Animator>(); // �ڽ��� �ִϸ�����
        startPos = transform.position;  // ���� ��ġ ����

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

            // �ﰢ�Լ��� �̿��Ͽ� �������� ����
            float newY = startPos.y + Mathf.PingPong(Time.time * speed, distance * 2) - distance;
            transform.position = new Vector3(startPos.x, newY, 0f);
        }
    }

    public void KeyActive(bool _TF)
    {
        gameObject.SetActive(_TF);
    }
}

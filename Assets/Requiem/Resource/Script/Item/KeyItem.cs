// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{
    void Start()
    {
        gameObject.layer = (int)LayerName.Item; // ������ ���̾� ����
        m_collider = GetComponent<Collider2D>(); // �ڽ��� �ݶ��̴�
        m_animator = GetComponent<Animator>(); // �ڽ��� �ִϸ�����

        if (m_collider == null) Debug.Log("m_collider == null");
        if (m_animator == null) Debug.Log("m_animator == null");
    }

    void Update()
    {
        
    }
}

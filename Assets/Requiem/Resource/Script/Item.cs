using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // ������ ȹ��
    // ���� ȹ�� �� UI �ȳ�

    public int m_ID; // ������ �ϷĹ�ȣ
    public string m_name; // ������ �̸�
    public Sprite m_image;
    public Animator m_animator;
    public string m_explanation;
    protected Collider2D m_collider;
    

    private void Awake()
    {
        
    }
}

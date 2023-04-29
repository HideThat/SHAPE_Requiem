// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dynamic : MonoBehaviour
{
    protected string m_name; // ���ʹ��� �̸�
    protected int damage; // ���ʹ��� ������
    protected Collider2D m_collider2D; // ���ʹ��� �ݶ��̴�

    public virtual void TriggerOn()
    {
        
    }

    public string GetName
    {
        get { return m_name; }
    }

    public int GetDamage
    {
        get { return damage; }
    }
}

// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Static : Enemy
{
    public Vector2 resetPoint; // ���� ������ ��ġ�� �����ϴ� ����

    // ���� Ʈ���Ÿ� Ȱ��ȭ��Ű�� ���� �޼ҵ�
    public virtual void TriggerOn()
    {

    }

    // ���� �̸��� ��ȯ�ϴ� ������Ƽ
    public string GetName
    {
        get { return m_name; }
    }

    // ���� ������ ���ط��� ��ȯ�ϴ� ������Ƽ
    public int GetDamage
    {
        get { return damage; }
    }
}

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

    // ���� ������ ���ط��� ��ȯ�ϴ� ������Ƽ
    public int GetDamage
    {
        get { return damage; }
    }

    public override void Hit(int _demage)
    {
        base.Hit(_demage);
    }
}

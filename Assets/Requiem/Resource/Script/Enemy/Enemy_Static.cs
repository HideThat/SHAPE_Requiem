// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Static : Enemy
{
    public Vector2 resetPoint; // 리셋 지점의 위치를 저장하는 변수

    // 적이 트리거를 활성화시키는 가상 메소드
    public virtual void TriggerOn()
    {

    }

    // 적이 입히는 피해량을 반환하는 프로퍼티
    public int GetDamage
    {
        get { return damage; }
    }

    public override void Hit(int _demage)
    {
        base.Hit(_demage);
    }
}

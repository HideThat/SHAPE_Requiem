// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dynamic : Enemy
{
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

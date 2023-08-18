// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dynamic : Enemy
{
    public int souls;
    public GameObject soulObject;

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

    public virtual void Dead()
    {
        for (int i = 0; i < souls; i++)
        {
            GameObject gameObject = Instantiate(soulObject);
            gameObject.transform.position = transform.position;
        }

        GetComponent<Collider2D>().enabled = false;
    }
}

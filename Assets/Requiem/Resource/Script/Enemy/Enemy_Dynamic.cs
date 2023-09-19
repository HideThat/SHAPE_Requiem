// 1Â÷ ¸®ÆåÅä¸µ

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Enemy_Dynamic : Enemy
{
    public int souls;
    //public GameObject damageBox;
    public GameObject soulObject;

    public virtual void TriggerOn()
    {
        
    }

    public int GetDamage
    {
        get { return damage; }
    }

    public override void Dead()
    {
        for (int i = 0; i < souls; i++)
        {
            GameObject gameObject = Instantiate(soulObject);
            gameObject.transform.position = transform.position;
        }

        GetComponent<Collider2D>().enabled = false;
    }

    public virtual void Hit(int _damage)
    {
        HP -= _damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameActive_Sorcerer : Enemy
{
    public float posColliderDelay;
    public float preColliderDelay;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(ActiveColliderCoroutine());
    }

    IEnumerator ActiveColliderCoroutine()
    {
        yield return new WaitForSeconds(preColliderDelay);
        m_collider2D.enabled = true;

        yield return new WaitForSeconds(posColliderDelay);
        m_collider2D.enabled = false;
    }
}

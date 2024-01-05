using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutoRockMislie : Enemy
{
    public float pointY;
    public TutorialBoss tutorialBoss;

    float originY;

    protected override void Start()
    {
        base.Start();

        originY = transform.position.y;
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Impact();

        base.OnTriggerStay2D(collision);
    }

    void Impact()
    {
        tutorialBoss.checkHit = true;
        m_collider2D.enabled = false;
    }

    public void MoveUp()
    {
        transform.DOMoveY(pointY, 0.5f);
    }

    public void MoveDown()
    {
        transform.DOMoveY(originY, 0.5f);
    }

    public float moveSpeed = 5f; // 전진 속도
    public float moveDuration = 3f; // 전진하는 데 걸리는 시간

    public void MoveFront()
    {
        StartCoroutine(MoveFrontCoroutine());
    }

    IEnumerator MoveFrontCoroutine()
    {
        while (true)
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            yield return null;
        }
    }
}

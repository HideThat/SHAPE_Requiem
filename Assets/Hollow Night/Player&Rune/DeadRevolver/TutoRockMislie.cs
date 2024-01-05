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

    public float moveSpeed = 5f; // ���� �ӵ�
    public float moveDuration = 3f; // �����ϴ� �� �ɸ��� �ð�

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

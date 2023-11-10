using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost_Hand : Enemy
{
    [Header("Ghost_Hand")]
    public float waitTime;
    public Animator animator;
    public EffectDestroy deadEffect;
    public Transform target;

    

    protected override void Start()
    {
        base.Start();

        StartCoroutine(FlipScaleIfNeeded());

        target = PlayerCoroutine.Instance.transform;

        m_collider2D.enabled = false;
        base.Start();

        StartCoroutine(HandActive());
    }

    float scaleX;
    float scaleY;
    private IEnumerator FlipScaleIfNeeded()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;

        while (true)
        {
            if (target.position.x > transform.position.x)
                transform.localScale = new Vector3(-scaleX, scaleY, 1f);
            else
                transform.localScale = new Vector3(scaleX, scaleY, 1f);

            yield return null;
        }
    }

    IEnumerator HandActive()
    {
        yield return new WaitForSeconds(waitTime);
        animator.Play("A_Hand_Active");
        yield return new WaitForSeconds(0.2f);
        m_collider2D.enabled = true;
    }

    public override void Dead()
    {
        base.Dead();

        EffectDestroy effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.5f);

        Destroy(gameObject);
    }
}

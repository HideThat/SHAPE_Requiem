using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("Ghost")]
    public GameObject hand;
    public float handPosY;
    public EffectDestroy deadEffect;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public AudioClip summonClip;
    public AudioClip chargeClip;
    public Transform target;
    public Animator animator;
    public float preSummonHandDelay;
    public float posSummonHandDelay;

    protected override void Start()
    {
        base.Start();
        target = PlayerCoroutine.Instance.transform;
        voiceSource.PlayOneShot(summonClip);
        StartCoroutine(FlipScaleIfNeeded());
        StartCoroutine(FSM());
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
                transform.localScale = new Vector3(scaleX, scaleY, 1f);
            else
                transform.localScale = new Vector3(-scaleX, scaleY, 1f);

            yield return null;
        }
    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return SummonHand();
        }
    }

    IEnumerator SummonHand()
    {
        animator.Play("A_Ghost_SpellCast");
        yield return new WaitForSeconds(preSummonHandDelay);
        GameObject _hand = Instantiate(hand);
        _hand.transform.position = new Vector2(target.position.x, handPosY);
        animator.Play("A_Ghost_Idle");
        yield return new WaitForSeconds(posSummonHandDelay);
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

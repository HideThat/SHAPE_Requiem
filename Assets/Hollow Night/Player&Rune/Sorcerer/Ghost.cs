using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("Ghost")]
    public Sorcerer sorcerer;
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
        _hand.GetComponent<Ghost_Hand>().sorcerer = sorcerer;
        sorcerer.ghost_Hands.Add(_hand.GetComponent<Ghost_Hand>());
        animator.Play("A_Ghost_Idle");
        yield return new WaitForSeconds(posSummonHandDelay);
    }

    

    [Header("Hell Fire")]
    public float speed;
    public float someThreshold;
    public float rotationSpeed = 50f; // ȸ�� �ӵ�, �ʿ信 ���� ����

    public void HellFireReady()
    {
        StopAllCoroutines();
        StartCoroutine(HellFireReadyCoroutine());
    }

    IEnumerator HellFireReadyCoroutine()
    {
        // �Ҽ����� ���� ȸ���� �̵��� ó���ϴ� ����
        while (true)
        {
            // ������ �ӵ��� ��� �ð�������� ȸ��
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            // �Ҽ����� ���� �̵� ó��
            float step = speed * Time.deltaTime; // speed�� �̵� �ӵ�
            transform.position = Vector2.MoveTowards(transform.position, sorcerer.transform.position, step);

            // �Ҽ������� �Ÿ� üũ
            float distanceToSorcerer = Vector2.Distance(transform.position, sorcerer.transform.position);
            if (distanceToSorcerer <= someThreshold) // someThreshold�� ������ �Ÿ�
            {
                Dead();
                break;
            }

            yield return null;
        }
    }

    public override void Dead()
    {
        base.Dead();

        EffectDestroy effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.5f);
        sorcerer.ghosts.Remove(this);

        Destroy(gameObject);
    }
}

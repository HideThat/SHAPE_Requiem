using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Hand_Sweep : Enemy_Dynamic
{
    public ShadowOfTheKing originShadow;
    public float appearDelay;
    public float startMoveDelay;
    public float moveTime;
    public float DisappearDelay;
    private Vector2 origin;

    Tween myTween;

    private void Start()
    {
        origin = transform.position;
    }

    public void Set_Hand_Sweep(Transform _target)
    {
        StartCoroutine(Set_Hand_Sweep_Coroutine(_target));
    }

    IEnumerator Set_Hand_Sweep_Coroutine(Transform _target)
    {
        if (myTween != null)
        {
            DOTween.Kill(myTween);
        }

        Appear_Hand(appearDelay);

        yield return new WaitForSeconds(startMoveDelay);

        DOTween.Kill(myTween);
        MoveTarget(_target, moveTime);

        yield return new WaitForSeconds(moveTime);

        DOTween.Kill(myTween);
        Disappear_Hand_AND_Move_Origin(DisappearDelay);
    }

    void Appear_Hand(float _delay)
    {
        myTween = spriteRenderer.DOColor(Color.white, _delay);
    }

    void MoveTarget(Transform _target, float _moveTime)
    {
        myTween = transform.DOMove(_target.position, _moveTime);
    }

    void Disappear_Hand_AND_Move_Origin(float _delay)
    {
        myTween = spriteRenderer.DOColor(Color.clear, _delay).OnComplete(() =>
        {
            MoveOrigin(origin);
        });
    }

    void MoveOrigin(Vector2 _origin)
    {
        transform.position = _origin;
        gameObject.SetActive(false);
    }

    public override void Hit(int _damage)
    {
        base.Hit(_damage);

        originShadow.HP -= _damage;
    }
}

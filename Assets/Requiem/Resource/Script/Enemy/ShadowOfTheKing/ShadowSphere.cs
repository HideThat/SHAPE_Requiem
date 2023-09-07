using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System;

public class ShadowSphere : Enemy_Dynamic
{
    public float moveTime;
    public float otherHandSweepDelay;

    public ShadowOfTheKing originShadow;
    public SpriteRenderer spriteRenderer;
    public Vacuum_Cleaner Vacuum_Cleaner_R;
    public Vacuum_Cleaner Vacuum_Cleaner_L;
    public Hand_Sweep Hand_Sweep_R;
    public Hand_Sweep Hand_Sweep_L;
    public LaserEye Laser_Eye;

    public Transform hand_Sweep_Target_R;
    public Transform hand_Sweep_Target_L;

    public enum ShadowSphereState
    {
        Idle,
        Move_Vacuum_Cleaner_R,
        Move_Vacuum_Cleaner_L,
        Move_Hand_Sweep_R,
        Move_Hand_Sweep_L,
        Move_Laser_Eye
    }

    private void Start()
    {
        
    }

    public void SetShadowSphere(Transform _target, ShadowSphereState _state)
    {
        Debug.Log("SetShadowSphere 호출됨. 상태: " + _state);

        switch (_state)
        {
            case ShadowSphereState.Move_Vacuum_Cleaner_R:
                StartCoroutine(MoveDownAndHorison(_target, ()=> WakeUp_Vacuum_Cleaner_R()));
                break;

            case ShadowSphereState.Move_Vacuum_Cleaner_L:
                StartCoroutine(MoveDownAndHorison(_target, () => WakeUp_Vacuum_Cleaner_L()));
                break;

            case ShadowSphereState.Move_Hand_Sweep_R:
                StartCoroutine(MoveDownAndHorison(_target, 
                    ()=> WakeUp_Hand_Sweep_R(hand_Sweep_Target_L)));
                StartCoroutine(Start_Other_Hand_Sweep(otherHandSweepDelay, 
                    () => WakeUp_Hand_Sweep_L(hand_Sweep_Target_R)));
                break;

            case ShadowSphereState.Move_Hand_Sweep_L:
                StartCoroutine(MoveDownAndHorison(_target, 
                    ()=> WakeUp_Hand_Sweep_L(hand_Sweep_Target_R)));
                StartCoroutine(Start_Other_Hand_Sweep(otherHandSweepDelay, 
                    () => WakeUp_Hand_Sweep_R(hand_Sweep_Target_L)));
                break;

            case ShadowSphereState.Move_Laser_Eye:
                StartCoroutine(MoveTarget(_target, ()=> WakeUp_Move_Laser_Eye()));
                break;

            default:
                break;
        }
    }

    IEnumerator MoveDownAndHorison(Transform _target, Action _onComplete)
    {
        bool isComplete = false;

        transform.DOMoveY(_target.position.y, moveTime).OnComplete(() =>
        {
            Debug.Log("ShadowSphere Y축 이동 완료");
            isComplete = true;
        });

        yield return new WaitUntil(() => isComplete);

        StartCoroutine(MoveTarget(_target, _onComplete));
    }

    IEnumerator MoveTarget(Transform _target, Action _onComplete)
    {
        bool isComplete = false;

        transform.DOMove(_target.position, moveTime).OnComplete(() =>
        {
            Debug.Log("ShadowSphere 이동 완료");
            isComplete = true;
            DisappearSphere();
        });

        yield return new WaitUntil(() => isComplete);

        _onComplete?.Invoke();
    }

    public void AppearSphere()
    {
        spriteRenderer.DOColor(Color.white, 0.5f).OnComplete(()=>
        {
            m_collider2D.enabled = true;
        });
    }

    public void DisappearSphere()
    {
        spriteRenderer.DOColor(Color.clear, 0.5f).OnComplete(()=>
        {
            m_collider2D.enabled = false;
            transform.position = originShadow.transform.position;
        });
    }

    void WakeUp_Vacuum_Cleaner_R()
    {
        Vacuum_Cleaner_R.gameObject.SetActive(true);
        Vacuum_Cleaner_R.Set_Vacuum_Cleaner();
    }

    void WakeUp_Vacuum_Cleaner_L()
    {
        Vacuum_Cleaner_L.gameObject.SetActive(true);
        Vacuum_Cleaner_L.Set_Vacuum_Cleaner();
    }

    void WakeUp_Hand_Sweep_R(Transform _target)
    {
        Hand_Sweep_R.gameObject.SetActive(true);
        Hand_Sweep_R.Set_Hand_Sweep(_target);
    }

    void WakeUp_Hand_Sweep_L(Transform _target)
    {
        Hand_Sweep_L.gameObject.SetActive(true);
        Hand_Sweep_L.Set_Hand_Sweep(_target);
    }

    IEnumerator Start_Other_Hand_Sweep(float _delay, Action _otherHandSweep)
    {
        yield return new WaitForSeconds(_delay);

        Debug.Log("다른손 시작");
        _otherHandSweep();
    }

    void WakeUp_Move_Laser_Eye()
    {
        Laser_Eye.gameObject.SetActive(true);
        Laser_Eye.Set_Laser_Eye();
    }

    public override void Hit(int _damage)
    {
        base.Hit(_damage);

        originShadow.HP -= _damage;
    }
}

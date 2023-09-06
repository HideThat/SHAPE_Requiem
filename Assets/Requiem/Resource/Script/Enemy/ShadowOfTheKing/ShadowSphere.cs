using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class ShadowSphere : MonoBehaviour
{
    public float moveTime;

    public GameObject Vacuum_Cleaner_R;
    public GameObject Vacuum_Cleaner_L;
    public GameObject Hand_Sweep_R;
    public GameObject Hand_Sweep_L;
    public GameObject Eye_Laser;

    public enum ShadowSphereState
    {
        Idle,
        Move_Vacuum_Cleaner_R,
        Move_Vacuum_Cleaner_L,
        Move_Hand_Sweep_R,
        Move_Hand_Sweep_L,
        Move_Eye_Laser
    }

    private void Start()
    {
        
    }

    public void SetShadowSphere(Transform _target, ShadowSphereState _state)
    {
        switch (_state)
        {
            case ShadowSphereState.Move_Vacuum_Cleaner_R:
                MoveDownAndHorison(_target);
                WakeUp_Vacuum_Cleaner_R();
                break;

            case ShadowSphereState.Move_Vacuum_Cleaner_L:
                MoveDownAndHorison(_target);
                WakeUp_Vacuum_Cleaner_L();
                break;

            case ShadowSphereState.Move_Hand_Sweep_R:
                MoveDownAndHorison(_target);
                WakeUp_Hand_Sweep_R();
                break;

            case ShadowSphereState.Move_Hand_Sweep_L:
                MoveDownAndHorison(_target);
                WakeUp_Hand_Sweep_L();
                break;

            case ShadowSphereState.Move_Eye_Laser:
                MoveTarget(_target);
                break;

            default:
                break;
        }
    }

    void MoveDownAndHorison(Transform _target)
    {
        bool isComplete = false;

        transform.DOMoveY(_target.position.y, moveTime).OnComplete(() =>
        {
            Debug.Log("ShadowSphere Y축 이동 완료");
            isComplete = true;
        });

        while (!isComplete)
        {
            if (isComplete) break;
        }

        if (isComplete)
        {
            MoveTarget(_target);
        }
    }

    void MoveTarget(Transform _target)
    {
        bool isComplete = false;

        transform.DOMove(_target.position, moveTime).OnComplete(() =>
        {
            Debug.Log("ShadowSphere 이동 완료");
            isComplete = true;
        }); ;

        while (!isComplete)
        {
            if (isComplete) break;
        }
    }

    void WakeUp_Vacuum_Cleaner_R()
    {

    }

    void WakeUp_Vacuum_Cleaner_L()
    {

    }

    void WakeUp_Hand_Sweep_R()
    {

    }

    void WakeUp_Hand_Sweep_L()
    {

    }

    void WakeUp_Move_Eye_Laser()
    {

    }
}

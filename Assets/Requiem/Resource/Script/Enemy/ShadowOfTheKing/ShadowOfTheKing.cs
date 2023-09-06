using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShadowSphere;

// 앞으로 해야 할 것들
// 1. 패턴이 끝날 때 isPattern을 false로 전환
// 2. 쉐도우 스피어 로직 구현 완료
// 3. 보스의 나머지 패턴 구현

public class ShadowOfTheKing : Enemy_Dynamic
{
    [Header("ShadowOfTheKing")]
    public ShadowState myState;

    public float idle_Delay;

    public ShadowSphere Shadow_Sphere;
    public GameObject Vacuum_Cleaner_R;
    public GameObject Vacuum_Cleaner_L;
    public GameObject Hand_Sweep_R;
    public GameObject Hand_Sweep_L;
    public GameObject Eye_Laser;
    public GameObject Wraith_Summoner;

    public Transform[] ShadowSphere_Target;

    public bool isPattern = false;

    public enum ShadowState
    {
        Idle,
        Stun,
        Dead,
        Vacuum_Cleaner,
        Summon_Wraith,
        Hand_Sweep,
        Eye_Laser
    }

    private void Update()
    {
        if (isPattern) return;

        switch (myState)
        {
            case ShadowState.Idle:
                ShadowIdle(idle_Delay);
                break;

            case ShadowState.Stun:
                break;

            case ShadowState.Dead:
                isPattern = true;
                break;

            case ShadowState.Summon_Wraith:
                isPattern = true;
                break;

            case ShadowState.Hand_Sweep:
                isPattern = true;
                int rand = Random.Range(0, 1);
                switch (rand)
                {
                    case 0:
                        ChangeShadow_Sphere(ShadowSphere_Target[0], ShadowSphereState.Move_Hand_Sweep_L);
                        break;

                    case 1:
                        ChangeShadow_Sphere(ShadowSphere_Target[1], ShadowSphereState.Move_Hand_Sweep_L);
                        break;

                    default:
                        break;
                }

                break;

            case ShadowState.Eye_Laser:
                isPattern = true;
                ChangeShadow_Sphere(ShadowSphere_Target[2], ShadowSphereState.Move_Eye_Laser);

                break;

            default:
                break;
        }
    }

    void ShadowIdle(float _delay)
    {
        int rand = Random.Range(0, 3);

        while (true)
        {
            if (_delay < 0f)
            {
                switch (rand)
                {
                    case 0:
                        myState = ShadowState.Hand_Sweep;
                        break;

                    case 1:
                        myState = ShadowState.Eye_Laser;
                        break;

                    case 2:
                        myState = ShadowState.Summon_Wraith;
                        break;

                    case 3:
                        myState = ShadowState.Vacuum_Cleaner;
                        break;

                    default:
                        break;
                }
                break;
            }
            else
            {
                _delay -= Time.deltaTime;
            }
        }
    }

    void StunDelay(float _delay)
    {
        while (true)
        {
            if (_delay < 0f)
            {


                break;
            }
            else
            {
                _delay -= Time.deltaTime;
            }
        }
    }

    void ChangeShadow_Sphere(Transform _target, ShadowSphereState _sphereState)
    {
        Shadow_Sphere.gameObject.SetActive(true);
        Shadow_Sphere.SetShadowSphere(_target, _sphereState);
    }
}

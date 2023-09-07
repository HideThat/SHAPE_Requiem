using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShadowSphere;

public class ShadowOfTheKing : Enemy_Dynamic
{
    [Header("ShadowOfTheKing")]
    public ShadowState myState = ShadowState.Idle;

    public float idle_Delay;
    public float pattern_Delay;

    public ShadowSphere Shadow_Sphere;
    public Vacuum_Cleaner Vacuum_Cleaner_R;
    public Vacuum_Cleaner Vacuum_Cleaner_L;
    public Hand_Sweep Hand_Sweep_R;
    public Hand_Sweep Hand_Sweep_L;
    public LaserEye Laser_Eye;
    public WraithSpawner[] wraith_Spawner;

    public Transform[] ShadowSphere_Target;

    public bool isPattern = false;

    private ShadowState lastState = ShadowState.Idle;

    public enum ShadowState
    {
        Idle,
        Stun,
        Dead,
        Vacuum_Cleaner,
        Summon_Wraith,
        Hand_Sweep,
        Laser_Eye
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (isPattern) return;

        StopAllCoroutines();  // 현재 실행 중인 모든 코루틴을 멈춤

        switch (myState)
        {
            case ShadowState.Idle:
                Debug.Log("Entering Idle state.");
                StartCoroutine(ShadowIdleCoroutine(idle_Delay));
                isPattern = true;
                break;

            case ShadowState.Stun:
                break;

            case ShadowState.Dead:
                isPattern = true;
                break;

            case ShadowState.Summon_Wraith:
                Debug.Log("Entering Summon_Wraith state.");
                for (int i = 0; i < wraith_Spawner.Length; i++)
                {
                    wraith_Spawner[i].gameObject.SetActive(true);
                }
                isPattern = true;
                StartCoroutine(PatternDelay(pattern_Delay));
                break;

            case ShadowState.Hand_Sweep:
                Debug.Log("Entering Hand_Sweep state.");
                isPattern = true;
                int rand = Random.Range(0, 1);
                switch (rand)
                {
                    case 0:
                        ChangeShadow_Sphere(ShadowSphere_Target[0], ShadowSphereState.Move_Hand_Sweep_L);
                        break;

                    case 1:
                        ChangeShadow_Sphere(ShadowSphere_Target[1], ShadowSphereState.Move_Hand_Sweep_R);
                        break;

                    default:
                        break;
                }
                StartCoroutine(PatternDelay(pattern_Delay));
                break;

            case ShadowState.Laser_Eye:
                Debug.Log("Entering Laser_Eye state.");
                isPattern = true;
                ChangeShadow_Sphere(ShadowSphere_Target[2], ShadowSphereState.Move_Laser_Eye);
                StartCoroutine(PatternDelay(pattern_Delay));
                break;

            case ShadowState.Vacuum_Cleaner:
                Debug.Log("Entering Vacuum_Cleaner state.");
                isPattern = true;
                int rand1 = Random.Range(0, 1);
                switch (rand1)
                {
                    case 0:
                        ChangeShadow_Sphere(ShadowSphere_Target[0], ShadowSphereState.Move_Vacuum_Cleaner_L);
                        break;

                    case 1:
                        ChangeShadow_Sphere(ShadowSphere_Target[1], ShadowSphereState.Move_Vacuum_Cleaner_R);
                        break;

                    default:
                        break;
                }
                StartCoroutine(PatternDelay(pattern_Delay));
                break;

            default:
                Debug.Log("Entering Unknown state.");
                break;
        }
        lastState = myState;
    }

    IEnumerator ShadowIdleCoroutine(float _delay)
    {
        float elapsed = 0f;
        while (elapsed < _delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                myState = ShadowState.Hand_Sweep;
                break;
            case 1:
                myState = ShadowState.Laser_Eye;
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

        isPattern = false;
    }

    IEnumerator StunDelayCoroutine(float _delay)
    {
        float elapsed = 0f;
        while (elapsed < _delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 여기서 상태를 변경하거나 다른 액션을 수행합니다.
    }

    void ChangeShadow_Sphere(Transform _target, ShadowSphereState _sphereState)
    {
        Shadow_Sphere.AppearSphere();
        Shadow_Sphere.SetShadowSphere(_target, _sphereState);
    }

    IEnumerator PatternDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }
}


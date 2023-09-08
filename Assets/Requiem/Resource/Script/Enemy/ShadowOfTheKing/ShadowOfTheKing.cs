using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShadowSphere;

public class ShadowOfTheKing : Enemy_Dynamic
{
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

    [Header("ShadowOfTheKing")]
    public ShadowState myState = ShadowState.Idle;

    public float idle_Delay;
    public float pattern_Delay;

    public SpriteRenderer spriteRenderer;
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



    private void Start()
    {
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            if (!isPattern)
            {
                

                switch (myState)
                {
                    case ShadowState.Idle:
                        yield return ShadowIdleCoroutine(idle_Delay);
                        break;

                    case ShadowState.Stun:
                        yield return StunDelayCoroutine(1f); // 예시, 실제 지연 시간은 필요에 따라 조정
                        break;

                    case ShadowState.Dead:
                        Dead();
                        yield break;

                    case ShadowState.Summon_Wraith:
                        yield return SummonWraithCoroutine();
                        break;

                    case ShadowState.Hand_Sweep:
                        yield return HandSweepCoroutine();
                        break;

                    case ShadowState.Laser_Eye:
                        yield return LaserEyeCoroutine();
                        break;

                    case ShadowState.Vacuum_Cleaner:
                        yield return VacuumCleanerCoroutine();
                        break;
                }
            }
            yield return null;
        }
    }

    IEnumerator ShadowIdleCoroutine(float _delay)
    {
        

        spriteRenderer.DOColor(Color.red, 1f);
        isPattern = true;
        yield return new WaitForSeconds(_delay);

        ShadowState[] patterns = { ShadowState.Vacuum_Cleaner, ShadowState.Summon_Wraith, ShadowState.Hand_Sweep, ShadowState.Laser_Eye };
        int rand = Random.Range(0, patterns.Length);

        myState = patterns[rand];
        isPattern = false;

        if (HP <= 0)
        {
            myState = ShadowState.Dead;
        }
    }

    IEnumerator StunDelayCoroutine(float _delay)
    {
        float elapsed = 0f;
        while (elapsed < _delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
    }

    IEnumerator SummonWraithCoroutine()
    {
        isPattern = true;
        Debug.Log("Entering Summon_Wraith state.");
        for (int i = 0; i < wraith_Spawner.Length; i++)
        {
            wraith_Spawner[i].HP = 100;
            wraith_Spawner[i].gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(pattern_Delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }

    IEnumerator HandSweepCoroutine()
    {
        isPattern = true;
        Debug.Log("Entering Hand_Sweep state.");
        int rand = Random.Range(0, 2);
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
        yield return new WaitForSeconds(pattern_Delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }

    IEnumerator LaserEyeCoroutine()
    {
        isPattern = true;
        Debug.Log("Entering Laser_Eye state.");
        ChangeShadow_Sphere(ShadowSphere_Target[2], ShadowSphereState.Move_Laser_Eye);
        yield return new WaitForSeconds(pattern_Delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }

    IEnumerator VacuumCleanerCoroutine()
    {
        isPattern = true;
        Debug.Log("Entering Vacuum_Cleaner state.");
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
        yield return new WaitForSeconds(pattern_Delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }

    void ChangeShadow_Sphere(Transform _target, ShadowSphereState _sphereState)
    {
        spriteRenderer.DOColor(Color.clear, 1f);
        Shadow_Sphere.AppearSphere();
        Shadow_Sphere.SetShadowSphere(_target, _sphereState);
    }

    IEnumerator PatternDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        myState = ShadowState.Idle;
        isPattern = false;
    }

    public override void Dead()
    {
        base.Dead();

        gameObject.SetActive(false);
        StopAllCoroutines();
    }
}


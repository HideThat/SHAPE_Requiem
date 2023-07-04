using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StoneRollingTrigger : MonoBehaviour
{
    [SerializeField] private Switch m_switch;
    [SerializeField] Switch platformSwich1;
    [SerializeField] Switch platformSwich2;
    [SerializeField] Object_Trigger rollingStoneTrigger;
    [SerializeField] RollingStone rollingStone;
    [SerializeField] CinemachineVirtualCamera mainCM;
    [SerializeField] WarpDoor[] warpDoors;

    [SerializeField] bool isActive = false;
    [SerializeField] bool isChange = false;
    private bool prevIsActive = false;

    private Coroutine platformActiveRoutine;
    private Coroutine stoneActiveRoutine;

    public Tween myTween;

    private CinemachineConfiner2D mainCMConfiner;

    void Start()
    {
        mainCM = DataController.MainCM;
        mainCMConfiner = mainCM.GetComponent<CinemachineConfiner2D>();
    }

    void Update()
    {
        CheckSwitch();
        SetWarpDoors();
        HandleCameraTransition();
    }

    private void CheckSwitch()
    {
        if (isActive != m_switch.isActive && !isChange)
        {
            isActive = m_switch.isActive;
            isChange = true;
        }
    }

    private void SetWarpDoors()
    {
        if (!isActive || warpDoors == null)
            return;

        foreach (var item in warpDoors)
        {
            if (!item.isOpened)
            {
                item.isOpened = true;
            }
        }
    }

    private void HandleCameraTransition()
    {
        if (isActive)
        {
            if (!prevIsActive)
            {
                BeginCameraTransition();
            }

            DivAreaManager.Instance.DivAreaActive(false);
            StartCoroutine(TweenControl());
        }

        prevIsActive = isActive;
    }

    IEnumerator TweenControl()
    {
        myTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, 10f, 4f);

        yield return new WaitForSeconds(4f);

        isActive = false;
        myTween.Kill();
    }

    private void BeginCameraTransition()
    {
        StopActiveRoutines();

        platformActiveRoutine = StartCoroutine(PlatformActiveAfterDelay(2f));
        stoneActiveRoutine = StartCoroutine(StoneActiveAfterDelay(4f));

        mainCMConfiner.enabled = false;
        mainCM.Follow = rollingStone.transform;
    }

    private void StopActiveRoutines()
    {
        if (platformActiveRoutine != null)
        {
            StopCoroutine(platformActiveRoutine);
        }

        if (stoneActiveRoutine != null)
        {
            StopCoroutine(stoneActiveRoutine);
        }
    }

    private IEnumerator PlatformActiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        platformSwich1.isActive = true;
        platformSwich2.isActive = true;
    }

    private IEnumerator StoneActiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rollingStoneTrigger.ActiveTrigger();
    }
}

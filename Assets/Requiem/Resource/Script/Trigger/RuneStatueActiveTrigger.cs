using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class RuneStatueActiveTrigger : MonoBehaviour
{
    [SerializeField] RuneStatue runeStatue;
    [SerializeField] CinemachineVirtualCamera cinemachine;

    private void Update()
    {
        if (runeStatue.isActive)
        {
            StartCoroutine(TriggerActive(2f));
        }
    }

    IEnumerator TriggerActive(float _changeTime)
    {
        DOTween.To(() => cinemachine.m_Lens.OrthographicSize, x => cinemachine.m_Lens.OrthographicSize = x, 8f, _changeTime);

        yield return new WaitForSeconds(_changeTime);

        this.enabled = false;
    }
}

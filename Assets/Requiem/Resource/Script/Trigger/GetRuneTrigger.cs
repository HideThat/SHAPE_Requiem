using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GetRuneTrigger : MonoBehaviour
{
    [SerializeField] float DeadDelay;
    [SerializeField] SceneChangeTrigger changeTrigger;

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !RuneControllerGPT.Instance.m_isGetRune)
        {
            StartCoroutine(PlayerIn(collision.GetComponent<HP_SystemGPT>(), DeadDelay));
        }
    }

    IEnumerator PlayerIn(HP_SystemGPT _hpSystem,  float deadDelay)
    {
        yield return new WaitForSeconds(deadDelay);

        _hpSystem.Dead(DeadDelay);

        yield return new WaitForSeconds(0.5f);

        _hpSystem.GetComponent<NewGameStart>().enabled = true;
        StartCoroutine(changeTrigger.FadeOutAndLoadScene());
    }
}

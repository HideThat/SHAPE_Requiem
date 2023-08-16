using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] string nextScene;
    [SerializeField] string contactObjectName;
    [SerializeField] float fadeOutTime;
    [SerializeField] Vector2 nextPlayerPos;
    [SerializeField] Quaternion nextPlayerRot;
    [SerializeField] Vector2 nextRunePos;
    [SerializeField] Quaternion nextRuneRot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(contactObjectName))
        {
            Debug.LogError("contactObjectName이 설정되지 않았습니다.");
            return;
        }

        if (collision == null || string.IsNullOrEmpty(collision.tag))
        {
            Debug.LogWarning("충돌한 객체의 태그가 없습니다.");
            return;
        }

        if (collision.tag != contactObjectName)
        {
            Debug.LogWarning("충돌한 객체의 태그가 contactObjectName과 일치하지 않습니다.");
            return;
        }

        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem 인스턴스가 없습니다. 위치 및 안개 데이터를 저장할 수 없습니다.");
            return;
        }

        SaveSystem.Instance.SetPlayerNextPos(nextPlayerPos, nextPlayerRot);
        SaveSystem.Instance.SetRuneNextPos(nextRunePos);

        Func<List<Transform>> fogListFunc = FogOfWar.fogList;
        if (fogListFunc != null)
        {
            List<Transform> fogList = fogListFunc();
            if (fogList != null)
            {
                SaveSystem.Instance.SetSceneFogData(fogList);
            }
            else
            {
                Debug.LogWarning("안개 목록이 null입니다. 안개 데이터를 저장할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("FogOfWar.fogList 함수가 설정되지 않았습니다.");
        }

        Debug.Log("모든 데이터가 저장되었습니다. FadeOutAndLoadScene 코루틴을 시작합니다.");
        StartCoroutine(FadeOutAndLoadScene());
    }


    IEnumerator FadeOutAndLoadScene()
    {
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOut(fadeOutTime);
        }
        else
        {
            Debug.LogError("FadeManager 인스턴스가 없습니다.");
        }

        yield return new WaitForSeconds(fadeOutTime);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("nextScene이 설정되지 않았습니다.");
            yield break;
        }

        //if (!SceneManager.GetSceneByName(nextScene).IsValid())
        //{
        //    Debug.LogError($"씬 {nextScene}을 찾을 수 없습니다.");
        //    yield break;
        //}

        SceneManager.LoadScene(nextScene);
    }
}

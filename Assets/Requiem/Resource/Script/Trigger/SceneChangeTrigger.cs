using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] public string nextScene;
    [SerializeField] public string contactObjectName;
    [SerializeField] public float fadeOutTime;
    [SerializeField] public Vector2 nextPlayerPos;
    [SerializeField] public Quaternion nextPlayerRot;
    [SerializeField] public Vector2 nextRunePos;
    public Quaternion nextRuneRot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsCollisionValid(collision))
        {
            SaveFogData();
            SaveMovablePlatformData();
            Debug.Log("모든 데이터가 저장되었습니다. FadeOutAndLoadScene 코루틴을 시작합니다.");
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private bool IsCollisionValid(Collider2D collision)
    {
        return IsContactObjectNameValid() && IsCollisionTagValid(collision);
    }

    private bool IsContactObjectNameValid()
    {
        if (string.IsNullOrEmpty(contactObjectName))
        {
            Debug.LogError("contactObjectName이 설정되지 않았습니다.");
            return false;
        }
        return true;
    }

    private bool IsCollisionTagValid(Collider2D collision)
    {
        if (collision == null || string.IsNullOrEmpty(collision.tag) || collision.tag != contactObjectName)
        {
            Debug.LogWarning("충돌한 객체의 조건이 유효하지 않습니다.");
            return false;
        }
        return true;
    }

    private void SaveFogData()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem 인스턴스가 없습니다. 위치 및 안개 데이터를 저장할 수 없습니다.");
            return;
        }

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
    }
    private void SaveMovablePlatformData()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem 인스턴스가 없습니다. 플랫폼 데이터를 저장할 수 없습니다.");
            return;
        }

        List<MovablePlatform> movablePlatforms = DataController.MovablePlatformList;
        if (movablePlatforms != null)
        {
            SaveSystem.Instance.SetSceneMovablePlatformData(movablePlatforms);
        }
        else
        {
            Debug.LogWarning("movablePlatforms 목록이 null입니다. 플랫폼 데이터를 저장할 수 없습니다.");
        }
    }


    public IEnumerator FadeOutAndLoadScene()
    {
        PerformFadeOut();
        yield return new WaitForSeconds(fadeOutTime);
        LoadNextScene();
    }

    private void PerformFadeOut()
    {
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOut(fadeOutTime);
        }
        else
        {
            Debug.LogError("FadeManager 인스턴스가 없습니다.");
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("nextScene이 설정되지 않았습니다.");
            return;
        }

        SaveSystem.Instance.SetPlayerNextPos(nextPlayerPos, nextPlayerRot);
        SaveSystem.Instance.SetRuneNextPos(nextRunePos);

        //if (!SceneManager.GetSceneByName(nextScene).IsValid())
        //{
        //    Debug.LogError($"씬 {nextScene}을 찾을 수 없습니다.");
        //    return;
        //}

        SceneManager.LoadScene(nextScene);
    }
}

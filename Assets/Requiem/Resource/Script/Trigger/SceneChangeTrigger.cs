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

    public IEnumerator FadeOutAndLoadScene()
    {
        LoadNextScene();
        yield return new WaitForSeconds(fadeOutTime);
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

        SceneManager.LoadScene(nextScene);
    }
}

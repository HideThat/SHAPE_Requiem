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
            Debug.LogError("contactObjectName�� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (collision == null || string.IsNullOrEmpty(collision.tag))
        {
            Debug.LogWarning("�浹�� ��ü�� �±װ� �����ϴ�.");
            return;
        }

        if (collision.tag != contactObjectName)
        {
            Debug.LogWarning("�浹�� ��ü�� �±װ� contactObjectName�� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem �ν��Ͻ��� �����ϴ�. ��ġ �� �Ȱ� �����͸� ������ �� �����ϴ�.");
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
                Debug.LogWarning("�Ȱ� ����� null�Դϴ�. �Ȱ� �����͸� ������ �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("FogOfWar.fogList �Լ��� �������� �ʾҽ��ϴ�.");
        }

        Debug.Log("��� �����Ͱ� ����Ǿ����ϴ�. FadeOutAndLoadScene �ڷ�ƾ�� �����մϴ�.");
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
            Debug.LogError("FadeManager �ν��Ͻ��� �����ϴ�.");
        }

        yield return new WaitForSeconds(fadeOutTime);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("nextScene�� �������� �ʾҽ��ϴ�.");
            yield break;
        }

        //if (!SceneManager.GetSceneByName(nextScene).IsValid())
        //{
        //    Debug.LogError($"�� {nextScene}�� ã�� �� �����ϴ�.");
        //    yield break;
        //}

        SceneManager.LoadScene(nextScene);
    }
}

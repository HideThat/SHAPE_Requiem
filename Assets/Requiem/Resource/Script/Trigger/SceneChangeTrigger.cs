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
            Debug.Log("��� �����Ͱ� ����Ǿ����ϴ�. FadeOutAndLoadScene �ڷ�ƾ�� �����մϴ�.");
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
            Debug.LogError("contactObjectName�� �������� �ʾҽ��ϴ�.");
            return false;
        }
        return true;
    }

    private bool IsCollisionTagValid(Collider2D collision)
    {
        if (collision == null || string.IsNullOrEmpty(collision.tag) || collision.tag != contactObjectName)
        {
            Debug.LogWarning("�浹�� ��ü�� ������ ��ȿ���� �ʽ��ϴ�.");
            return false;
        }
        return true;
    }

    private void SaveFogData()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem �ν��Ͻ��� �����ϴ�. ��ġ �� �Ȱ� �����͸� ������ �� �����ϴ�.");
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
                Debug.LogWarning("�Ȱ� ����� null�Դϴ�. �Ȱ� �����͸� ������ �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("FogOfWar.fogList �Լ��� �������� �ʾҽ��ϴ�.");
        }
    }
    private void SaveMovablePlatformData()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem �ν��Ͻ��� �����ϴ�. �÷��� �����͸� ������ �� �����ϴ�.");
            return;
        }

        List<MovablePlatform> movablePlatforms = DataController.MovablePlatformList;
        if (movablePlatforms != null)
        {
            SaveSystem.Instance.SetSceneMovablePlatformData(movablePlatforms);
        }
        else
        {
            Debug.LogWarning("movablePlatforms ����� null�Դϴ�. �÷��� �����͸� ������ �� �����ϴ�.");
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
            Debug.LogError("FadeManager �ν��Ͻ��� �����ϴ�.");
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("nextScene�� �������� �ʾҽ��ϴ�.");
            return;
        }

        SaveSystem.Instance.SetPlayerNextPos(nextPlayerPos, nextPlayerRot);
        SaveSystem.Instance.SetRuneNextPos(nextRunePos);

        //if (!SceneManager.GetSceneByName(nextScene).IsValid())
        //{
        //    Debug.LogError($"�� {nextScene}�� ã�� �� �����ϴ�.");
        //    return;
        //}

        SceneManager.LoadScene(nextScene);
    }
}

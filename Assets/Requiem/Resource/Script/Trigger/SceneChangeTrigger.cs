using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string contactObjectName;
    [SerializeField] float fadeOutTime;
    [SerializeField] Vector2 nextPlayerPos;
    [SerializeField] Quaternion nextPlayerRot;
    [SerializeField] Vector2 nextRunePos;
    [SerializeField] Quaternion nextRuneRot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == contactObjectName)
        {
            SaveSystem.Instance.SetPlayerNextPos(nextPlayerPos, nextPlayerRot);
            SaveSystem.Instance.SetRuneNextPos(nextRunePos);
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    IEnumerator FadeOutAndLoadScene()
    {
        FadeManager.Instance.FadeOut(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);

        SceneManager.LoadScene(sceneName);
    }
}

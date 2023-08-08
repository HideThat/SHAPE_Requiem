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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == contactObjectName)
        {
            SaveSystem.Instance.SetPlayerData(nextPlayerPos, nextPlayerRot);
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

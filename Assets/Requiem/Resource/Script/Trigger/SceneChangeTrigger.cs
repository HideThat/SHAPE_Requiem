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

    [SerializeField] SceneData sceneData;
    [SerializeField] SavePlayerData playerData;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == contactObjectName)
        {
            sceneData.SaveScene();
            playerData.SetData(nextPlayerPos, nextPlayerRot);
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

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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == contactObjectName)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    IEnumerator FadeOutAndLoadScene()
    {
        FadeManager.Instance.FadeOut(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);

        // Fully opaque, load the scene
        SceneManager.LoadScene(sceneName);
    }
}

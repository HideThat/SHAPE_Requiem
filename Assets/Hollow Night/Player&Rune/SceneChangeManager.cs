using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    public float waitTime;
    public string loadingSceneName;
    

    Coroutine sceneChangeCoroutine;

    public void SceneChange(SceneName _sceneName)
    {
        if (sceneChangeCoroutine != null) StopCoroutine(sceneChangeCoroutine);
        Sound_Manager.Instance.PlayBGM(0);

        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(Timer.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        sceneChangeCoroutine = StartCoroutine(SceneChangeCoroutine(_sceneName));
    }

    public void SceneChangeNoDoor(SceneName _sceneName)
    {
        if (sceneChangeCoroutine != null) StopCoroutine(sceneChangeCoroutine);
        Sound_Manager.Instance.PlayBGM(0);

        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(Timer.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        sceneChangeCoroutine = StartCoroutine(SceneChangeCoroutineNoDoor(_sceneName));
    }

    public void SceneChange(Scene _scene)
    {
        if (sceneChangeCoroutine != null) StopCoroutine(sceneChangeCoroutine);
        Sound_Manager.Instance.PlayBGM(0);

        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(Timer.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        sceneChangeCoroutine = StartCoroutine(SceneChangeCoroutine(_scene));
    }

    IEnumerator SceneChangeCoroutine(SceneName _sceneName)
    {
        // 뭔가 연출들
        yield return StartCoroutine(SceneChangeDoor.Instance.FadeIn());
        // 비동기 씬 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName.ToString());
        yield return StartCoroutine(SceneChangeDoor.Instance.DoorClose());

        yield return new WaitForSeconds(waitTime);

        while (!asyncLoad.isDone)
        {
            // 여기서 필요하면 로딩 프로그레스 바나 다른 UI를 업데이트 할 수 있습니다.
            yield return null;
        }
        StartCoroutine(SceneChangeDoor.Instance.FadeOut());
        yield return StartCoroutine(SceneChangeDoor.Instance.DoorOpen());
    }

    IEnumerator SceneChangeCoroutineNoDoor(SceneName _sceneName)
    {
        // 뭔가 연출들
        yield return StartCoroutine(SceneChangeDoor.Instance.FadeIn());
        // 비동기 씬 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName.ToString());

        while (!asyncLoad.isDone)
        {
            // 여기서 필요하면 로딩 프로그레스 바나 다른 UI를 업데이트 할 수 있습니다.
            yield return null;
        }
        
        yield return StartCoroutine(SceneChangeDoor.Instance.FadeOut());
    }

    IEnumerator SceneChangeCoroutine(Scene _scene)
    {
        // 뭔가 연출들
        yield return StartCoroutine(SceneChangeDoor.Instance.FadeIn());
        // 비동기 씬 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene.name);
        yield return StartCoroutine(SceneChangeDoor.Instance.DoorClose());

        yield return new WaitForSeconds(waitTime);

        while (!asyncLoad.isDone)
        {
            // 여기서 필요하면 로딩 프로그레스 바나 다른 UI를 업데이트 할 수 있습니다.
            yield return null;
        }
        StartCoroutine(SceneChangeDoor.Instance.FadeOut());
        yield return StartCoroutine(SceneChangeDoor.Instance.DoorOpen());
    }
}

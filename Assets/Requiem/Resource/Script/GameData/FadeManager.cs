using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] public float defaultFadeDuration = 1f;
    [SerializeField] public float defaultWaitDuration = 1f;

    private Coroutine currentFadeCoroutine;



    private static FadeManager instance;
    public static FadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FadeManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("FadeManager");
                    instance = obj.AddComponent<FadeManager>();
                }
            }
            return instance;
        }
    }



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        fadeImage = GetComponent<Image>();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FadeManager))]
    public class FadeManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FadeManager fadeManager = Instance;

            EditorGUILayout.Space();

            if (GUILayout.Button("Fade In"))
            {
                fadeManager.FadeIn();
            }

            if (GUILayout.Button("Fade Out"))
            {
                fadeManager.FadeOut();
            }

            if (GUILayout.Button("Fade Out and In"))
            {
                fadeManager.FadeOutAndIn();
            }
        }
    }
#endif

    public void FadeIn()
    {
        FadeIn(defaultFadeDuration);
    }

    public void FadeOut()
    {
        FadeOut(defaultFadeDuration);
    }

    public void FadeOutAndIn()
    {
        FadeOutAndIn(defaultFadeDuration, defaultWaitDuration);
    }

    public void FadeIn(float fadeDuration)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(Fade(FadeType.In, fadeDuration));
    }

    public void FadeOut(float fadeDuration)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(Fade(FadeType.Out, fadeDuration));
    }

    public void FadeOutAndIn(float fadeDuration, float waitDuration)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeOutAndInRoutine(fadeDuration, waitDuration));
    }

    private IEnumerator Fade(FadeType fadeType, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = fadeType == FadeType.In ? Color.clear : Color.black;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            fadeImage.color = Color.Lerp(startColor, targetColor, normalizedTime);
            yield return null;
        }

        fadeImage.color = targetColor;
    }

    private IEnumerator FadeOutAndInRoutine(float fadeDuration, float waitDuration)
    {
        yield return StartCoroutine(Fade(FadeType.Out, fadeDuration));
        yield return new WaitForSeconds(waitDuration);
        yield return StartCoroutine(Fade(FadeType.In, fadeDuration));
    }

    private enum FadeType
    {
        In,
        Out
    }
}

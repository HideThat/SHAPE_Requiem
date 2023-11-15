using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ClearUI : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI timer;
    public Image[] images;
    public TextMeshProUGUI[] texts;
    
    public Image retryButton;
    public TextMeshProUGUI retryButtonText;
    public Image quitButton;
    public TextMeshProUGUI quitButtonText;

    public Color panelChangeColor;

    public float waitTime;
    public float changeTime;

    public string firstStageSceneName;
    public string titleSceneName;

    void Start()
    {
        timer.text = Timer.Instance.OutTimeFormat();

        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0f);

        foreach (var item in images)
            item.color = new Color(item.color.r, item.color.g, item.color.b, 0f);

        foreach (var item in texts)
            item.color = new Color(item.color.r, item.color.g, item.color.b, 0f);

        retryButton.color = new Color(retryButton.color.r, retryButton.color.g, retryButton.color.b, 0f);
        retryButtonText.color = new Color(retryButtonText.color.r, retryButtonText.color.g, retryButtonText.color.b, 0f);
        quitButton.color = new Color(quitButton.color.r, quitButton.color.g, quitButton.color.b, 0f);
        quitButtonText.color = new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, 0f);

        StartCoroutine(ClearUI_Coroutine());
    }

    IEnumerator ClearUI_Coroutine()
    {
        retryButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTime);
        panel.DOColor(panelChangeColor, changeTime);

        foreach (var item in images)
            item.DOColor(new Color(item.color.r, item.color.g, item.color.b, 1f), changeTime);

        foreach (var item in texts)
            item.DOColor(new Color(item.color.r, item.color.g, item.color.b, 1f), changeTime);

        yield return new WaitForSeconds(3f);
        retryButton.gameObject.SetActive(true);
        retryButton.DOColor(new Color(retryButton.color.r, retryButton.color.g, retryButton.color.b, 1f), changeTime);
        retryButtonText.DOColor(new Color(retryButtonText.color.r, retryButtonText.color.g, retryButtonText.color.b, 1f), changeTime);
        yield return new WaitForSeconds(1.5f);
        quitButton.gameObject.SetActive(true);
        quitButton.DOColor(new Color(quitButton.color.r, quitButton.color.g, quitButton.color.b, 1f), changeTime);
        quitButtonText.DOColor(new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, 1f), changeTime);
    }

    public void Retry()
    {
        Destroy(Timer.Instance.gameObject);
        GameInGameData.Instance.ResetPlayerHP();
        SceneChangeManager.Instance.SceneChange(firstStageSceneName);
    }

    public void GoTitle()
    {
        Destroy(Timer.Instance.gameObject);
        SceneChangeManager.Instance.SceneChange(titleSceneName);
    }
}

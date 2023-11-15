using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class StageClearUI : Singleton<StageClearUI>
{
    public GameObject panel;
    public Image[] images;
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI clearTimeText;
    public Image bossImage;
    public Button ReStartButton;
    public Button QuitButton;
    public Image[] stars;
    public string titleSceneName;

    Color[] imageCurrentColors;
    Color[] textCurrentColors;

    protected override void Awake()
    {
        base.Awake();

        imageCurrentColors = new Color[images.Length];
        textCurrentColors= new Color[texts.Length];

        for (int i = 0; i < images.Length; i++)
            imageCurrentColors[i] = images[i].color;
        for (int i = 0; i < texts.Length; i++)
            textCurrentColors[i] = texts[i].color;
    }

    public void OpenStageClearUI(string _clearTime, string _bossName)
    {
        panel.SetActive(true);

        for (int i = 0; i < images.Length; i++)
            images[i].DOColor(imageCurrentColors[i], 0.5f);
        for (int i = 0; i < texts.Length; i++)
            texts[i].DOColor(textCurrentColors[i], 0.5f);

        clearTimeText.text = _clearTime;
        bossNameText.text = _bossName;
        StartCoroutine(GetStarCoroutine());
    }

    IEnumerator GetStarCoroutine()
    {
        for (int i = 0; i < GameInGameData.Instance.GetStarCount(); i++)
        {
            stars[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    public void ClosStageClearUI()
    {
        ResetStageClearUI();
        panel.SetActive(false);
    }

    private void ResetStageClearUI()
    {
        DOTween.KillAll(this);
        StopAllCoroutines();

        foreach (var item in stars)
            item.gameObject.SetActive(false);
        foreach (var item in images)
            item.color = Color.clear;
        foreach (var item in texts)
            item.color = Color.clear;

        bossNameText.text = new string("Boss Name");
        clearTimeText.text = new string("00:00");
        bossImage.sprite = null;
    }

    public void RestartButtonClick()
    {
        ClosStageClearUI();
        SceneChangeManager.Instance.SceneChange(GameInGameData.Instance.currentStageBossName);
    }

    public void QuitButtonClick()
    {
        ClosStageClearUI();
        SceneChangeManager.Instance.SceneChange(titleSceneName);
    }
}

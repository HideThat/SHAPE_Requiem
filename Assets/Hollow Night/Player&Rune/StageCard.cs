using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StageCard : MonoBehaviour
{
    public BossName bossName;
    public SceneName beforeSceneName;
    BossData bossData;
    SceneName sceneName;
    string clearTime;
    int getStar;
    int starNeedToOpen;
    [SerializeField] GameObject lockPanel;
    [SerializeField] TextMeshProUGUI clearTimeText;
    [SerializeField] TextMeshProUGUI bossNameText;
    [SerializeField] TextMeshProUGUI totalStarText;
    [SerializeField] Image[] stars;

    void Start()
    {
        InitStageCard();
        bossNameText.text = bossName.ToString();

        clearTimeText.text = clearTime;
        totalStarText.text = $"¡Ú X {starNeedToOpen}";

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == getStar) break;

            stars[i].gameObject.SetActive(true);
        }
    }

    public void GoToScene()
    {
        if (starNeedToOpen > GameInGameData.Instance.totalStar) return;
        GameInGameData.Instance.ResetPlayerHP();
        GameInGameData.Instance.currentSceneName = sceneName;
        GameInGameData.Instance.beforeSceneName = beforeSceneName;
        GameInGameData.Instance.currentStageBossName = bossName;
        SceneChangeManager.Instance.SceneChangeNoDoor(sceneName);
    }

    void InitStageCard()
    {
        bossData = GameInGameData.Instance.GetBossData(bossName);
        sceneName = bossData.SceneName;
        clearTime = bossData.ClearTime;
        getStar = bossData.GetStar;

        starNeedToOpen = bossData.starNeedToOpen;

        if (starNeedToOpen <= GameInGameData.Instance.totalStar) lockPanel.SetActive(false);
    }
}

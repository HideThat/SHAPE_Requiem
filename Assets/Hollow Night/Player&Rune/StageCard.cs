using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



public class StageCard : MonoBehaviour
{
    public string BossName;
    public string SceneName;
    public string ClearTime;
    public int GetStar;
    public int starNeedToOpen;
    public string beforeSceneName;
    [SerializeField] GameObject LockPanel;
    [SerializeField] TextMeshProUGUI clearTimeText;
    [SerializeField] TextMeshProUGUI BossNameText;
    [SerializeField] TextMeshProUGUI totalStarText;
    [SerializeField] Image[] stars;

    void Start()
    {
        GameInGameData.Instance.ChangeStageCard(this);
        if (starNeedToOpen <= GameInGameData.Instance.totalStar) LockPanel.SetActive(false);
        BossNameText.text = BossName;

        clearTimeText.text = ClearTime;
        totalStarText.text = $"¡Ú X {starNeedToOpen}";

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == GetStar) break;

            stars[i].gameObject.SetActive(true);
        }
    }

    public void GoToScene()
    {
        if (starNeedToOpen > GameInGameData.Instance.totalStar) return;
        GameInGameData.Instance.ResetPlayerHP();
        GameInGameData.Instance.currentSceneName = SceneName;
        GameInGameData.Instance.beforeSceneName = beforeSceneName;
        StageClearUI.Instance.beforeSceneName = beforeSceneName;
        GameInGameData.Instance.currentStageBossName = BossName;
        SceneChangeManager.Instance.SceneChangeNoDoor(SceneName);
    }
}

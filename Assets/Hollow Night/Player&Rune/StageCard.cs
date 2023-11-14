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
    [SerializeField] TextMeshProUGUI clearTimeText;
    [SerializeField] TextMeshProUGUI BossNameText;
    [SerializeField] Image[] stars;

    void Start()
    {
        GameInGameData.Instance.ChangeStageCard(this);
        BossNameText.text = BossName;

        clearTimeText.text = ClearTime;

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == GetStar) break;

            stars[i].gameObject.SetActive(true);
        }
    }

    public void GoToScene()
    {
        SceneChangeManager.Instance.SceneChange(SceneName);
    }
}

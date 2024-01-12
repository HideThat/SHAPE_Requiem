using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BossName
{
    None,
    Tutorial,
    Spider,
    Wendigo,
    Demon,
    CrazyMonk,
    BigSlime,
    Sorcerer,
    Level_1,
    Level_2,
    TheTower
}
public enum SceneName
{
    None,
    Title,
    OptionScene,
    ModeChoice,
    SingleStageUI,
    BossRelayStageUI,
    Tutorial,
    Spider,
    Wendigo,
    Demon,
    CrazyMonk,
    Slime,
    Sorcerer,
    Level_1,
    Level_1_1,
    Level_1_2,
    Level_2,
    Level_2_1,
    Level_2_2,
    Tower_1,
    Tower_2,
    Tower_3,
    Tower_4,
    Tower_5,
    Tower_6
}

[Serializable]
public class BossData
{
    public BossName BossName; // 이넘으로 교체
    public Sprite BossImage;
    public SceneName SceneName; // 이넘으로 교체
    public BossName NextBossName; // 이넘으로 교체
    public string ClearTime;
    public float clearTimeSec;
    public int GetStar;
    public int starNeedToOpen;
    public bool isOpened;
    public bool isCleared;
    public float[] GetStarStandards;

    public void ChangeBossData(string _ClearTime, float _clearTimeSec, bool _isCleared)
    {
        ClearTime = _ClearTime;
        clearTimeSec = _clearTimeSec;

        if (_isCleared)
            isCleared = true;

        GetStarFunc();
    }

    public void GetStarFunc()
    {
        if (isCleared)
        {
            int count = 1;

            for (int i = 0; i < GetStarStandards.Length; i++)
            {
                if (i == 0 || clearTimeSec < GetStarStandards[i - 1])
                    count++;
            }

            GetStar = count;
        }
    }
}

public class GameInGameData : Singleton<GameInGameData>
{
    [Header("Player Data")]
    public int playerCurrentHP;
    public int playerMaxHP;

    [Header("Scene Data")]
    public BossName currentStageBossName;
    public SceneName currentSceneName;
    public SceneName beforeSceneName;

    [Header("Boss Data")]
    [Header("Single Stage")]
    public int totalStar;
    public TextMeshProUGUI totalStarText;
    public BossData tutorialData = new();
    public BossData spiderData = new();
    public BossData wendigoData = new();
    public BossData demonData = new();
    public BossData crazyMonkData = new();
    public BossData bigSlimeData = new();
    public BossData sorcererData = new();
    [Header("Boss Relay Stage")]
    public BossData bossRelayLevelData_1 = new();
    public BossData bossRelayLevelData_2 = new();
    public BossData towerBossData = new();

    private Dictionary<BossName, BossData> bossDataDictionary;

    public Dictionary<BossName, BossData> BossDataDictionary
    {
        get { return bossDataDictionary; }
    }

    protected override void Awake()
    {
        base.Awake();

        bossDataDictionary = new Dictionary<BossName, BossData>
        {
            { spiderData.BossName, spiderData },
            { wendigoData.BossName, wendigoData },
            { demonData.BossName, demonData },
            { crazyMonkData.BossName, crazyMonkData },
            { bigSlimeData.BossName, bigSlimeData },
            { sorcererData.BossName, sorcererData },
            { tutorialData.BossName, tutorialData},
            { bossRelayLevelData_1.BossName, bossRelayLevelData_1},
            { bossRelayLevelData_2.BossName, bossRelayLevelData_2},
            { towerBossData.BossName, towerBossData}
        };

        LoadData();
    }

    public void ResetPlayerHP()
    {
        playerCurrentHP = playerMaxHP;
    }

    public void SetPlayerHP(int _hp)
    {
        playerCurrentHP = _hp;
    }

    public void ChangeBossData(string _ClearTime, float _clearTimeSec, bool _isCleared)
    {
        if (bossDataDictionary.TryGetValue(currentStageBossName, out BossData bossData))
        {
            bossData.ChangeBossData(_ClearTime, _clearTimeSec, _isCleared);
            CalculateTotalStars();
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
        }
    }

    public int GetStarCount()
    {
        if (bossDataDictionary.TryGetValue(currentStageBossName, out BossData bossData))
        {
            return bossData.GetStar;
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
            return 0;
        }
    }

    public void CalculateTotalStars()
    {
        totalStar = 0;
        foreach (var bossData in bossDataDictionary.Values)
        {
            totalStar += bossData.GetStar;
        }
        totalStarText.text = $"X {totalStar.ToString()}";
    }

    public Sprite GetBossImage()
    {
        if (bossDataDictionary.TryGetValue(currentStageBossName, out BossData bossData))
        {
            return bossData.BossImage;
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
            return null;
        }
    }

    public BossName GetNextBossName()
    {
        if (bossDataDictionary.TryGetValue(currentStageBossName, out BossData bossData))
        {
            return bossData.NextBossName;
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
            return BossName.None;
        }
    }

    public BossData GetBossData(BossName _bossName)
    {
        if (bossDataDictionary.TryGetValue(_bossName, out BossData bossData))
        {
            return bossData;
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
            return null;
        }
    }

    public void SaveData()
    {
        foreach (KeyValuePair<BossName, BossData> entry in bossDataDictionary)
        {
            BossData bossData = entry.Value;
            PlayerPrefs.SetString(entry.Key + "_ClearTime", bossData.ClearTime);
            PlayerPrefs.SetFloat(entry.Key + "_clearTimeSec", bossData.clearTimeSec);
            PlayerPrefs.SetInt(entry.Key + "_GetStar", bossData.GetStar);
        }

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        foreach (KeyValuePair<BossName, BossData> entry in bossDataDictionary)
        {
            BossData bossData = entry.Value;
            bossData.ClearTime = PlayerPrefs.GetString(entry.Key + "_ClearTime", bossData.ClearTime);
            bossData.clearTimeSec = PlayerPrefs.GetFloat(entry.Key + "_clearTimeSec", bossData.clearTimeSec);
            bossData.GetStar = PlayerPrefs.GetInt(entry.Key + "_GetStar", bossData.GetStar);
        }

        CalculateTotalStars();
    }
}

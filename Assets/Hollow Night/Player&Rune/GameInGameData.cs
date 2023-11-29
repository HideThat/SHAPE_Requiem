using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossData
{
    public string BossName;
    public Sprite BossImage;
    public string SceneName;
    public string ClearTime;
    public float clearTimeSec;
    public int GetStar;
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
    public string currentStageBossName;
    public string currentSceneName;
    public string beforeSceneName;

    [Header("Boss Data")]
    public int totalStar;
    public BossData stage1_Data = new();
    public BossData stage2_Data = new();
    public BossData stage3_Data = new();
    public BossData spiderData = new();
    public BossData wendigoData = new();
    public BossData demonData = new();
    public BossData crazyMonkData = new();
    public BossData bigSlimeData = new();
    public BossData sorcererData = new();
    public BossData bossRelayLevelData_1 = new();
    public BossData bossRelayLevelData_2 = new();
    public BossData bossRelayLevelData_3 = new();
    public BossData towerBossData = new();
    public BossData stage1_HardData = new();
    public BossData stage2_HardData = new();
    public BossData stage3_HardData = new();
    public BossData spider_HardData = new();
    public BossData wendigo_HardData = new();
    public BossData demon_HardData = new();
    public BossData crazyMonk_HardData = new();
    public BossData bigSlime_HardData = new();
    public BossData sorcerer_HardData = new();
    public BossData bossRelayLevelHardData_1 = new();
    public BossData bossRelayLevelHardData_2 = new();
    public BossData bossRelayLevelHardData_3 = new();

    private Dictionary<string, BossData> bossDataDictionary;

    protected override void Awake()
    {
        base.Awake();

        bossDataDictionary = new Dictionary<string, BossData>
        {
            { spiderData.BossName, spiderData },
            { wendigoData.BossName, wendigoData },
            { demonData.BossName, demonData },
            { crazyMonkData.BossName, crazyMonkData },
            { bigSlimeData.BossName, bigSlimeData },
            { sorcererData.BossName, sorcererData },
            { stage1_Data.BossName, stage1_Data},
            { stage2_Data.BossName, stage2_Data},
            { stage3_Data.BossName, stage3_Data},
            { bossRelayLevelData_1.BossName, bossRelayLevelData_1},
            { bossRelayLevelData_2.BossName, bossRelayLevelData_2},
            { bossRelayLevelData_3.BossName, bossRelayLevelData_3},
            { towerBossData.BossName, towerBossData},
            { stage1_HardData.BossName, stage1_HardData},
            { stage2_HardData.BossName, stage2_HardData},
            { stage3_HardData.BossName, stage3_HardData},
            { spider_HardData.BossName, spider_HardData},
            { wendigo_HardData.BossName, wendigo_HardData},
            { demon_HardData.BossName, demon_HardData},
            { crazyMonk_HardData.BossName, crazyMonk_HardData},
            { bigSlime_HardData.BossName, bigSlime_HardData},
            { sorcerer_HardData.BossName, sorcerer_HardData},
            { bossRelayLevelHardData_1.BossName, bossRelayLevelHardData_1},
            { bossRelayLevelHardData_2.BossName, bossRelayLevelHardData_2},
            { bossRelayLevelHardData_3.BossName, bossRelayLevelHardData_3}
        };
    }

    public void ResetPlayerHP()
    {
        playerCurrentHP = playerMaxHP;
    }

    public void SetPlayerHP(int _hp)
    {
        playerCurrentHP = _hp;
    }

    public void ChangeStageCard(StageCard _card)
    {
        if (bossDataDictionary.TryGetValue(_card.BossName, out BossData bossData))
        {
            _card.SceneName = bossData.SceneName;
            _card.ClearTime = bossData.ClearTime;
            _card.GetStar = bossData.GetStar;
        }
        else
        {
            Debug.Log("카드의 이름과 보스의 이름이 일치하지 않습니다.");
        }
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[Serializable]
public class Stage1_Data : BossData { }

[Serializable]
public class Stage2_Data : BossData { }

[Serializable]
public class Stage3_Data : BossData { }

[Serializable]
public class SpiderData : BossData { }

[Serializable]
public class WendigoData : BossData { }

[Serializable]
public class DemonData : BossData { }

[Serializable]
public class CrazyMonkData : BossData { }

[Serializable]
public class BigSlimeData : BossData { }

[Serializable]
public class SorcererData : BossData { }

[Serializable]
public class BossRelayLevelData_1 : BossData { }

[Serializable]
public class BossRelayLevelData_2 : BossData { }

[Serializable]
public class BossRelayLevelData_3 : BossData { }

[Serializable]
public class TowerBossData : BossData { }

public class GameInGameData : Singleton<GameInGameData>
{
    [Header("Player Data")]
    public int playerCurrentHP;
    public int playerMaxHP;
    public string currentStageBossName;

    [Header("Boss Data")]
    public int totalStar;
    public Stage1_Data stage1_Data = new();
    public Stage2_Data stage2_Data = new();
    public Stage3_Data stage3_Data = new();
    public SpiderData spiderData = new();
    public WendigoData wendigoData = new();
    public DemonData demonData = new();
    public CrazyMonkData crazyMonkData = new();
    public BigSlimeData bigSlimeData = new();
    public SorcererData sorcererData = new();
    public BossRelayLevelData_1 bossRelayLevelData_1 = new();
    public BossRelayLevelData_2 bossRelayLevelData_2 = new();
    public BossRelayLevelData_3 bossRelayLevelData_3 = new();
    public TowerBossData towerBossData = new();

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
            { towerBossData.BossName, towerBossData}
        };
    }

    public void ResetPlayerHP()
    {
        playerCurrentHP = playerMaxHP;
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

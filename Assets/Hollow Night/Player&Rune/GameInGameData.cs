using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossData
{
    public string BossName;
    public string SceneName;
    public string ClearTime;
    public float clearTimeSec;
    public int GetStar;
    public bool isOpened;
    public bool isCleared;
    public float[] GetStarStandards;

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

public class GameInGameData : Singleton<GameInGameData>
{
    [Header("Player Data")]
    public int playerCurrentHP;
    public int playerMaxHP;

    [Header("Boss Data")]
    public Stage1_Data stage1_Data = new();
    public Stage2_Data stage2_Data = new();
    public Stage3_Data stage3_Data = new();
    public SpiderData spiderData = new();
    public WendigoData wendigoData = new();
    public DemonData demonData = new();
    public CrazyMonkData crazyMonkData = new();
    public BigSlimeData bigSlimeData = new();
    public SorcererData sorcererData = new();

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
}

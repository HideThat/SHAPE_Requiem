using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInGameData : Singleton<GameInGameData>
{
    [Header("Player Data")]
    public int playerCurrentHP;
    public int playerMaxHP;


    public void ResetPlayerHP()
    {
        playerCurrentHP = playerMaxHP;
    }
}

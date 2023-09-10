using System;
using UnityEngine;

[Serializable]
public abstract class P_State
{
    public MyPlayerState playerState;

    public abstract void Enter(PlayerState_Macine _player);
    public abstract void Execute(PlayerState_Macine _player);
    public abstract void Exit(PlayerState_Macine _player);
}

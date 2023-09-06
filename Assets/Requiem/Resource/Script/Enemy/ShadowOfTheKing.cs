using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowOfTheKing : MonoBehaviour
{
    ShadowState myState;

    public enum ShadowState
    {
        Idle,
        Stun,
        Dead,
        Vacuum_Cleaner,
        Summon_Wraith,
        Hand_Sweep,
        Eye_Laser
    }
}

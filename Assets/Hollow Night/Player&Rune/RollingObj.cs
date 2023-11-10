using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RollingObj : MonoBehaviour
{
    public void SetRotate(float rotateTime)
    {
        transform.DORotate(new(0f, 0f, -360f), rotateTime, RotateMode.FastBeyond360);
    }
}

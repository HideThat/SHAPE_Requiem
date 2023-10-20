using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slime_Thorn : Enemy
{
    [Header("Slime_Thorn")]
    public float maxY;
    public float minY;
    public float changeTime;

    public void StartMakeThorn()
    {
        transform.DOScaleY(maxY, changeTime).OnComplete(() =>
        {
            transform.DOScaleY(minY, changeTime).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
    }
}

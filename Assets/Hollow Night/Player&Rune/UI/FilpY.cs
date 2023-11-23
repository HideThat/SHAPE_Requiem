using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FilpY : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float flipTime;

    private void Start()
    {
        StartCoroutine(FlipY());
    }
    IEnumerator FlipY()
    {
        while (true)
        {
            image.rectTransform.DORotate(new(180f, 0f, 0f), flipTime, RotateMode.Fast);

            yield return new WaitForSeconds(flipTime);

            image.rectTransform.DORotate(new(0f, 0f, 0f), flipTime, RotateMode.Fast);

            yield return new WaitForSeconds(flipTime);
        }
        
    }
}

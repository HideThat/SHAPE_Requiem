using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActiveTrigger : MonoBehaviour
{
    public bool PlayerIn = false;
    public GameObject target;
    public float shakeDuration;
    public float shakeMagnitude;
    public float shakeFrequency = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerIn = true;
            target.SetActive(true);
            CameraManager.Instance.CameraShake(shakeDuration, shakeMagnitude, shakeFrequency);

            Destroy(gameObject);
        }
    }
}

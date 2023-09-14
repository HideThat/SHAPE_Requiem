using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public void SetDestroy(float _dalay)
    {
        StartCoroutine(DestroyObj(_dalay));
    }

    IEnumerator DestroyObj(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Destroy(gameObject);
    }
}

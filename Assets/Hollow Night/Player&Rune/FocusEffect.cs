using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FocusEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    public void SetFocusEffect(float _delay, float _playSpeed)
    {
        StartCoroutine(SetFocusEffectCoroutine(_delay, _playSpeed));
    }

    IEnumerator SetFocusEffectCoroutine(float _delay, float _playSpeed)
    {
        particle.playbackSpeed = _playSpeed;
        yield return new WaitForSeconds(_delay);
        Destroy(gameObject);
    }
}

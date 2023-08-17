using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ReactiveLamp : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] SpriteRenderer lit;
    [SerializeField] float brightness;
    [SerializeField] float maxRadius;
    [SerializeField] float delayTime;
    [SerializeField] float changeTime;
    [SerializeField] ReactiveLampState state;
    [SerializeField] LayerMask enemyLayer;

    private bool isTurningOff = false;
    private float circleCastRadius = 0f;

    private void Start()
    {
        if (state == ReactiveLampState.Infinity)
        {
            TurnOn();
        }
    }

    private void Update()
    {
        switch (state)
        {
            case ReactiveLampState.TurnOn:
                TurnOn();
                break;

            case ReactiveLampState.TurnOff:
                TurnOff();
                break;

            case ReactiveLampState.Infinity:
                // Do nothing as the TurnOn state will persist
                break;

            default:
                break;
        }

        RaycastHit2D enemy = Physics2D.CircleCast(transform.position, circleCastRadius, Vector2.zero, enemyLayer);

        if (enemy.transform.GetComponent<Wraith>() != null)
        {
            enemy.transform.GetComponent<Wraith>().currentState = Wraith.WraithState.Dead;
        }

    }

    private void TurnOn()
    {
        // Check if the light2D is null
        if (light2D == null) return;

        // Preventing continuous calling if the state is already in Infinity
        if (state == ReactiveLampState.Infinity) return;

        DOTween.To(() => circleCastRadius, x => circleCastRadius = x, maxRadius * 0.7f, changeTime);
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, brightness, changeTime);
        lit.DOColor(new Color(lit.color.r, lit.color.g, lit.color.b, 1f), changeTime);
        DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, maxRadius, changeTime)
            .OnComplete(() =>
            {
                if (state != ReactiveLampState.Infinity)
                {
                    StartCoroutine(DelayedTurnOff());
                }
            });
    }

    private void TurnOff()
    {
        // Check if the light2D or lit is null
        if (light2D == null || lit == null) return;

        // Check if already in the process of turning off
        if (isTurningOff) return;

        isTurningOff = true; // Mark as turning off

        DOTween.To(() => circleCastRadius, x => circleCastRadius = x, 0f, changeTime);
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 1f, changeTime);
        DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, 1.5f, changeTime);

        StartCoroutine(BlinkingEffect());
    }

    private IEnumerator BlinkingEffect()
    {
        while (state == ReactiveLampState.TurnOff)
        {
            float blinkDelay = UnityEngine.Random.Range(0.05f, 0.7f); // Random delay value

            lit.color = new Color(lit.color.r, lit.color.g, lit.color.b, 0.1f);
            yield return new WaitForSeconds(blinkDelay);
            lit.color = new Color(lit.color.r, lit.color.g, lit.color.b, 0.3f);
            yield return new WaitForSeconds(blinkDelay);
        }

        isTurningOff = false; // Reset the turning off flag
    }

    IEnumerator DelayedTurnOff()
    {
        yield return new WaitForSeconds(delayTime);
        state = ReactiveLampState.TurnOff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == null || !collision.CompareTag("Rune")) return;

        state = ReactiveLampState.TurnOn;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 빨간색으로 설정
        Gizmos.DrawWireSphere(transform.position, circleCastRadius); // 원의 위치와 반지름을 그림
    }
}

enum ReactiveLampState
{
    TurnOn,
    TurnOff,
    Infinity
}

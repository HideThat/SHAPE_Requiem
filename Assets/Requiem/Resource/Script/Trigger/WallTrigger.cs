using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class WallTrigger : Trigger_Requiem
{
    [SerializeField] private float changeTime = 3f;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Light2D m_light;

    public bool isActive;
    public bool triggerActive = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_light = GetComponent<Light2D>();
    }

    private void FixedUpdate()
    {
        if (isActive && triggerActive)
        {
            spriteRenderer.DOColor(Color.red, changeTime);
            DOTween.To(() => m_light.pointLightOuterRadius, x => m_light.pointLightOuterRadius = x, 5f, changeTime);
            triggerActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerActive = true;
            isActive = true;
        }
    }

    public void ResetTrigger()
    {
        // 도트윈 애니메이션 중단
        spriteRenderer.DOKill();
        DOTween.Kill(m_light);

        // 상태를 초기 상태로 재설정
        isActive = false;
        triggerActive = false;

        // 색상과 광원을 원래 값으로 재설정
        spriteRenderer.color = Color.white; // 이 부분은 원래의 색상으로 변경해야 합니다
        m_light.pointLightOuterRadius = 0f; // 이 부분은 원래의 반지름으로 변경해야 합니다
    }
}

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
        // ��Ʈ�� �ִϸ��̼� �ߴ�
        spriteRenderer.DOKill();
        DOTween.Kill(m_light);

        // ���¸� �ʱ� ���·� �缳��
        isActive = false;
        triggerActive = false;

        // ����� ������ ���� ������ �缳��
        spriteRenderer.color = Color.white; // �� �κ��� ������ �������� �����ؾ� �մϴ�
        m_light.pointLightOuterRadius = 0f; // �� �κ��� ������ ���������� �����ؾ� �մϴ�
    }
}

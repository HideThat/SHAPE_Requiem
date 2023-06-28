using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class WallTrigger : Trigger_Requiem
{
    [SerializeField] private float changeTime = 3f;

    SpriteRenderer spriteRenderer;
    Light2D m_light;

    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_light = GetComponent<Light2D>();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            spriteRenderer.DOColor(Color.red, changeTime);
            DOTween.To(() => m_light.pointLightOuterRadius, x => m_light.pointLightOuterRadius = x, 5f, changeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActive = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RuneTutorial : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float delayTime;
    [SerializeField] bool isActive;

    private void Start()
    {
        // 스프라이트의 초기 알파값을 0으로 설정
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isActive = true;
            // 이전 트윈을 죽이고 새로운 트윈을 시작
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f), delayTime);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            isActive = false;
            // 이전 트윈을 죽이고 새로운 트윈을 시작
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f), delayTime);
        }
    }
}

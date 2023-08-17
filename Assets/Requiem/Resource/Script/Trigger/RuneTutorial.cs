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
        // ��������Ʈ�� �ʱ� ���İ��� 0���� ����
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isActive = true;
            // ���� Ʈ���� ���̰� ���ο� Ʈ���� ����
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f), delayTime);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            isActive = false;
            // ���� Ʈ���� ���̰� ���ο� Ʈ���� ����
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f), delayTime);
        }
    }
}

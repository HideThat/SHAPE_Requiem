using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FearMan : NPC
{
    public enum StateFearMan
    {
        Idle, Fear
    }

    [Header("FearMan")]
    [SerializeField] private CapsuleCollider2D capsuleCollider; // 인스펙터에서만 접근 가능하게
    [SerializeField] private LayerMask layerMask; // 인스펙터에서만 접근 가능하게

    private StateFearMan state = StateFearMan.Idle;

    new void Start()
    {
        base.Start();

        if (capsuleCollider == null)
        {
            Debug.LogError("Capsule Collider is not assigned!");
        }
    }

    new void Update()
    {
        base.Update();

        if (SceneManager.GetActiveScene().name == "6-2")
        {
            if (capsuleCollider == null) return; // 방어 코드, 콜라이더가 없으면 업데이트를 건너뜁니다.

            switch (state)
            {
                case StateFearMan.Idle:
                    AnimationPlay("Idle");
                    break;
                case StateFearMan.Fear:
                    AnimationPlay("Fear");
                    break;
            }

            CheckFearState();

            if (finishTextIndex == 1)
            {
                textIndex = 2;
            }
        }
    }

    private void CheckFearState()
    {
        if (Physics2D.OverlapCapsule(transform.position, capsuleCollider.size, CapsuleDirection2D.Vertical, 0f, layerMask))
        {
            if (textIndex != 2)
            {
                textIndex = 1; // 6-1 기본 상태 대화
            }
            state = StateFearMan.Idle;
        }
        else
        {
            textIndex = 0; // 6-1 공포 상태 대화
            state = StateFearMan.Fear;
        }
    }

}

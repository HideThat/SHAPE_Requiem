using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FearMan : NPC
{
    

    public enum SituationFearMan
    {
        FirstMeet, RuneStatueDescription
    }

    public enum StateFearMan
    {
        Idle, Fear
    }

    [Header("FearMan")]
    [SerializeField] private CapsuleCollider2D capsuleCollider; // 인스펙터에서만 접근 가능하게
    [SerializeField] private LayerMask layerMask; // 인스펙터에서만 접근 가능하게
    [SerializeField] Image keyImage;
    [SerializeField] GameObject keyText;
    public KeyItem keyItem;
    public float keyCoolTime;
    public bool wasGiveKey = false;
    public float throwTime;

    public SituationFearMan situation = SituationFearMan.FirstMeet;
    public StateFearMan state = StateFearMan.Idle;

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

        switch (situation)
        {
            case SituationFearMan.FirstMeet:
                if (capsuleCollider == null) return; // 방어 코드, 콜라이더가 없으면 업데이트를 건너뜁니다.

                HandleAnimationState();
                CheckFearState();
                HandleDialogueState();
                HandleKeyThrowing();
                break;
            case SituationFearMan.RuneStatueDescription:
                AnimationPlay("Idle");
                textIndex = 4;
                break;
            default:
                break;
        }

        if (isPlayerInRange)
        {
            keyImage.DOColor(new Color(1f, 1f, 1f, 1f), 1f);
            keyText.SetActive(true);
        }
        else
        {
            keyImage.DOColor(new Color(1f, 1f, 1f, 0f), 1f);
            keyText.SetActive(false);
        }
    }

    private void HandleAnimationState()
    {
        switch (state)
        {
            case StateFearMan.Idle:
                AnimationPlay("Idle");
                break;
            case StateFearMan.Fear:
                AnimationPlay("Fear");
                break;
        }
    }

    private void HandleDialogueState()
    {
        if (finishTextIndex == 1)
        {
            textIndex = 2;
        }

        if (finishTextIndex == 3)
        {
            finishTextIndex = 0;
            notComtpleteTalk = false;
            textIndex = 1;
        }
    }

    private void HandleKeyThrowing()
    {
        if (currentTextIndex == 8 && !wasGiveKey)
        {
            if (keyItem == null || keyItem.gameObject == null)
            {
                Debug.LogError("Key Item or its GameObject is null!");
                return; // keyItem이 없으면 리턴
            }
            wasGiveKey = true;
            KeyThrow(45f, 3f, throwTime);
        }
    }

    private void CheckFearState()
    {
        if (IsPlayerInFearRange())
        {
            SetIdleState();
        }
        else
        {
            SetFearState();
        }
    }

    private bool IsPlayerInFearRange()
    {
        return Physics2D.OverlapCapsule(transform.position, capsuleCollider.size, CapsuleDirection2D.Vertical, 0f, layerMask);
    }

    private void SetIdleState()
    {
        if (textIndex != 2)
        {
            textIndex = 1; // 6-1 기본 상태 대화
        }

        if (textIndex != 2 && notComtpleteTalk)
        {
            textIndex = 3; // 중간에 끊겼을 때 대화
        }

        state = StateFearMan.Idle;
    }

    private void SetFearState()
    {
        textIndex = 0; // 6-1 공포 상태 대화
        state = StateFearMan.Fear;
    }

    void KeyThrow(float _angle, float _power, float _time)
    {
        if (keyItem == null || keyItem.gameObject == null)
        {
            Debug.LogError("Key Item or its GameObject is null!");
            return;
        }

        GameObject itemObject = Instantiate(keyItem.gameObject, transform.position, Quaternion.identity);
        StartCoroutine(itemObject.GetComponent<KeyItem>().InteractionCooldown(keyCoolTime));
        Rigidbody2D rb = itemObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = itemObject.AddComponent<Rigidbody2D>(); // 리지드바디2D가 없다면 추가합니다.
        }

        rb.bodyType = RigidbodyType2D.Dynamic;

        float radianAngle = _angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        Vector2 direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)); // 지정된 각도로 방향을 계산
        rb.velocity = direction * _power; // 계산된 방향과 힘으로 투척합니다.

        StartCoroutine(ChangeBodyTypeToStatic(rb, _time)); // _time 동안 대기 후 리지드바디 타입을 Static으로 변경합니다.
    }

    IEnumerator ChangeBodyTypeToStatic(Rigidbody2D rb, float time)
    {
        yield return new WaitForSeconds(time);
        rb.bodyType = RigidbodyType2D.Static; // 지정된 시간 후 리지드바디 타입을 Static으로 변경
    }
}

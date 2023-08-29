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
    [SerializeField] private CapsuleCollider2D capsuleCollider; // �ν����Ϳ����� ���� �����ϰ�
    [SerializeField] private LayerMask layerMask; // �ν����Ϳ����� ���� �����ϰ�
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
                if (capsuleCollider == null) return; // ��� �ڵ�, �ݶ��̴��� ������ ������Ʈ�� �ǳʶݴϴ�.

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
                return; // keyItem�� ������ ����
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
            textIndex = 1; // 6-1 �⺻ ���� ��ȭ
        }

        if (textIndex != 2 && notComtpleteTalk)
        {
            textIndex = 3; // �߰��� ������ �� ��ȭ
        }

        state = StateFearMan.Idle;
    }

    private void SetFearState()
    {
        textIndex = 0; // 6-1 ���� ���� ��ȭ
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
            rb = itemObject.AddComponent<Rigidbody2D>(); // ������ٵ�2D�� ���ٸ� �߰��մϴ�.
        }

        rb.bodyType = RigidbodyType2D.Dynamic;

        float radianAngle = _angle * Mathf.Deg2Rad; // ������ �������� ��ȯ
        Vector2 direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)); // ������ ������ ������ ���
        rb.velocity = direction * _power; // ���� ����� ������ ��ô�մϴ�.

        StartCoroutine(ChangeBodyTypeToStatic(rb, _time)); // _time ���� ��� �� ������ٵ� Ÿ���� Static���� �����մϴ�.
    }

    IEnumerator ChangeBodyTypeToStatic(Rigidbody2D rb, float time)
    {
        yield return new WaitForSeconds(time);
        rb.bodyType = RigidbodyType2D.Static; // ������ �ð� �� ������ٵ� Ÿ���� Static���� ����
    }
}

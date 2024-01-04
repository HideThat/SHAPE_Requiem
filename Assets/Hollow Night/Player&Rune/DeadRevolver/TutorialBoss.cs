using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialBoss : MonoBehaviour
{
    public Animator animator;
    public Transform rock;
    public Transform wallCollider;
    public Transform bossAppearEffect;

    void Start()
    {
        StartCoroutine(AppearBoss());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 movePos;
    public float moveTime;

    IEnumerator FSM()
    {
        animator.Play("A_LInteractionPull");
        yield return null;
    }

    IEnumerator AppearBoss()
    {
        bool finishMove = false;

        transform.DOMove(movePos, moveTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            finishMove = true;
        });

        while (!finishMove)
        {
            CameraManager.Instance.CameraShake();
            yield return new WaitForSeconds(0.5f);
        }
        animator.Play("A_Climb");
        rock.parent= null;
        wallCollider.parent= null;
        bossAppearEffect.parent= null;
        yield return new WaitForSeconds(0.7f);
        bossAppearEffect.gameObject.SetActive(true);
        yield return CameraShakeLoop(2f);
        StartCoroutine(FSM());
    }

    IEnumerator CameraShakeLoop(float _time)
    {
        float elapsedTime = 0f; // ��� �ð� ����

        while (elapsedTime < _time)
        {
            CameraManager.Instance.CameraShake(); // ī�޶� ����
            yield return new WaitForSeconds(0.5f); // 0.5�� ���

            elapsedTime += 0.5f; // ��� �ð� ������Ʈ
        }
    }

    public Vector2 errorPos;

    public void ClimbFinishChangePos()
    {
        transform.position = new(transform.position.x + errorPos.x, transform.position.y + errorPos.y);
    }
}

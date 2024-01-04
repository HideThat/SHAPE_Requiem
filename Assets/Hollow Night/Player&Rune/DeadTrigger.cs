using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    // 이 트리거와 플레이어가 닿을 시
    // 조작 불가
    // 플레이어의 죽음 모션 재생
    // 플레이어가 위로 튕김
    // 화면 암전
    // 리스폰 포인트에서 플레이어 리스폰
    // 화면 페이드 인
    // 조작 가능
    public Vector2 responPoint;
    bool isActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            isActive = true;
            TriggerPlayer();
        }
    }

    void TriggerPlayer()
    {
        StartCoroutine(TriggerPlayerCoroutine());
    }

    IEnumerator TriggerPlayerCoroutine()
    {
        PlayerCoroutine.Instance.DeadNoneUI(new(0, 1));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SceneChangeDoor.Instance.FadeIn());
        yield return new WaitForSeconds(1.5f);
        PlayerCoroutine.Instance.transform.position = responPoint;
        isActive = false;
        StartCoroutine(SceneChangeDoor.Instance.FadeOut());
        
    }
}

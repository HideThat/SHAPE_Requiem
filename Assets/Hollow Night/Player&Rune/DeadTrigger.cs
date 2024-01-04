using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    // �� Ʈ���ſ� �÷��̾ ���� ��
    // ���� �Ұ�
    // �÷��̾��� ���� ��� ���
    // �÷��̾ ���� ƨ��
    // ȭ�� ����
    // ������ ����Ʈ���� �÷��̾� ������
    // ȭ�� ���̵� ��
    // ���� ����
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

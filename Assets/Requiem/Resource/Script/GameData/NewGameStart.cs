// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewGameStart : MonoBehaviour
{
    [SerializeField] private Transform player; // �÷��̾� ������Ʈ
    [SerializeField] private Transform rune; // �� ������Ʈ

    private Animator playerAnimator; // �÷��̾� �ִϸ�����


    private void Start()
    {
        InitializeGame(); // ���� �ʱ�ȭ
    }

    private void InitializeGame()
    {
        player = PlayerData.PlayerObj.transform;
        rune = RuneData.RuneObj.transform;

        PlayerData.PlayerIsMove = false; // �÷��̾� �̵� �Ұ���
        PlayerData.PlayerIsGetRune = false; // �÷��̾� �� ȹ�� �Ұ���
        playerAnimator = player.GetComponent<Animator>(); // �÷��̾� �ִϸ����� ������Ʈ �޾ƿ�
        playerAnimator.SetBool("IsFirstStart", true); // ó�� ���� ���·� ����

        if (player == null) Debug.Log("player == null");
        if (rune == null) Debug.Log("rune == null");
        if (playerAnimator == null) Debug.Log("playerAnimator == null");
    }

    private void Update()
    {
        CheckMoveKeyPress(); // �̵� Ű ���� üũ
    }

    private void CheckMoveKeyPress()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) // A �Ǵ� D Ű�� ������ ���
        {
            playerAnimator.SetBool("IsFirstStart", false); // ù ���� �ִϸ��̼� ����
            StartCoroutine(EnablePlayerMovement()); // �÷��̾� �̵� Ȱ��ȭ �ڷ�ƾ ����
        }
    }

    private IEnumerator EnablePlayerMovement()
    {
        yield return new WaitForSeconds(1f); // 1�� ���

        PlayerData.PlayerIsMove = true; // �÷��̾� �̵� ����
        Destroy(GetComponent<NewGameStart>());
    }
}

// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewGameStart : MonoBehaviour
{
    [SerializeField] private Transform player; // �÷��̾� ������Ʈ
    [SerializeField] private Transform rune; // �� ������Ʈ
    [SerializeField] private Animator playerAnimator; // �÷��̾� �ִϸ�����

    PlayerControllerGPT playerController;
    HP_SystemGPT hP_SystemGPT;
    RuneControllerGPT runeController;


    private void Start()
    {
        InitializeGame(); // ���� �ʱ�ȭ
    }

    private void InitializeGame()
    {
        playerController = player.GetComponent<PlayerControllerGPT>();
        hP_SystemGPT = player.GetComponent<HP_SystemGPT>();
        runeController = player.GetComponent<RuneControllerGPT>();

        playerController.playerData.canMove = false; // �÷��̾� �̵� �Ұ���
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

        playerController.playerData.canMove = true; // �÷��̾� �̵� ����
        this.enabled = false;
    }
}

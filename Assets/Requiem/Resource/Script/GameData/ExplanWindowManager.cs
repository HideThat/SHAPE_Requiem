// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanWindowManager : MonoBehaviour
{
    private GameObject[] explanWindows = new GameObject[32];

    // ������ �� ����â �ʱ�ȭ
    private void Start()
    {
        InitializeExplanWindows();
    }

    // ����â �迭�� �� �ڽ� ���ӿ�����Ʈ�� �Ҵ�
    private void InitializeExplanWindows()
    {
        for (int i = 0; i < explanWindows.Length; i++)
        {
            explanWindows[i] = transform.GetChild(i).gameObject;
        }
    }

    // �κ��丮�� ������ ���� ��� ����â ��Ȱ��ȭ
    void Update()
    {
        if (!DataController.IsInvenOpen)
        {
            DeactivateAllExplanWindows();
        }
    }

    // ��� ����â�� ��Ȱ��ȭ�ϴ� �޼���
    private void DeactivateAllExplanWindows()
    {
        for (int i = 0; i < explanWindows.Length; i++)
        {
            explanWindows[i].SetActive(false);
        }
    }
}

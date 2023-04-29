// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseOver;
    public GameObject explanWindow;
    private PlayerInventorySystem playerInven;
    private int index;

    // �ʱ�ȭ
    private void Start()
    {
        InitializeVariables();
    }

    // ���� �ʱ�ȭ
    private void InitializeVariables()
    {
        mouseOver = false;
        playerInven = PlayerData.PlayerObj.GetComponent<PlayerInventorySystem>();
        index = transform.GetSiblingIndex();
        InitializeExplanWindow();
    }

    // ���� â �ʱ�ȭ
    private void InitializeExplanWindow()
    {
        explanWindow = GameObject.Find("ExplanWindowEvery").transform.GetChild(index).gameObject;
        explanWindow.SetActive(false);
    }

    // ���� â ������Ʈ
    private void FixedUpdate()
    {
        if (DataController.IsInvenOpen)
        {
            UpdateExplanWindow();
        }
        else
        {
            explanWindow.SetActive(false);
        }
    }

    // ���� â ���¿� ���� ������Ʈ
    private void UpdateExplanWindow()
    {
        if (mouseOver && playerInven.m_items[index] != null)
        {
            explanWindow.SetActive(true);
            ChangeExplanWindow(playerInven.m_items[index]);
        }
        else
        {
            explanWindow.SetActive(false);
        }
    }

    // ���콺 ���� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    // ���콺�� ��� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    // ���� â�� �ؽ�Ʈ ����
    private void ChangeExplanWindow(Item item)
    {
        TextMeshProUGUI TMPro = explanWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TMPro.text = item.m_explanation;
    }
}

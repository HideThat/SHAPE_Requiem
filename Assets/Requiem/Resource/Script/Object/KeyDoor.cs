// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private int keyID; // Ű ID
    [SerializeField] private AudioClip doorSound;
    [SerializeField] private float invokeTime;
    [SerializeField] public bool isOpened;
    [SerializeField] private SpriteRenderer openedSprite;
    [SerializeField] private LightsManager lightsManager;
    [SerializeField] private SpriteRenderer[] keyUISpriteRenderer;
    [SerializeField] private bool playerIn = false;
    [SerializeField] private SpriteRenderer needKeyUI;
    [SerializeField] private bool needKeyUIOpen = false;

    private AudioSource audioSource;


    private void Start()
    {
        openedSprite = transform.Find("Lit").GetComponent<SpriteRenderer>();
        lightsManager = openedSprite.GetComponent<LightsManager>();
        audioSource = GetComponent<AudioSource>();
        playerIn = false;
        needKeyUI = transform.Find("NeedKeyUI").GetComponent<SpriteRenderer>();

        if (audioSource == null) Debug.Log("audioSource == null");
        openedSprite.gameObject.SetActive(true);
        OffNeedKeyUI();
    }

    private void Update()
    {
        DoorStateChange();
        OnOffUI();
        OnOffNeedKeyUI();
    }

    // Ʈ���ſ� �ٸ� ������Ʈ�� ���� �� ó���ϴ� �Լ�
    private void OnTriggerStay2D(Collider2D collision)
    {
        //��ȣ�ۿ� ���� UI ��ġ

        if (IsPlayer(collision))
        {
            playerIn = true;

            if (isOpened)
            {
                PlayerData.PlayerSavePoint = transform.position;
            }
        }

        if (IsPlayer(collision) && Input.GetKeyDown(KeyCode.F))
        {
            PlayerInventorySystem inven = GetPlayerInventorySystem(collision);
            OpenAndSearchInventory(inven);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerIn = false;
        }
    }

    // �÷��̾����� Ȯ���ϴ� �Լ�
    private bool IsPlayer(Collider2D collision)
    {
        return collision.gameObject.layer == (int)LayerName.Player;
    }

    // �÷��̾� �κ��丮 �ý����� �������� �Լ�
    private PlayerInventorySystem GetPlayerInventorySystem(Collider2D collision)
    {
        return collision.GetComponent<PlayerInventorySystem>();
    }

    // �κ��丮�� ���� Ű�� ã�� �Լ�
    private void OpenAndSearchInventory(PlayerInventorySystem inven)
    {
        inven.OpenInventory();
        bool hasKey = false;

        for (int i = 0; i < inven.currentIndex; i++)
        {
            if (HasKey(inven, i))
            {
                hasKey = true;
                UseKeyAndActiveDoor(inven, i);
                break;
            }
        }

        if (!hasKey)
        {
            needKeyUIOpen = true;
        }

        UpdateAndCloseInventory(inven);
    }

    // �κ��丮�� Ű�� �ִ��� Ȯ���ϴ� �Լ�
    private bool HasKey(PlayerInventorySystem inven, int index)
    {
        return inven.items[index].m_ID == keyID;
    }

    // Ű�� ����ϰ� ���� �۵��ϴ� �Լ�
    private void UseKeyAndActiveDoor(PlayerInventorySystem inven, int index)
    {
        inven.UseItem(index);
        inven.CloseInventory();
        isOpened = true;
        audioSource.PlayOneShot(doorSound);
    }

    // �κ��丮�� ������Ʈ�ϰ� �ݴ� �Լ�
    private void UpdateAndCloseInventory(PlayerInventorySystem inven)
    {
        inven.playerInventory.GetComponent<InventorySystem>().UpdateInventory();
        inven.CloseInventory();
    }

    void DoorStateChange()
    {
        if (isOpened)
        {
            openedSprite.DOColor(new Color(255f, 255f, 255f, 255f), lightsManager.turnOnTime);
            lightsManager.turnOffValue = false;
        }
        else
        {
            openedSprite.DOColor(new Color(255f, 255f, 255f, 0f), lightsManager.turnOffTime);
            lightsManager.turnOffValue = true;
        }
    }
    
    void OnOffNeedKeyUI()
    {
        if (needKeyUIOpen)
        {
            OnNeedKeyUI();
            Invoke("ChangeNeedKeyUIOpen", 1f);
        }
        else
        {
            OffNeedKeyUI();
        }
    }

    void ChangeNeedKeyUIOpen()
    {
        needKeyUIOpen = false;
    }

    void OnNeedKeyUI()
    {
        needKeyUI.DOColor(new Color(255f, 255f, 255f, 255f), 1f);
    }

    void OffNeedKeyUI()
    {
        needKeyUI.DOColor(new Color(255f, 255f, 255f, 0f), 1f);
    }

    void OnOffUI()
    {
        if (playerIn && !isOpened)
        {
            OnUI();
        }
        else
        {
            OffUI();
        }
    }

    void OnUI()
    {
        for (int i = 0; i < keyUISpriteRenderer.Length; i++)
        {
            keyUISpriteRenderer[i].DOColor(new Color(255f, 255f, 255f, 255f), 1f);
        }
    }

    void OffUI()
    {
        for (int i = 0; i < keyUISpriteRenderer.Length; i++)
        {
            keyUISpriteRenderer[i].DOColor(new Color(255f, 255f, 255f, 0f), 1f);
        }
    }

    void DestroyDoor()
    {
        Destroy(gameObject);
    }
}

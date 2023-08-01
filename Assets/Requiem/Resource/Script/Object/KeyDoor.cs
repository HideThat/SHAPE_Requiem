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
    [SerializeField] public bool isOpened;
    [SerializeField] private SpriteRenderer openedSprite;
    [SerializeField] private LightsManager lightsManager;
    [SerializeField] private SpriteRenderer[] keyUISpriteRenderer;
    [SerializeField] private bool playerIn = false;
    [SerializeField] private SpriteRenderer needKeyUI;
    [SerializeField] private bool needKeyUIOpen = false;
    [Header("���α׷���")]
    [SerializeField] private float delayTime;
    [SerializeField] private float sightRadius;
    [SerializeField] private FieldOfView2D view2D;

    private AudioSource audioSource;
    private bool isInventoryOpen = false;

    private void Start()
    {
        openedSprite = transform.Find("Lit").GetComponent<SpriteRenderer>();
        lightsManager = openedSprite.GetComponent<LightsManager>();
        audioSource = GetComponent<AudioSource>();
        playerIn = false;
        needKeyUI = transform.Find("NeedKeyUI").GetComponent<SpriteRenderer>();

        view2D.TurnOffView();

        if (audioSource == null) Debug.Log("audioSource == null");
        openedSprite.gameObject.SetActive(true);
        OffNeedKeyUI();
    }

    private void FixedUpdate()
    {
        OnOffNeedKeyUI();
    }

    private void Update()
    {
        DoorStateChange();
        OnOffUI();
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

        if (IsPlayer(collision) && Input.GetKeyDown(KeyCode.F) && !isOpened)
        {
            PlayerInventorySystem inven = GetPlayerInventorySystem(collision);
            StartCoroutine(OpenAndSearchInventoryCoroutine(inven));
        }
    }

    private IEnumerator OpenAndSearchInventoryCoroutine(PlayerInventorySystem inven)
    {
        isInventoryOpen = true;
        yield return new WaitForSeconds(delayTime);

        if (!isOpened) // ���� �̹� �������� ���� ��쿡�� Ű �˻� ����
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

            inven.playerInventory.GetComponent<InventorySystem>().UpdateInventory();
            inven.CloseInventory();
        }

        isInventoryOpen = false;
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
            view2D.TurnOnView(sightRadius, lightsManager.turnOnTime);
        }
        else
        {
            openedSprite.DOColor(new Color(255f, 255f, 255f, 0f), lightsManager.turnOffTime);
            lightsManager.turnOffValue = true;
            view2D.TurnOffView();
        }
    }
    
    void OnOffNeedKeyUI()
    {
        if (needKeyUIOpen && !isOpened)
        {
            OnNeedKeyUI();
            //Invoke("ChangeNeedKeyUIOpen", 1f);
            ChangeNeedKeyUIOpen();
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

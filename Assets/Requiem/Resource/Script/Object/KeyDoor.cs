// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private Item keyData; // Ű ID
    [SerializeField] private AudioClip doorSound;
    [SerializeField] public bool isOpened;
    [SerializeField] private SpriteRenderer openedSprite;
    [SerializeField] private LightsManager lightsManager;
    [SerializeField] private SpriteRenderer[] keyUISpriteRenderer;
    [SerializeField] private bool playerIn = false;
    [SerializeField] private SpriteRenderer needKeyUI;
    [SerializeField] private bool needKeyUIOpen = false;
    [Header("programer")]
    [SerializeField] private float delayTime;
    [SerializeField] private float sightRadius;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform player;

    private AudioSource audioSource;
    private bool isInventoryOpen = false;

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

    private void FixedUpdate()
    {
        OnOffNeedKeyUI();
    }

    private void Update()
    {
        DoorStateChange();
        OnOffUI();

        if (!isOpened && playerIn)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (player.GetComponent<InventorySysyem>().ContainsItem(keyData))
                {
                    player.GetComponent<InventorySysyem>().RemoveItem(keyData);
                    isOpened = true;
                    audioSource.PlayOneShot(doorSound);
                }
            }
        }
    }

    // Ʈ���ſ� �ٸ� ������Ʈ�� ���� �� ó���ϴ� �Լ�
    private void OnTriggerStay2D(Collider2D collision)
    {
        //��ȣ�ۿ� ���� UI ��ġ

        if (IsPlayer(collision))
        {
            playerIn = true;
            player = collision.transform;

            if (isOpened)
            {
                SaveSystem.Instance.responPoint.responSceneName = SceneManager.GetActiveScene().name;
                SaveSystem.Instance.responPoint.responScenePoint = transform.position;
            }
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

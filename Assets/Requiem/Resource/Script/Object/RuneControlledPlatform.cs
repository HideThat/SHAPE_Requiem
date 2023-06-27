using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneControlledPlatform : MonoBehaviour
{
    [SerializeField] private RuneControllerGPT runeController;
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 destination;
    [SerializeField] private AudioClip soundClip;

    private AudioSource audioSource;

    private Vector2 target;
    private Vector2 origin;
    private bool isRuneAttached = false;

    private float originalMoveTime;

    private void Start()
    {
        if (runeController == null)
        {
            runeController = PlayerData.PlayerObj.GetComponent<RuneControllerGPT>();
        }

        audioSource = transform.Find("Sound").GetComponent<AudioSource>();
        player = PlayerData.PlayerObj.transform;

        audioSource.clip = soundClip;
        origin = transform.position;
        target = destination;
        originalMoveTime = runeController.moveTime;
        audioSource.gameObject.SetActive(false);

        if (player == null) Debug.LogError("Player object not found.");
        if (runeController == null) Debug.LogError("RuneControllerGPT component not found.");
        if (audioSource == null) Debug.LogError("AudioSource component not found.");
        if (soundClip == null) Debug.LogError("AudioClip not assigned.");
    }

    private void Update()
    {
        if (isRuneAttached)
        {
            MovePlatform();
            PlaySound(isRuneAttached);

            if (Input.GetMouseButtonDown(1))
            {
                DetachRune();
            }
        }
        else
        {
            PlaySound(isRuneAttached);
        }

        UpdateTarget();
    }

    private void MovePlatform()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void UpdateTarget()
    {
        if ((Vector2)transform.position == origin)
        {
            if (isRuneAttached)
            {
                DetachRune();
            }
            target = destination;
        }

        if ((Vector2)transform.position == destination)
        {
            if (isRuneAttached)
            {
                DetachRune();
            }
            target = origin;
        }
    }

    private void DetachRune()
    {
        runeController.moveTime = originalMoveTime;
        RuneData.RuneUseControl = true;
        runeController.target = player.position;
        isRuneAttached = false;
        runeController.isShoot = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rune") && RuneData.RuneActive)
        {
            RuneData.RuneUseControl = false;
            runeController.moveTime = 0.1f;
            isRuneAttached = true;
        }
    }

    private void PlaySound(bool isAttached)
    {
        audioSource.gameObject.SetActive(isAttached);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.parent = null;
        }
    }
}

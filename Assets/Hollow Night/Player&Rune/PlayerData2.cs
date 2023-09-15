using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerData2
{
    [Header("Player Move")]
    public float playerSpeed = 6f; // �÷��̾� �̵��ӵ�
    public bool canMove; // �̵� ����

    [Header("Player Jump")]
    public int jumpLeft; // ���� ���� Ƚ��
    public int maxJump = 2; // ���� ���� Ƚ��
    public float minJumpSpeed = 2f; // �ִ� ���� �ӵ�
    public float timeToReachMaxSpeed = 2f; // �ִ� ���� �ӵ�
    public float platformCastDistance; // ������ �浹 ����
    public float wallCastDistance; // ������ �浹 ����
    public Transform wallCastTransform;
    public Vector2 wallBoxCastSize; // ������ �浹 ����
    public bool isJump; // ���� ���� üũ
    public bool m_isGrounded; // �� ���� ���� üũ
    public LayerMask platform; // �÷��� ���̾� ����ũ
    public ParticleSystem randingEffect;


    [Header("Player Sound")]
    public AudioSource BGM_AudioSource;
    public AudioSource walkAudioSource;
    public AudioSource jumpAudioSource;
    public AudioClip playerMoveSoundClip;
    public AudioClip[] playerJumpSoundClips;


    [Header("Attack")]
    public int damage;
    public float attackSizeX;
    public float attackSizeY;
    public float upAttackSizeX;
    public float upAttackSizeY;
    public int attackStartFrames;
    public int attackEndFrames;
    public int getSoul;
    public float pushForce = 5.0f; // �ڷ� �и��� ���� ũ��
    public LayerMask enemyAndPlatform;
    public EffectDestroy attackEffect;
    public float attackEffectDestroyTime = 0.2f;

    [Header("Dash")]
    public bool isDash = false;
    public bool canDash = true;
    public bool canDashDuringJump = true;  // ���� �߿� �뽬�� �� �� �ִ��� ��Ÿ���� ����
    public float dashTime;
    public float dashCurrentTime;
    public float dashSpeed;
    public float dashDelay;
    public float dashCurrentDelay;
    public float dashDirection;

    [Header("HP")]
    public int HP;
    public int maxHP; // �ִ� ü��
    public bool isHit = false; // ���� ����
    public float hitTime;
    public float verticalDistance; // ���� �浹 üũ �Ÿ�
    public float horizontalDistance; // ���� �浹 üũ �Ÿ�
    public float fadeSpeed = 0.5f; // ������ ����Ǵ� �ӵ�
    public float minAlpha = 0.2f; // �ּ� ����
    public float maxAlpha = 1f;  // �ִ� ����
    public float cycleTime = 2f; // ������ ������ ������ٰ� ��Ÿ���� ��ü �ֱ�
    public bool loseControl = false;
    public float recorverDelay;
    public Coroutine hitCoroutine;

    [Header("Skill")]
    public int currentSoul;
    public int maxSoul;
    public Image soul;

    [Header("UI")]
    public Canvas uiCanvas;
    public GridLayoutGroup HP_Panel;
    public List<GameObject> heart;
}

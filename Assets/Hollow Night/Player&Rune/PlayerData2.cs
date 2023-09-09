using System;
using UnityEngine;

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
    [SerializeField] public AudioSource BGM_AudioSource;
    [SerializeField] public AudioSource walkAudioSource;
    [SerializeField] public AudioSource jumpAudioSource;
    [SerializeField] public AudioClip playerMoveSoundClip;
    [SerializeField] public AudioClip[] playerJumpSoundClips;


    [Header("Attack")]
    [SerializeField] public PlayerAttack playerAttack;
    [SerializeField] public PlayerAttack playerUpAttack;
    [SerializeField] public int damage;
    [SerializeField] public int attackStartFrames;
    [SerializeField] public int attackEndFrames;

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
}

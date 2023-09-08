using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Header("Player Move")]
    public float playerSpeed = 6f; // �÷��̾� �̵��ӵ�
    public bool canMove; // �̵� ����

    [Header("Player Jump")]
    public int jumpLeft; // ���� ���� Ƚ��
    public int maxJump = 2; // ���� ���� Ƚ��
    public float jumpForce; // ���� �Ŀ�
    public float fallForce; // ���� �ӵ�
    public float maxFallSpeed; // �ִ� ���� �ӵ�
    public float castDistance; // ������ �浹 ����
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

    [Header("Player Attack")]
    [SerializeField] public Bullet bullet;
    [SerializeField] public Transform shootPosition;
    [SerializeField] public float shootDelay;
}

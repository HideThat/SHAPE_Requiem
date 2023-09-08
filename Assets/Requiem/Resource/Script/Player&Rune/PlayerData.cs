using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Header("Player Move")]
    public float playerSpeed = 6f; // 플레이어 이동속도
    public bool canMove; // 이동 판정

    [Header("Player Jump")]
    public int jumpLeft; // 남은 점프 횟수
    public int maxJump = 2; // 남은 점프 횟수
    public float jumpForce; // 점프 파워
    public float fallForce; // 낙하 속도
    public float maxFallSpeed; // 최대 낙하 속도
    public float castDistance; // 땅과의 충돌 판정
    public bool isJump; // 점프 상태 체크
    public bool m_isGrounded; // 땅 접촉 상태 체크
    public LayerMask platform; // 플랫폼 레이어 마스크
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

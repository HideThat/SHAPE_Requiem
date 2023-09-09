using System;
using UnityEngine;

[Serializable]
public class PlayerData2
{
    [Header("Player Move")]
    public float playerSpeed = 6f; // 플레이어 이동속도
    public bool canMove; // 이동 판정

    [Header("Player Jump")]
    public int jumpLeft; // 남은 점프 횟수
    public int maxJump = 2; // 남은 점프 횟수
    public float minJumpSpeed = 2f; // 최대 점프 속도
    public float timeToReachMaxSpeed = 2f; // 최대 점프 속도
    public float platformCastDistance; // 땅과의 충돌 판정
    public float wallCastDistance; // 땅과의 충돌 판정
    public Transform wallCastTransform;
    public Vector2 wallBoxCastSize; // 땅과의 충돌 판정
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


    [Header("Attack")]
    [SerializeField] public PlayerAttack playerAttack;
    [SerializeField] public PlayerAttack playerUpAttack;
    [SerializeField] public int damage;
    [SerializeField] public int attackStartFrames;
    [SerializeField] public int attackEndFrames;

    [Header("Dash")]
    public bool isDash = false;
    public bool canDash = true;
    public bool canDashDuringJump = true;  // 점프 중에 대쉬를 할 수 있는지 나타내는 변수
    public float dashTime;
    public float dashCurrentTime;
    public float dashSpeed;
    public float dashDelay;
    public float dashCurrentDelay;
    public float dashDirection;
}

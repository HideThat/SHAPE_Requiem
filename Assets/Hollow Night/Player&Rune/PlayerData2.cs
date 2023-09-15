using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float pushForce = 5.0f; // 뒤로 밀리는 힘의 크기
    public LayerMask enemyAndPlatform;
    public EffectDestroy attackEffect;
    public float attackEffectDestroyTime = 0.2f;

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

    [Header("HP")]
    public int HP;
    public int maxHP; // 최대 체력
    public bool isHit = false; // 맞음 판정
    public float hitTime;
    public float verticalDistance; // 세로 충돌 체크 거리
    public float horizontalDistance; // 가로 충돌 체크 거리
    public float fadeSpeed = 0.5f; // 투명도가 변경되는 속도
    public float minAlpha = 0.2f; // 최소 투명도
    public float maxAlpha = 1f;  // 최대 투명도
    public float cycleTime = 2f; // 투명도가 완전히 사라졌다가 나타나는 전체 주기
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

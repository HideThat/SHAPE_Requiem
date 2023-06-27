// 1차 리펙토링

using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite; // 활성화 스위치
    [SerializeField] private Sprite inactiveSprite; // 비 활성화 스위치
    [SerializeField] private AudioClip switchOnAudio; // 활성화 사운드
    [SerializeField] private AudioClip switchOffAudio; // 비 활성화 사운드

    private SpriteRenderer spriteRenderer; // 자신의 스프라이트 렌더러
    private AudioSource audioSource; // 자신의 오디오 소스
    public bool isActive; // 활성화 여부

    private void Start()
    {
        InitializeComponents();
        SetInitialState();
    }

    // 필요한 컴포넌트 가져오기
    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null) Debug.Log("spriteRenderer == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (inactiveSprite == null) Debug.Log("inactiveSprite == null");
        if (activeSprite == null) Debug.Log("activeSprite == null");
        if (switchOnAudio == null) Debug.Log("switchOnAudio == null");
        if (switchOffAudio == null) Debug.Log("switchOffAudio == null");
    }

    // 스위치의 초기 상태 설정
    private void SetInitialState()
    {
        spriteRenderer.sprite = inactiveSprite;
        isActive = false;
    }

    // 스위치 상호작용 처리를 위한 OnTrigger 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsRuneCollision(collision))
        {
            ToggleSwitchState();
            PlaySwitchAudio();
            UpdateSwitchSprite();
        }
    }

    // 룬과 충돌한 경우 확인
    private bool IsRuneCollision(Collider2D collision)
    {
        return collision.CompareTag("Rune");
    }

    // 스위치 상태 전환
    private void ToggleSwitchState()
    {
        isActive = true;
    }

    // 스위치 오디오 재생
    private void PlaySwitchAudio()
    {
        audioSource.PlayOneShot(isActive ? switchOnAudio : switchOffAudio);
    }

    // 스위치 스프라이트 업데이트
    private void UpdateSwitchSprite()
    {
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
    }

    // 초기화 함수
    public void Initialize()
    {
        isActive = false;
    }
}

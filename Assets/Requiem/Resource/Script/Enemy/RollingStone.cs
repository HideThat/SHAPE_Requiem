// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : Enemy_Static
{
    Vector2 origin; // 자신의 최초 위치를 저장하는 변수
    Rigidbody2D rb; // 자신의 리지드바디를 저장하는 변수
    AudioSource audioSource; // 자신의 오디오 소스
    AudioClip audioClip; // 돌이 활성화 시 재생되는 소리


    void Start()
    {
        m_name = EnemyData.StaticEnemyNameArr[1]; // 이름 설정
        damage = 1; // 데미지 설정
        rb = GetComponent<Rigidbody2D>(); // 자신의 리지드바디 참조.
        audioSource = GetComponent<AudioSource>(); // 자신의 오디오 소스 참조
        audioClip = EnemyData.StaticEnemyAudioClipArr[0]; // 활성화 시 재생되는 소리
        rb.bodyType = RigidbodyType2D.Kinematic; // 움직이지 않게 키네마틱으로 바디 타입 변경
        origin = transform.position; // 자신의 초기 위치 저장


        if (m_name == null) Debug.Log("m_name == null");
        if (rb == null) Debug.Log("m_rigid == null");
        if (origin == null) Debug.Log("origin == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (audioClip == null) Debug.Log("audioClip == null");
    }

    // 돌이 활성화 되었을 때 호출
    public override void TriggerOn()
    {
        transform.parent = null; // 부모 오브젝트 해제 / 해제하지 않으면, 트리거랑 같이 회전하게 된다
        rb.bodyType = RigidbodyType2D.Dynamic; // 오브젝트가 물리 영향을 받게끔 바디 타입 다이나믹으로 설정
        rb.freezeRotation = false; // 오브젝트가 회전할 수 있게 설정
        audioSource.PlayOneShot(audioClip);
        Debug.Log("Rolling Stone is active"); // 돌이 활성화 되었다고 로그 창에 알림.
    }

    // 오브젝트를 초기 상태로 되돌리는 리셋 함수
    public void resetPosition()
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // 움직이지 않게 키네마틱으로 바디 타입 변경
        rb.velocity = Vector2.zero; // 정지
        rb.freezeRotation = true; // 회전 불가
        transform.position = origin; // 초기 위치로 되돌림
    }

}
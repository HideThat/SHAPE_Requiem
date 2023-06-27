// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Trigger : Trigger_Requiem
{
    
    [SerializeField] Enemy_Static m_object; // 트리거를 발동 시킬 해당 오브젝트
    [SerializeField] AudioClip triggerAudioClip; // 작동시 사운드
    [SerializeField] bool m_isActive = false; // 트리거 작동 여부
    
    AudioSource triggerAdioSource; // 트리거의 오디오 소스




    void Start()
    {
        triggerAdioSource = GetComponent<AudioSource>(); // 자신의 오디오 소스 참조

        if (m_object == null) Debug.Log("m_object == null");
        if (triggerAdioSource == null) Debug.Log("m_triggerAdioSource == null");
        if (triggerAudioClip == null) Debug.Log("triggerAudioClip == null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 트리거가 충돌 했는지 체크
        if (collision.gameObject.layer == (int)LayerName.Player && !m_isActive)
        {
            triggerAdioSource.PlayOneShot(triggerAudioClip); // 트리거 사운드 출력
            m_isActive = true; // 트리거 작동 여부 true로 변경
            m_object.TriggerOn(); // 연결된 오브젝트 작동.
        }
    }

    public void ActiveTrigger()
    {
        m_object.TriggerOn();
    }

    public void ResetTrigger()
    {
        m_isActive = false;
    }
}
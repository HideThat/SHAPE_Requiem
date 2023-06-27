// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : Enemy_Static
{
    Vector2 origin; // �ڽ��� ���� ��ġ�� �����ϴ� ����
    Rigidbody2D rb; // �ڽ��� ������ٵ� �����ϴ� ����
    AudioSource audioSource; // �ڽ��� ����� �ҽ�
    AudioClip audioClip; // ���� Ȱ��ȭ �� ����Ǵ� �Ҹ�


    void Start()
    {
        m_name = EnemyData.StaticEnemyNameArr[1]; // �̸� ����
        damage = 1; // ������ ����
        rb = GetComponent<Rigidbody2D>(); // �ڽ��� ������ٵ� ����.
        audioSource = GetComponent<AudioSource>(); // �ڽ��� ����� �ҽ� ����
        audioClip = EnemyData.StaticEnemyAudioClipArr[0]; // Ȱ��ȭ �� ����Ǵ� �Ҹ�
        rb.bodyType = RigidbodyType2D.Kinematic; // �������� �ʰ� Ű�׸�ƽ���� �ٵ� Ÿ�� ����
        origin = transform.position; // �ڽ��� �ʱ� ��ġ ����


        if (m_name == null) Debug.Log("m_name == null");
        if (rb == null) Debug.Log("m_rigid == null");
        if (origin == null) Debug.Log("origin == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (audioClip == null) Debug.Log("audioClip == null");
    }

    // ���� Ȱ��ȭ �Ǿ��� �� ȣ��
    public override void TriggerOn()
    {
        transform.parent = null; // �θ� ������Ʈ ���� / �������� ������, Ʈ���Ŷ� ���� ȸ���ϰ� �ȴ�
        rb.bodyType = RigidbodyType2D.Dynamic; // ������Ʈ�� ���� ������ �ްԲ� �ٵ� Ÿ�� ���̳������� ����
        rb.freezeRotation = false; // ������Ʈ�� ȸ���� �� �ְ� ����
        audioSource.PlayOneShot(audioClip);
        Debug.Log("Rolling Stone is active"); // ���� Ȱ��ȭ �Ǿ��ٰ� �α� â�� �˸�.
    }

    // ������Ʈ�� �ʱ� ���·� �ǵ����� ���� �Լ�
    public void resetPosition()
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // �������� �ʰ� Ű�׸�ƽ���� �ٵ� Ÿ�� ����
        rb.velocity = Vector2.zero; // ����
        rb.freezeRotation = true; // ȸ�� �Ұ�
        transform.position = origin; // �ʱ� ��ġ�� �ǵ���
    }

}
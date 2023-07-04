using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : Enemy_Static
{
    Rigidbody2D rigid;
    public bool isActive = false;
    public bool isRanding = false;
    [SerializeField] FallingBlockTrigger fallingBlockTrigger;
    [SerializeField] FallingBlockRaningTrigger fallingBlockRaningTrigger;
    [SerializeField] AudioSource audioSource;

    Vector2 originPos;
    Quaternion originRotate;

    private void Start()
    {
        damage = 1;
        rigid = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        originPos = transform.position;
        originRotate = transform.rotation;
    }

    void Update()
    {
        if (isActive && !isRanding)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
        else if (!isActive && !isRanding)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else if (isRanding)
        {
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Static;
            m_collider2D.isTrigger = false;
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    public void SetRanding(bool isRanding)
    {
        this.isRanding = isRanding;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        rigid.velocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Static;
        m_collider2D.isTrigger = false;
    }

    public override void ResetEnemy()
    {
        fallingBlockTrigger.ObjActiveSet(true);
        fallingBlockRaningTrigger.ObjActiveSet(true);

        // ��ġ�� ���� ��ġ�� �缳��
        transform.position = originPos;
        // ȸ���� ���� ȸ������ �缳��
        transform.rotation = originRotate;

        // Rigidbody2D�� �Ӽ��� ���� ���·� �缳��
        rigid.bodyType = RigidbodyType2D.Kinematic;

        // �� ���� �������� �ʱ� ���·� �缳��
        isActive = false;
        isRanding = false;

        // Collider2D�� �Ӽ��� �ʱ� ���·� �缳��
        m_collider2D.isTrigger = true;
    }
}

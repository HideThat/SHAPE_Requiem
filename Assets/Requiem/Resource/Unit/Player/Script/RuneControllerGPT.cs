using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneControllerGPT : MonoBehaviour
{
    
    public Vector2 m_target; // ���� ��ǥ ����
    public float m_moveTime; // ���� ��ǥ �������� ���� �ð�
    public bool m_isShoot; // ���� �߻� �������� üũ�ϴ� ����

    [SerializeField] Vector2 m_RunePosition; // ���� �÷��̾� �ֺ��� ���� �� ��ǥ
    [SerializeField] Vector2 m_origin;
    [SerializeField] float m_shootDelayTime; // �� �߻� ������ Ÿ��
    [SerializeField] float m_RuneReturnDistance;
    [SerializeField] bool m_isMouseDelay = false;

    GameObject m_runeObj;
    RuneSoundManager m_runeSoundManager;
    Light2D m_runeSight;

    LayerMask m_layerMask;

    void Start()
    {
        m_runeObj = RuneData.RuneObj;
        m_runeSoundManager = RuneData.RuneObj.GetComponent<RuneSoundManager>();
        m_runeSight = RuneData.RuneObj.GetComponent<Light2D>();
        m_runeObj.transform.parent = null;
        RuneData.RuneActive = false;
        m_runeObj.SetActive(true);
        m_target = transform.position;
        m_isShoot = false;
        m_layerMask = LayerMask.GetMask("Platform", "Wall", "RiskFactor");
    }

    void Update()
    {
        if (PlayerData.PlayerIsGetRune)
        {
            
            RuneColliding();
            RuneControl();
            RuneMove();
        }
    }

    void RuneControl()
    {
        if (RuneData.RuneUseControl)
        {
            if (Input.GetMouseButtonDown(0) && !m_isMouseDelay)
            {
                if (m_isShoot)
                {
                    m_runeSoundManager.PlayRuneOff();
                    m_isShoot = false;
                    m_isMouseDelay = true;
                    StartCoroutine("MouseClickDelay");
                }
                else
                {
                    m_isShoot = true;

                    if (!RuneData.RuneTouchWater)
                    {
                        RuneData.RuneActive = true;
                        RuneData.RuneLightArea.enabled = true;
                    }
                    ChangeTargetToMouse();
                    m_isMouseDelay = true;
                    m_runeSoundManager.PlayRuneOn();
                    StartCoroutine("MouseClickDelay");
                }
            }
            else if (!m_isShoot)
            {
                ReturnRune();
            }

            if (RuneData.RuneTouchWater)
            {
                RunePowerLose();
            }

            RunePowerBack();
            RuneReturnDistance();
        }

    }

    void RuneMove()
    {
        m_runeObj.transform.DOMove(m_target, m_moveTime);
    }

    void ChangeTargetToMouse()
    {
        m_target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    /// <summary>
    /// ���� �������� �÷��̾� ������ ��������ִ� �Լ�
    /// </summary>
    void ReturnRune()
    {
        if (transform.rotation.y == 0)
        {
            // ������ ���� ��
            m_target = new Vector2(transform.position.x + m_RunePosition.x, transform.position.y + m_RunePosition.y);
        }
        else if (transform.rotation.y != 0f)
        {
            // ���� ���� ��
            m_target = new Vector2(transform.position.x + (-m_RunePosition.x), transform.position.y + m_RunePosition.y);
        }
        RuneData.RuneActive = m_isShoot;
        RuneData.RuneLightArea.enabled = false;
    }

    void RunePowerBack()
    {
        if (Vector2.Distance(transform.position, m_runeObj.transform.position) <= RuneData.RunePowerBackDistance && !RuneData.RuneOnWater)
        {
            RuneData.RuneTouchWater = false;
            RuneData.RuneLightArea.enabled = true;
            DOTween.To(() => m_runeSight.pointLightOuterRadius, x => m_runeSight.pointLightOuterRadius = x, RuneData.RuneOuterRadius, RuneData.RunePowerBackTime);
        }
    }

    void RunePowerLose()
    {
        DOTween.To(() => m_runeSight.pointLightOuterRadius, x => m_runeSight.pointLightOuterRadius = x, 0f, RuneData.RunePowerBackTime);
    }


    public void RuneStop()
    {
        m_target = m_runeObj.transform.position;
    }

    IEnumerator MouseClickDelay()
    {
        yield return new WaitForSeconds(m_shootDelayTime);

        m_isMouseDelay = false;
    }

    void RuneReturnDistance()
    {
        if (Vector2.Distance(m_runeObj.transform.position, transform.position) >= m_RuneReturnDistance)
        {
            m_isShoot = false;
            m_isMouseDelay = true;
            m_runeSoundManager.PlayRuneOff();
            StartCoroutine("MouseClickDelay");
        }
    }

    void RuneColliding()
    {
        // �� �Ǵ� �ٸ� ��ֹ����� �浹�� �����ϱ� ���� �� ��ü�� ��ǥ ���� ���̿� Raycast�� ����
        RaycastHit2D hit =
            Physics2D.Raycast(m_runeObj.transform.position,
            (m_target - (Vector2)m_runeObj.transform.position).normalized,
            Vector2.Distance(m_runeObj.transform.position, m_target),
            m_layerMask);

        // Raycast�� �� �Ǵ� �ٸ� ��ֹ��� �浹�� ��� �浹 �������� �� ��ü�� ����
        if (hit.collider != null &&
            (hit.collider.gameObject.layer == (int)LayerName.Platform ||
            hit.collider.gameObject.layer == (int)LayerName.Wall ||
            hit.collider.gameObject.layer == (int)LayerName.RiskFactor))
        {
            m_target = hit.point;
        }
    }
}
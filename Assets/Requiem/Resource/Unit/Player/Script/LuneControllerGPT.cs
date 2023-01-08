using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LuneControllerGPT : MonoBehaviour
{
    /// <summary>
    /// �� GameObject
    /// </summary>
    [SerializeField] GameObject m_luneObj;

    /// <summary>
    /// �� Rigidbody
    /// </summary>
    [SerializeField] Rigidbody2D m_luneRigid;

    /// <summary>
    /// �� �þ�
    /// </summary>
    [SerializeField] Light2D m_luneLight;

    /// <summary>
    /// ���� ��ǥ ����
    /// </summary>
    [SerializeField] public Vector2 m_target;

    /// <summary>
    /// ���� �÷��̾� �ֺ��� ���� �� ��ǥ
    /// </summary>
    [SerializeField] Vector2 m_lunePosition;

    /// <summary>
    /// ���� ��ǥ �������� ���� �ð�
    /// </summary>
    [SerializeField] float m_moveTime;

    /// <summary>
    /// ���� ������ �� ���� �Ҹ�
    /// </summary>
    [SerializeField] AudioSource m_luneSound;

    /// <summary>
    /// ���� �߻� �������� üũ�ϴ� ����
    /// </summary>
    [SerializeField] bool m_isShoot;

    /// <summary>
    /// �� �߻� ������ Ÿ��
    /// </summary>
    [SerializeField] float m_shootDelayTime;

    [SerializeField] bool m_isMouseDelay = false;

    void Start()
    {
        DataController.LuneActive = false;
        m_luneObj.transform.parent = null;
        m_luneObj.SetActive(true);
        m_target = transform.position;
        m_isShoot = false;
        m_luneSound.volume = 0.2f;
    }

    void Update()
    {
        LuneControl();
        LuneSoundController();
        LuneMove();
    }

    /// <summary>
    /// ���� �������� ��Ʈ���� ����ϴ� �Լ�
    /// </summary>
    void LuneControl()
    {
        if (DataController.LuneUseControl)
        {
            if (Input.GetMouseButtonDown(0) && !m_isMouseDelay)
            {
                if (m_isShoot)
                {
                    m_isShoot = false;
                    m_isMouseDelay = true;
                    StartCoroutine("MouseClickDelay");
                }
                else
                {
                    m_isShoot = true;

                    if (!DataController.LuneTouchWater)
                    {
                        DataController.LuneActive = true;
                        DataController.LuneLightArea.enabled = true;
                    }
                    ChangeTargetToMouse();

                    m_isMouseDelay = true;
                    StartCoroutine("MouseClickDelay");
                }
            }
            else if (!m_isShoot)
            {
                ReturnLune();
            }

            if (DataController.LuneTouchWater)
            {
                LunePowerLose();
            }

            LunePowerBack();
        }

    }


    /// <summary>
    /// ���� ���� �������� �̵� ��Ű�� �Լ�
    /// ��� �ݺ� ���� ��
    /// </summary>
    void LuneMove()
    {
        m_luneObj.transform.DOMove(m_target, m_moveTime);
    }

    /// <summary>
    /// ���� ���������� ���콺 �����ͷ� �Ű��ִ� �Լ�
    /// </summary>
    void ChangeTargetToMouse()
    {
        m_target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    /// <summary>
    /// �� ���带 �������ִ� �Լ�
    /// </summary>
    void LuneSoundController()
    {
        if (m_isShoot)
        {
            // �Ķ���� ����(���� �� ����, ���� �� ����, ��ǥ ��, �ҿ� �ð�)
            DOTween.To(() => m_luneSound.volume, x => m_luneSound.volume = x, 1f, 1f);
        }
        else
        {
            DOTween.To(() => m_luneSound.volume, x => m_luneSound.volume = x, 0f, 0.4f);
        }
    }


    /// <summary>
    /// ���� �������� �÷��̾� ������ ��������ִ� �Լ�
    /// </summary>
    void ReturnLune()
    {
        if (transform.rotation.y == 0)
        {
            // ������ ���� ��
            m_target = new Vector2(transform.position.x + m_lunePosition.x, transform.position.y + m_lunePosition.y);
        }
        else if (transform.rotation.y != 0f)
        {
            // ���� ���� ��
            m_target = new Vector2(transform.position.x + (-m_lunePosition.x), transform.position.y + m_lunePosition.y);
        }
        DataController.LuneActive = m_isShoot;
        DataController.LuneLightArea.enabled = false;
    }

    /// <summary>
    /// ���� �þ߸� ȸ���Ѵ�.
    /// </summary>
    void LunePowerBack()
    {
        if (Vector2.Distance(transform.position, m_luneObj.transform.position) <= DataController.LunePowerBackDistance && !DataController.LuneOnWater)
        {
            DataController.LuneTouchWater = false;
            DataController.LuneLightArea.enabled = true;
            DOTween.To(() => m_luneLight.pointLightOuterRadius, x => m_luneLight.pointLightOuterRadius = x, DataController.LuneOuterRadius, DataController.LunePowerBackTime);
        }
    }

    void LunePowerLose()
    {
        DOTween.To(() => m_luneLight.pointLightOuterRadius, x => m_luneLight.pointLightOuterRadius = x, 0f, DataController.LunePowerBackTime);
    }


    /// <summary>
    /// ���� ���ڸ��� �����.
    /// </summary>
    public void LuneStop()
    {
        m_target = m_luneObj.transform.position;
    }

    IEnumerator MouseClickDelay()
    {
        // wait for the specified delay
        yield return new WaitForSeconds(m_shootDelayTime);

        // reset the mouseClicked flag
        m_isMouseDelay = false;
    }
}
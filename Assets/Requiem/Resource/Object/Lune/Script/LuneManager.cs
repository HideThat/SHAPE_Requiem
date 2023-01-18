using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class LuneManager : MonoBehaviour
{
    public static Action a_Initialized;

    [SerializeField] LuneControllerGPT m_luneControl;
    [SerializeField] Light2D m_luneLight;
    [SerializeField] float m_moveTime = 3f;
    [SerializeField] float m_rotationSpeed = 10f;
    [SerializeField] bool m_isStatueInteraction = false;

    Vector2 m_origin;


    private void Start()
    {
        a_Initialized = () => { Initialized(); };

        m_origin = transform.position;
    }

    private void Update()
    {
        if (m_isStatueInteraction)
        {
            StatueInteraction();
        }
    }

    /// <summary>
    /// ���� Ư�� �ݶ��̴� ����  �ӹ� �� �ߵ�(Ʈ����)
    /// ���� �����ϰ� �ȴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case (int)LayerName.Platform:
            case (int)LayerName.Wall:
            case (int)LayerName.RiskFactor:
                m_luneControl.LuneStop();
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<RuneStatue>() != null && DataController.LuneActive)
        {
            m_luneControl.m_target = collision.transform.position;
            DataController.LuneUseControl = false;
            m_isStatueInteraction = true;
        }
    }

    /// <summary>
    /// ���� �ӹ� ��� �ߵ�
    /// </summary>
    public void EnterWater()
    {
        DataController.LuneTouchWater = true;
        DataController.LuneActive = false;
        DataController.LuneOnWater = true;
        DataController.LuneLightArea.enabled = false;
    }

    /// <summary>
    /// ������ ���� ��� �ߵ�
    /// </summary>
    public void ExitWater()
    {
        DataController.LuneOnWater = false;
    }

    /// <summary>
    /// �� �þ� ������ �����Ѵ�.
    /// </summary>
    void ChangeLightArea()
    {
        DataController.LuneLightArea.radius = m_luneLight.pointLightOuterRadius;
    }

    void StatueInteraction()
    {
        transform.Rotate(Vector3.back * m_rotationSpeed);
        transform.DOMove(m_luneControl.m_target, m_moveTime);
        StartCoroutine("StatueInteractionDelay");
    }

    IEnumerator StatueInteractionDelay()
    {
        yield return new WaitForSeconds(m_moveTime);

        DataController.LuneUseControl = true;
        DataController.LuneActive = false;
        m_isStatueInteraction = false;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public void Initialized()
    {
        transform.position = m_origin;
    }
}

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
    [SerializeField] Transform m_statue;

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
            if (!m_statue.GetComponent<RuneStatue>().m_isActive)
            {
                StatueInteraction(m_statue);
            }
        }
    }

    /// <summary>
    /// 룬이 특정 콜라이더 위에  머물 시 발동(트리거)
    /// 룬이 정지하게 된다.
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
            m_statue = collision.transform;
            m_isStatueInteraction = true;
        }
    }

    /// <summary>
    /// 물에 머물 경우 발동
    /// </summary>
    public void EnterWater()
    {
        DataController.LuneTouchWater = true;
        DataController.LuneActive = false;
        DataController.LuneOnWater = true;
        DataController.LuneLightArea.enabled = false;
    }

    /// <summary>
    /// 물에서 나갈 경우 발동
    /// </summary>
    public void ExitWater()
    {
        DataController.LuneOnWater = false;
    }

    /// <summary>
    /// 룬 시야 범위를 변경한다.
    /// </summary>
    void ChangeLightArea()
    {
        DataController.LuneLightArea.radius = m_luneLight.pointLightOuterRadius;
    }

    public void StatueInteraction(Transform _target)
    {
        m_luneControl.m_target = _target.position;
        DataController.LuneUseControl = false;
        transform.Rotate(Vector3.back * m_rotationSpeed);
        transform.DOMove(m_luneControl.m_target, m_moveTime);
        StartCoroutine("StatueInteractionDelay");
    }

    IEnumerator StatueInteractionDelay()
    {
        yield return new WaitForSeconds(m_moveTime);

        DataController.LuneUseControl = true;
        DataController.LuneActive = false;
        DataController.PlayerIsGetLune = true;
        m_isStatueInteraction = false;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        m_statue.GetComponent<RuneStatue>().m_isActive = true;
    }

    public void Initialized()
    {
        transform.position = m_origin;
    }
}

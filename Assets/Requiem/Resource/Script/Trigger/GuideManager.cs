using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    [SerializeField] GameObject m_moveGuide;
    [SerializeField] GameObject m_jumpGuide;
    [SerializeField] GameObject m_doubleJumpGuide;
    [SerializeField] GameObject m_runeThrowGuide;
    [SerializeField] GameObject m_runeReturnGuide;
    [SerializeField] GameObject m_runeChargeGuide;

    private void Start()
    {
        if (m_moveGuide == null) Debug.Log("moveGuide == null");
        else m_moveGuide.SetActive(false);

        if (m_jumpGuide == null) Debug.Log("m_jumpGuide == null");
        else m_jumpGuide.SetActive(false);

        if (m_doubleJumpGuide == null) Debug.Log("m_doubleJumpGuide == null");
        else m_doubleJumpGuide.SetActive(false);

        if (m_runeThrowGuide == null) Debug.Log("m_runeThrowGuide == null");
        else m_runeThrowGuide.SetActive(false);

        if (m_runeReturnGuide == null) Debug.Log("m_runeReturnGuide == null");
        else m_runeReturnGuide.SetActive(false);

        if (m_runeChargeGuide == null) Debug.Log("m_runeChargeGuide == null");
        else m_runeChargeGuide.SetActive(false);
    }

    #region MoveGuide
    public void CloseMoveGuide()
    {
        Destroy(m_moveGuide);
    }

    public void OpenMoveGuide()
    {
        if (m_moveGuide != null)
        {
            m_moveGuide.SetActive(true);
        }
    }
    #endregion MoveGuide
    #region JumpGuide
    public void CloseJumpGuide()
    {
        Destroy(m_jumpGuide);
    }

    public void OpenJumpGuide()
    {
        if (m_jumpGuide != null)
        {
            m_jumpGuide.SetActive(true);
        }
    }
    #endregion JumpGuide
    #region DoubleJumpGuide
    public void CloseDoubleJumpGuide()
    {
        Destroy(m_doubleJumpGuide);
    }

    public void OpenDoubleJumpGuide()
    {
        if (m_doubleJumpGuide != null)
        {
            m_doubleJumpGuide.SetActive(true);
        }
    }
    #endregion DoubleJumpGuide
    #region RuneThrowGuide
    public void CloseRuneThrowGuide()
    {
        m_runeThrowGuide.SetActive(false);
    }

    public void OpenRuneThrowGuide()
    {
        if (m_runeThrowGuide != null)
        {
            m_runeThrowGuide.SetActive(true);
        }
    }
    #endregion RuneThrowGuide
    #region RuneReturnGuide
    public void CloseRuneReturnGuide()
    {
        m_runeReturnGuide.SetActive(false);
    }

    public void OpenRuneReturnGuide()
    {
        m_runeReturnGuide.SetActive(true);
    }
    #endregion RuneReturnGuide
    #region RuneChargeGuide
    public void CloseRuneChargeGuide()
    {
        m_runeChargeGuide.SetActive(false);
    }

    public void OpenRuneChargeGuide()
    {
        m_runeChargeGuide.SetActive(true);
    }
    #endregion RuneChargeGuide
}

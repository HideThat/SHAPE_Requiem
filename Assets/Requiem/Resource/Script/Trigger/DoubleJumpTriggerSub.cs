using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoubleJumpTriggerSub : MonoBehaviour
{
    [SerializeField] DoubleJumpTrigger m_doubleJumpTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            m_doubleJumpTrigger.m_isJump = true;
            m_doubleJumpTrigger.m_isActive = true;
            m_doubleJumpTrigger.m_onTrigger = false;
            m_doubleJumpTrigger.m_doubleJumpGuide.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, gameObject.tag);
    }
#endif
}

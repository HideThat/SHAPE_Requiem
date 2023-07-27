using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

#region EventClass
[Serializable] class OnTriggerEvent
{
    [SerializeField] public UnityEvent TriggerEnter;
    [SerializeField] public UnityEvent TriggerStay;
    [SerializeField] public UnityEvent TriggerExit;
}
[Serializable] class OnCollisionEvent
{
    [SerializeField] public UnityEvent CollisionEnter;
    [SerializeField] public UnityEvent CollisionStay;
    [SerializeField] public UnityEvent CollisionExit;
}

[Serializable] class TimeTriggerEvent
{
    [SerializeField] public UnityEvent TimeEvent;
}
[Serializable] class OnButtonEventNode
{
    public KeyCode m_keyCode;
    [SerializeField] public UnityEvent onButtonEvent;
}
#endregion EventClass
[Serializable] class ValueNode
{
    public GameObject targetObject;
    public string className;
    public string valueName;
    public string targetValue;

    public bool CheckValue()
    {
        // className�� ���� ������Ʈ Ÿ���� ��
        Type componentType = Type.GetType(className);
        if (componentType == null)
        {
            // �ش� Ÿ���� ������Ʈ�� ���� ��� ���� �޽����� ����ϰ� null�� ��ȯ
            Debug.LogError("No component of type " + className + " found.");
            return false;
        }

        // targetObject���� ã�� ������Ʈ Ÿ���� �ν��Ͻ��� ��
        Component targetComponent = targetObject.GetComponent(componentType);
        if (targetComponent == null)
        {
            // �ش� ������Ʈ�� targetObject�� ���� ��� ���� �޽����� ����ϰ� null�� ��ȯ
            Debug.LogError("No component of type " + className + " found in " + targetObject.name);
            return false;
        }

        // ������Ʈ Ÿ�Կ��� valueName�� ��ġ�ϴ� �ʵ带 ã�´�.
        FieldInfo fieldInfo = componentType.GetField(valueName);
        if (fieldInfo == null)
        {
            // �ش� �ʵ尡 ���� ��� ���� �޽����� ����ϰ� null�� ��ȯ
            Debug.LogError("No field of name " + valueName + " found in " + className);
            return false;
        }

        // �ʵ��� ���� ��
        object value = fieldInfo.GetValue(targetComponent);

        // ������ Ÿ�� ����� �� �� ��ȯ
        return value?.ToString() == targetValue;
    }
}
[Serializable] class OnBuildValueCheck
{
    [SerializeField] public float currentTime0 = 0f;
    [SerializeField] public float currentTime1 = 0f;
    [SerializeField] public bool firstActive = false;
}

public class Trigger_Requiem : MonoBehaviour
{
    [SerializeField] public List<string> objectTagList; // ���� ���� �±� ����Ʈ
    [SerializeField] List<ValueNode> valueCheckList; // üũ�ؾ� �ϴ� ���� ����Ʈ
    [SerializeField] public bool isActive = false; // �ߵ� ����
    [SerializeField] public bool interactionCheck = true; // �ٸ� ��ü�� �����ؾ� �ϴ°� ����
    [SerializeField] public bool valueCheck = false; // Ư�� ������ ���� üũ�ϴ°� ����
    [SerializeField] public bool repeatCheck = false;  // �ݺ� �ϴ°� ����
    [SerializeField] public float delayTime = 0f; // �ߵ� ������
    [SerializeField] public float repeatDeleyTime = 0f; // �ݺ� ������
    [SerializeField] public int repeatCount = -1; // �ݺ� Ƚ�� / -1�̸� ����

    [Header("Event List")]
    [SerializeField] OnTriggerEvent onTriggerEvent = new OnTriggerEvent();
    [SerializeField] OnCollisionEvent onCollisionEvent = new OnCollisionEvent();
    [SerializeField] TimeTriggerEvent timeTriggerEvent = new TimeTriggerEvent();
    [SerializeField] List<OnButtonEventNode> onButtonEventNodes;

    [Header("Programer")]
    [SerializeField] OnBuildValueCheck onBuildValueCheck = new OnBuildValueCheck();


    private void Start()
    {
        if (delayTime < 0f) delayTime = 0f;
        if (repeatDeleyTime < 0f) repeatDeleyTime = 0f;
    }

    private void Update()
    {
        if (!interactionCheck)
        {
            if (valueCheck && !CompareTargetValue()) return;

            if (!repeatCheck)
            {
                if (onBuildValueCheck.currentTime0 >= delayTime && !isActive)
                {
                    timeTriggerEvent.TimeEvent?.Invoke();
                    isActive = true;
                }
                else onBuildValueCheck.currentTime0 += Time.deltaTime;
            }
            else
            {
                if (repeatCount == 0) return;

                if (onBuildValueCheck.currentTime0 >= delayTime)
                {
                    if (onBuildValueCheck.currentTime1 >= repeatDeleyTime)
                    {
                        timeTriggerEvent.TimeEvent?.Invoke();
                        onBuildValueCheck.currentTime0 = 0f;

                        if (repeatCount <= -2) repeatCount = -1;
                        else repeatCount--;
                    }
                    else onBuildValueCheck.currentTime1 += Time.deltaTime;
                }
                else onBuildValueCheck.currentTime0 += Time.deltaTime;
            }
        }

        foreach (var item in onButtonEventNodes)
        {
            if (Input.GetKey(item.m_keyCode))
            {
                item.onButtonEvent?.Invoke();
            }
        }
    }

    #region Ʈ���� �Լ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!repeatCheck)
        {
            if (valueCheck && !CompareTargetValue()) return;

            if (CompareObjectTag(collision) && !isActive)
            {
                isActive = true;
                StartCoroutine(DelayedTriggerEnter());
            }
        }
    }

    private IEnumerator DelayedTriggerEnter()
    {
        yield return new WaitForSeconds(delayTime);
        onTriggerEvent.TriggerEnter?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (repeatCheck)
        {
            if (valueCheck && !CompareTargetValue()) return;

            if (CompareObjectTag(collision) && !isActive && !onBuildValueCheck.firstActive)
            {
                isActive = true;
                onBuildValueCheck.firstActive = true;
                StartCoroutine(DelayedTriggerStay());
            }
            else if (CompareObjectTag(collision) && !isActive)
            {
                if (repeatCount == 0) return;
                isActive = true;
                StartCoroutine(DelayedTriggerStayRepeat());
            }
        }
    }

    private IEnumerator DelayedTriggerStay()
    {
        yield return new WaitForSeconds(delayTime);
        onTriggerEvent.TriggerStay?.Invoke();
        isActive = false;
    }

    private IEnumerator DelayedTriggerStayRepeat()
    {
        yield return new WaitForSeconds(repeatDeleyTime);
        onTriggerEvent.TriggerStay?.Invoke();
        isActive = false;

        if (repeatCount <= -2) repeatCount = -1;
        else repeatCount--;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (valueCheck && !CompareTargetValue()) return;
        if (CompareObjectTag(collision))
        {
            onBuildValueCheck.firstActive = false;
            StartCoroutine(DelayedTriggerExit());
        }
    }

    private IEnumerator DelayedTriggerExit()
    {
        yield return new WaitForSeconds(0.01f);
        onTriggerEvent.TriggerExit?.Invoke();
    }
    #endregion Ʈ���� �Լ�

    #region �ݸ��� �Լ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!repeatCheck)
        {
            if (valueCheck && !CompareTargetValue()) return;

            if (CompareObjectTag(collision) && !isActive)
            {
                isActive = true;
                StartCoroutine(DelayedCollisionEnter());
            }
        }
    }

    private IEnumerator DelayedCollisionEnter()
    {
        yield return new WaitForSeconds(delayTime);
        onCollisionEvent.CollisionEnter?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (repeatCheck)
        {
            if (valueCheck && !CompareTargetValue()) return;

            if (CompareObjectTag(collision) && !isActive && !onBuildValueCheck.firstActive)
            {
                isActive = true;
                onBuildValueCheck.firstActive = true;
                StartCoroutine(DelayedCollisionStay());
            }
            else if (CompareObjectTag(collision) && !isActive)
            {
                if (repeatCount == 0) return;
                StartCoroutine(DelayedCollisionStayRepeat());
            }
        }
    }

    private IEnumerator DelayedCollisionStay()
    {
        yield return new WaitForSeconds(delayTime);
        onCollisionEvent.CollisionStay?.Invoke();
        isActive = false;
    }

    private IEnumerator DelayedCollisionStayRepeat()
    {
        yield return new WaitForSeconds(repeatDeleyTime);
        onCollisionEvent.CollisionStay?.Invoke();
        isActive = false;

        if (repeatCount <= -2) repeatCount = -1;
        else repeatCount--;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (valueCheck && !CompareTargetValue()) return;
        if (CompareObjectTag(collision)) StartCoroutine(DelayedCollisionExit());
    }
    private IEnumerator DelayedCollisionExit()
    {
        yield return new WaitForSeconds(0.01f);
        onCollisionEvent.CollisionExit?.Invoke();
    }
    #endregion �ݸ��� �Լ�

    #region �±� �� �Լ�
    bool CompareObjectTag(Collider2D collision)
    {
        foreach (var item in objectTagList)
        {
            if (item == collision.tag) return true;
        }

        return false;
    }

    bool CompareObjectTag(Collision2D collision)
    {
        foreach (var item in objectTagList)
        {
            if (item == collision.gameObject.tag) return true;
        }

        return false;
    }
    #endregion �±� �� �Լ�

    bool CompareTargetValue()
    {
        if (valueCheckList == null) return true;

        foreach (var item in valueCheckList)
        {
            if (!item.CheckValue()) return false;
        }

        return true;
    }



#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Handles.Label(transform.position, gameObject.tag);
    }
#endif
}

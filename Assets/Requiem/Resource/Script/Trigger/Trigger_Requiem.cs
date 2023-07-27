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
        // className을 통해 컴포넌트 타입을 겟
        Type componentType = Type.GetType(className);
        if (componentType == null)
        {
            // 해당 타입의 컴포넌트가 없을 경우 오류 메시지를 출력하고 null을 반환
            Debug.LogError("No component of type " + className + " found.");
            return false;
        }

        // targetObject에서 찾은 컴포넌트 타입의 인스턴스를 겟
        Component targetComponent = targetObject.GetComponent(componentType);
        if (targetComponent == null)
        {
            // 해당 컴포넌트가 targetObject에 없을 경우 오류 메시지를 출력하고 null을 반환
            Debug.LogError("No component of type " + className + " found in " + targetObject.name);
            return false;
        }

        // 컴포넌트 타입에서 valueName과 일치하는 필드를 찾는다.
        FieldInfo fieldInfo = componentType.GetField(valueName);
        if (fieldInfo == null)
        {
            // 해당 필드가 없을 경우 오류 메시지를 출력하고 null을 반환
            Debug.LogError("No field of name " + valueName + " found in " + className);
            return false;
        }

        // 필드의 값을 겟
        object value = fieldInfo.GetValue(targetComponent);

        // 변수를 타겟 밸류와 비교 후 반환
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
    [SerializeField] public List<string> objectTagList; // 접촉 조건 태그 리스트
    [SerializeField] List<ValueNode> valueCheckList; // 체크해야 하는 변수 리스트
    [SerializeField] public bool isActive = false; // 발동 여부
    [SerializeField] public bool interactionCheck = true; // 다른 객체와 접촉해야 하는가 여부
    [SerializeField] public bool valueCheck = false; // 특정 변수의 값을 체크하는가 여부
    [SerializeField] public bool repeatCheck = false;  // 반복 하는가 여부
    [SerializeField] public float delayTime = 0f; // 발동 딜레이
    [SerializeField] public float repeatDeleyTime = 0f; // 반복 딜레이
    [SerializeField] public int repeatCount = -1; // 반복 횟수 / -1이면 무한

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

    #region 트리거 함수
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
    #endregion 트리거 함수

    #region 콜리전 함수
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
    #endregion 콜리전 함수

    #region 태그 비교 함수
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
    #endregion 태그 비교 함수

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

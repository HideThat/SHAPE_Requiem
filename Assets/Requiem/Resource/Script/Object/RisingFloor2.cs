using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RisingFloor2))]
public class RisingFloor2Editor : Editor
{
    private SerializedProperty wallTriggerProp;
    private SerializedProperty moveTimeProp;
    private SerializedProperty moveDistanceProp;
    private SerializedProperty isActiveProp;
    private SerializedProperty showOtherProp;
    private SerializedProperty targetPosProp;
    private SerializedProperty originPosProp;
    private SerializedProperty currentTimeProp;

    private void OnEnable()
    {
        wallTriggerProp = serializedObject.FindProperty("wallTrigger");
        moveTimeProp = serializedObject.FindProperty("moveTime");
        moveDistanceProp = serializedObject.FindProperty("moveDistance");
        isActiveProp = serializedObject.FindProperty("isActive");
        showOtherProp = serializedObject.FindProperty("showOther");
        targetPosProp = serializedObject.FindProperty("targetPos");
        originPosProp = serializedObject.FindProperty("originPos");
        currentTimeProp = serializedObject.FindProperty("currentTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(wallTriggerProp);
        EditorGUILayout.PropertyField(moveTimeProp);
        EditorGUILayout.PropertyField(moveDistanceProp);
        EditorGUILayout.PropertyField(isActiveProp);
        EditorGUILayout.PropertyField(showOtherProp);

        RisingFloor2 risingFloor2 = (RisingFloor2)target;

        if (risingFloor2.showOther)
        {
            EditorGUILayout.PropertyField(targetPosProp);
            EditorGUILayout.PropertyField(originPosProp);
            EditorGUILayout.PropertyField(currentTimeProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif


public class RisingFloor2 : MonoBehaviour
{
    [SerializeField] public WallTrigger wallTrigger;
    [SerializeField] private float moveTime = 3f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] public bool isActive = false;
    [Header("���α׷���")]
    [SerializeField] public bool showOther = false;
    [SerializeField] Vector2 targetPos;
    [SerializeField] Vector2 originPos;
    [SerializeField] float currentTime = 5.9f;


    void Start()
    {
        transform.GetChild(0).parent = null;
        originPos = transform.position;
        targetPos = new Vector2(originPos.x, originPos.y + moveDistance);
        currentTime = moveTime * 2f - 0.1f;
    }

    private void FixedUpdate()
    {
        if (isActive && currentTime > moveTime * 2)
        {
            StartCoroutine(FloorMove());
            currentTime = 0f;
        }
        else if(isActive && currentTime <= moveTime * 2)
        {
            currentTime += Time.deltaTime;
        }

        isActive = wallTrigger.isActive;
    }


    IEnumerator FloorMove()
    {
        Vector2 moveDirection = transform.up;  // ������Ʈ�� ���� Y�� ������ �̵� �������� ���
        Vector2 targetPosition = (Vector2)transform.position + moveDirection * moveDistance;

        transform.DOMove(targetPosition, moveTime);

        yield return new WaitForSeconds(moveTime);

        transform.DOMove(originPos, moveTime);
    }

    public void ResetFloor()
    {
        // ��� ���� �ߴ�
        StopAllCoroutines();
        transform.DOKill();

        // ��ġ�� ���� ��ġ�� �缳��
        transform.position = originPos;

        // ���� �������� �ʱ� ���·� �缳��
        isActive = false;
        currentTime = moveTime * 2f - 0.1f;
    }
}

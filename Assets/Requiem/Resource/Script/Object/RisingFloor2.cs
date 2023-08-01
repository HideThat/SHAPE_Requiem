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
    private SerializedProperty audioSourceProp;

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
        audioSourceProp = serializedObject.FindProperty("audioSource");
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
            EditorGUILayout.PropertyField(audioSourceProp);
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
    [Header("프로그래머")]
    [SerializeField] public bool showOther = false;
    [SerializeField] Vector2 targetPos;
    [SerializeField] Vector2 originPos;
    [SerializeField] float currentTime = 5.9f;
    [SerializeField] AudioSource audioSource;


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
        Vector2 moveDirection = transform.up;  // 오브젝트의 현재 Y축 방향을 이동 방향으로 사용
        Vector2 targetPosition = (Vector2)transform.position + moveDirection * moveDistance;

        transform.DOMove(targetPosition, moveTime);

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        yield return new WaitForSeconds(moveTime);

        transform.DOMove(originPos, moveTime);
    }

    public void ResetFloor()
    {
        // 모든 동작 중단
        StopAllCoroutines();
        transform.DOKill();

        // 위치를 원래 위치로 재설정
        transform.position = originPos;

        // 상태 변수들을 초기 상태로 재설정
        isActive = false;
        currentTime = moveTime * 2f - 0.1f;
    }
}

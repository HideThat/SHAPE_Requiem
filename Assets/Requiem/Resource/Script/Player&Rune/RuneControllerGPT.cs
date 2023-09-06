using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System;
using UnityEngine.Rendering.Universal.Internal;

public enum ArmState
{
    Front,
    Up,
    Down,
    Return
}

[Serializable]
public class ArmStatePair
{
    public ArmState state;
    public GameObject armObject;
}

public class RuneControllerGPT : Singleton<RuneControllerGPT>
{
    public float moveTime; // �� �̵� �ð�
    public bool isMouseDelay;
    public float clickDelay;

    [SerializeField] Transform target;
    [SerializeField] GameObject magicCircle;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform leftArm;
    [SerializeField] List<ArmStatePair> StandArms = new List<ArmStatePair>();

    public bool m_isGetRune; // �� ȹ�� ����

    private LayerMask layerMask; // �浹 ���� ���̾� ����ũ

    public Tween runeMoveTween;


    public enum RuneControlMode
    {
        Click,
        Follow,
        Keyboard
    }

    void Start()
    {
        InitializeRuneController(); // �� ��Ʈ�ѷ� �ʱ�ȭ
    }

    void Update()
    {
        FollowMouse();
    }

    private void InitializeRuneController()
    {
        layerMask = LayerMask.GetMask("Platform", "Wall", "RiskFactor");
    }

    ArmState DetermineArmState(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return ArmState.Front;
        else if (direction.y > 0)
            return ArmState.Up;
        else
            return ArmState.Down;
    }

    void StandArmSummon(List<ArmStatePair> _Arms, float _xDiration, ArmState _armState)
    {
        #region ����ڵ�
        // _Arms�� null�̰ų� ��� �ִ� ��� �α׸� ����ϰ� �޼��带 �����մϴ�.
        if (_Arms == null || _Arms.Count == 0)
        {
            Debug.LogError("Arms list is null or empty!");
            return;
        }

        // rightArm�� leftArm�� ���� ��� �α׸� ����ϰ� �޼��带 �����մϴ�.
        if (rightArm == null || leftArm == null)
        {
            Debug.LogError("Right or Left Arm Transform is missing!");
            return;
        }
        #endregion
        int index = GetArmIndex(_Arms, _armState);
        if (index == -1)
        {
            Debug.Log("�� �����ϴµ� ���� �������� ����");
            return;
        }

        GameObject newArm = Instantiate(_Arms[index].armObject);
        Transform targetArm = DetermineTargetArm(_xDiration);

        newArm.transform.rotation = DetermineRotation(_xDiration);
        newArm.transform.position = targetArm.position;
        newArm.GetComponent<StandArm>().followPoint = targetArm.position - transform.position;
    }

    int GetArmIndex(List<ArmStatePair> _Arms, ArmState _armState)
    {
        for (int i = 0; i < _Arms.Count; i++)
            if (_Arms[i].state == _armState)
                return i;

        return -1;
    }

    Transform DetermineTargetArm(float _xDiration)
    {
        if (transform.rotation.y == 0f)
            return _xDiration >= 0f ? rightArm : leftArm;
        else
            return _xDiration >= 0f ? leftArm : rightArm;
    }

    Quaternion DetermineRotation(float _xDiration)
    {
        if (transform.rotation.y == 0f)
            return _xDiration >= 0f ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);
        else
            return _xDiration >= 0f ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);
    }

    GameObject MagicCircleSummon(Vector2 _point)
    {
        GameObject newCircle = Instantiate(magicCircle);
        newCircle.transform.position = _point;

        return newCircle;
    }

    private void ResetMouseDelay()
    {
        isMouseDelay = false;
    }

    void FollowMouse()
    {
        DOTween.Kill(runeMoveTween);

        Vector3 mousePosition = Input.mousePosition; // ���콺�� ��ũ�� ��ǥ�� �����ɴϴ�.
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane)); // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.

        // z ��ǥ�� ������Ʈ�� ���� z ��ǥ�� �����մϴ�. (�ʿ��ϴٸ�)
        mouseWorldPosition.z = transform.position.z;

        runeMoveTween = transform.DOMove(mouseWorldPosition, moveTime);
    }

    Transform CheckTarget()
    {
        return null;
        //return Physics2D.OverlapCircle()
    }
}

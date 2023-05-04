// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneControllerGPT : MonoBehaviour
{
    public Vector2 target; // ���� �̵� ��ǥ ��ġ
    public float moveTime; // �� �̵� �ð�
    public bool isShoot; // ���� �߻�Ǿ����� ����

    [SerializeField] private Vector2 runePosition; // ���� �ʱ� ��ġ
    [SerializeField] private Vector2 origin; // ���� ���� ��ġ
    [SerializeField] private float shootDelayTime; // �߻� ���� �ð�
    [SerializeField] private float runeReturnDistance; // ���� �ڵ����� �ǵ��ƿ� �Ÿ�
    [SerializeField] private bool isMouseDelay = false; // ���콺 Ŭ�� ���� ����

    private GameObject runeObj; // �� ���� ������Ʈ
    private RuneSoundManager runeSoundManager; // �� ���� ������
    private Light2D runeSight; // ���� ���� ������Ʈ
    private LayerMask layerMask; // �浹 ���� ���̾� ����ũ

    void Start()
    {
        InitializeRuneController(); // �� ��Ʈ�ѷ� �ʱ�ȭ
    }

    void Update()
    {
        if (PlayerData.PlayerIsGetRune)
        {
            RuneColliding(); // �� �浹 ó��
            RuneControl(); // �� ����
            RuneMove(); // �� �̵�
        }
    }

    private void InitializeRuneController()
    {
        runeObj = RuneData.RuneObj;
        runeSoundManager = RuneData.RuneObj.GetComponent<RuneSoundManager>();
        runeSight = RuneData.RuneObj.GetComponent<Light2D>();
        runeObj.transform.parent = null;
        RuneData.RuneActive = false;
        runeObj.SetActive(true);
        target = transform.position;
        isShoot = false;
        layerMask = LayerMask.GetMask("Platform", "Wall", "RiskFactor");

        ValidateComponents(); // ������Ʈ ��ȿ�� �˻�
    }

    // ������Ʈ ��ȿ�� �˻�
    private void ValidateComponents()
    {
        if (runeObj == null) Debug.Log("m_runeObj == null");
        if (runeSoundManager == null) Debug.Log("m_runeSoundManager == null");
        if (runeSight == null) Debug.Log("m_runeSight == null");
    }

    // �� ����
    private void RuneControl()
    {
        if (RuneData.RuneUseControl)
        {
            HandleRuneShoot(); // �� �߻� ó��
            HandleRuneReturn(); // �� ��ȯ ó��
            HandleRunePower(); // �� �Ŀ� ó��
        }
    }

    // �� �̵�
    private void RuneMove()
    {
        runeObj.transform.DOMove(target, moveTime);
    }

    // ���콺 ��ġ�� ��ǥ ����
    private void ChangeTargetToMouse()
    {
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    // �� ��ȯ
    private void ReturnRune()
    {
        if (transform.rotation.y == 0)
        {
            target = new Vector2(transform.position.x + runePosition.x, transform.position.y + runePosition.y);
        }
        else if (transform.rotation.y != 0f)
        {
            target = new Vector2(transform.position.x + (-runePosition.x), transform.position.y + runePosition.y);
        }
        RuneData.RuneActive = isShoot;
        RuneData.RuneLightArea.enabled = false;
    }

    // �� �߻� ó��
    private void HandleRuneShoot()
    {
        if (Input.GetMouseButtonDown(0) && !isMouseDelay)
        {
            if (isShoot)
            {
                runeSoundManager.PlayRuneOff();
                isShoot = false;
                isMouseDelay = true;
                StartCoroutine("MouseClickDelay");
            }
            else
            {
                isShoot = true;

                if (!RuneData.RuneTouchWater)
                {
                    RuneData.RuneActive = true;
                    RuneData.RuneLightArea.enabled = true;
                }
                ChangeTargetToMouse();
                isMouseDelay = true;
                runeSoundManager.PlayRuneOn();
                StartCoroutine("MouseClickDelay");
            }
        }
        else if (!isShoot)
        {
            ReturnRune();
        }
    }

    // �� ��ȯ ó��
    private void HandleRuneReturn()
    {
        if (Vector2.Distance(runeObj.transform.position, transform.position) >= RuneReturnDistance())
        {
            isShoot = false;
            isMouseDelay = true;
            runeSoundManager.PlayRuneOff();
            StartCoroutine(MouseClickDelay());
        }
    }

    // �� ��ȯ �Ÿ� ����
    private float RuneReturnDistance()
    {
        return runeReturnDistance;
    }

    // �� �Ŀ� ó��
    private void HandleRunePower()
    {
        if (RuneData.RuneOnWater)
        {
            RunePowerLose(); // �� �Ŀ� ����
        }
        else
        {
            RunePowerBack(); // �� �Ŀ� ȸ��
        }
        RuneReturnDistance();
    }

    // �� �Ŀ� ȸ��
    private void RunePowerBack()
    {
        if (Vector2.Distance(transform.position, runeObj.transform.position) <= RuneData.RunePowerBackDistance && !RuneData.RuneOnWater)
        {
            RuneData.RuneTouchWater = false;
            RuneData.RuneLightArea.enabled = true;
            DOTween.To(() => runeSight.pointLightOuterRadius, x => runeSight.pointLightOuterRadius = x, RuneData.RuneOuterRadius, RuneData.RunePowerBackTime);
        }
    }

    // �� �Ŀ� ����
    private void RunePowerLose()
    {
        DOTween.To(() => runeSight.pointLightOuterRadius, x => runeSight.pointLightOuterRadius = x, 0f, RuneData.RunePowerBackTime);
    }

    // �� �̵� ����
    public void RuneStop()
    {
        target = runeObj.transform.position;
    }

    // ���콺 Ŭ�� ���� �ڷ�ƾ
    private IEnumerator MouseClickDelay()
    {
        yield return new WaitForSeconds(shootDelayTime);
        isMouseDelay = false;
    }

    // �� �浹 ó��
    private void RuneColliding()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(runeObj.transform.position,
            (target - (Vector2)runeObj.transform.position).normalized,
            Vector2.Distance(runeObj.transform.position, target),
            layerMask);

        if (hit.collider != null &&
            (hit.collider.gameObject.layer == (int)LayerName.Platform ||
            hit.collider.gameObject.layer == (int)LayerName.Wall ||
            hit.collider.gameObject.layer == (int)LayerName.RiskFactor))
        {
            target = hit.point;
        }
    }
}

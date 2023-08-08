using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneControllerGPT : MonoBehaviour
{
    public Vector2 target; // 룬의 이동 목표 위치
    public float moveTime; // 룬 이동 시간
    public bool isShoot; // 룬이 발사되었는지 여부
    public bool isCharge = false;
    [SerializeField] private Vector2 runePosition; // 룬의 초기 위치
    [SerializeField] private Vector2 origin; // 룬의 원점 위치
    [SerializeField] private float shootDelayTime; // 발사 지연 시간
    [SerializeField] private float runeReturnDistance; // 룬이 자동으로 되돌아올 거리
    [SerializeField] private bool isMouseDelay = false; // 마우스 클릭 지연 여부
    [SerializeField] public float batteryDrainSpeed = 1f;
    [SerializeField] public float additionalMovementReduction = 50f;
    [SerializeField] public ParticleSystem runeCharge;
    [SerializeField] SpriteRenderer[] batteryUI;
    [SerializeField] SpriteRenderer batteryBorder;
    [SerializeField] RuneManager runeManager;
    [SerializeField] LayerMask hitLayerMask;

    public bool m_isGetRune; // 룬 획득 판정

    private GameObject runeObj; // 룬 게임 오브젝트
    private Light2D runeSight; // 룬의 조명 컴포넌트
    private LayerMask layerMask; // 충돌 감지 레이어 마스크

    void Start()
    {
        InitializeRuneController(); // 룬 컨트롤러 초기화
    }
    void Update()
    {
        if (m_isGetRune)
        {
            RuneControl(); // 룬 제어
            RuneMove(); // 룬 이동
            RuneCharging(); // 룬 충전 효과
        }
    }
    private void InitializeRuneController()
    {
        runeObj = RuneData.RuneObj;
        runeSight = RuneData.RuneObj.GetComponent<Light2D>();
        runeObj.transform.parent = null;
        RuneData.Instance.isActive = false;
        runeObj.SetActive(true);
        target = transform.position;
        isShoot = false;
        layerMask = LayerMask.GetMask("Platform", "Wall", "RiskFactor");

        SetBatteryUIVisible(false, RuneData.Instance.battery / 1000f);

        if (runeObj == null) Debug.Log("m_runeObj == null");
        if (runeSight == null) Debug.Log("m_runeSight == null");
    }
    private void RuneControl()
    {
        if (!RuneData.Instance.useControl) return;

        HandleRuneShoot(); // 룬 발사 처리
        HandleRuneReturn(); // 룬 반환 처리

        if (isShoot)
        {
            DecreaseBatteryWhileShooting();
            UpdateBatteryUI();
        }
    }
    private void DecreaseBatteryWhileShooting()
    {
        if (RuneData.Instance.battery > 0) RuneData.Instance.battery -= batteryDrainSpeed * Time.deltaTime;
        else if (RuneData.Instance.battery <= 0 && !RuneData.Instance.useControl)
        {
            RunePowerLose();
            RuneData.Instance.battery = 0f;
        }
    }
    private void UpdateBatteryUI()
    {
        if (!isShoot) return;

        float batteryPercentage = RuneData.Instance.battery / 1000f;
        Color color;

        if (batteryPercentage > 0.75f) color = Color.green;
        else if (batteryPercentage > 0.25f) color = Color.yellow;
        else color = Color.red;

        if (batteryPercentage > 0) StartCoroutine(ActivateBatteryUISequentially(true, batteryPercentage));

        foreach (var ui in batteryUI) ui.DOColor(color, 5f);
    }
    private IEnumerator ActivateBatteryUISequentially(bool isVisible, float batteryPercentage)
    {
        if (isVisible)
        {
            int visibleUIElements = 0;

            if (batteryPercentage > 0.75f) visibleUIElements = 4;
            else if (batteryPercentage > 0.50f)
            {
                visibleUIElements = 3;
                batteryUI[3].gameObject.SetActive(false);
            }
            else if (batteryPercentage > 0.25f)
            {
                visibleUIElements = 2;
                batteryUI[3].gameObject.SetActive(false);
                batteryUI[2].gameObject.SetActive(false);
            }
            else
            {
                visibleUIElements = 1;
                batteryUI[3].gameObject.SetActive(false);
                batteryUI[2].gameObject.SetActive(false);
                batteryUI[1].gameObject.SetActive(false);
            }

            for (int i = 0; i < visibleUIElements; i++)
            {
                batteryUI[i].gameObject.SetActive(isVisible);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            foreach (var ui in batteryUI)
            {
                ui.gameObject.SetActive(isVisible);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    private void RuneMove()
    {
        MoveRuneToTarget();
    }
    private void MoveRuneToTarget()
    {
        if (isShoot)
        {
            Vector2 dir = target - (Vector2)runeObj.transform.position;
            float dis = Vector2.Distance(target, (Vector2)runeObj.transform.position);

            RaycastHit2D rayHit = Physics2D.Raycast(runeObj.transform.position, dir, dis, hitLayerMask);

            if (rayHit) runeObj.transform.DOMove(rayHit.point, moveTime);
            else runeObj.transform.DOMove(target, moveTime);
        }
        else runeObj.transform.DOMove(target, moveTime);
    }
    private void HandleMouseClicksForShooting()
    {
        if (Input.GetMouseButtonDown(1) && !isMouseDelay && isShoot)
        {
            PlayRuneOffSoundAndDelayMouse();
            isShoot = false;
            SetBatteryUIVisible(false, RuneData.Instance.battery / 1000f);
        }
        else if (Input.GetMouseButtonDown(0) && !isMouseDelay)
        {
            ChangeTargetToMouse();
            PlayRuneOnSoundAndDelayMouse();
            HandleShootingWithBattery();
            runeManager.RuneShootSoundPlay();
            RuneData.Instance.isReturn = false;
        }
        else if (!isShoot) ReturnRune();
    }
    private void PlayRuneOffSoundAndDelayMouse()
    {
        isMouseDelay = true;
        StartCoroutine(MouseClickDelay());
    }

    private void PlayRuneOnSoundAndDelayMouse()
    {
        isMouseDelay = true;
        StartCoroutine(MouseClickDelay());
    }

    private void HandleShootingWithBattery()
    {
        RuneData.Instance.isActive = true;
        RuneData.RuneLightArea.enabled = true;

        if (isShoot && RuneData.Instance.battery > 0) RuneData.Instance.battery -= additionalMovementReduction;

        isShoot = true;
        SetBatteryUIVisible(true, RuneData.Instance.battery / 1000f);
    }


    // 마우스 위치로 목표 변경
    private void ChangeTargetToMouse()
    {
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    // 룬 반환
    private void ReturnRune()
    {
        if (transform.rotation.y == 0) target = new Vector2(transform.position.x + runePosition.x, transform.position.y + runePosition.y);
        else if (transform.rotation.y != 0f) target = new Vector2(transform.position.x + (-runePosition.x), transform.position.y + runePosition.y);
        if (!RuneData.Instance.isReturn) runeManager.RuneReturnSoundPlay();
        RuneData.Instance.isReturn = true;
        RuneData.Instance.isActive = false;
        RuneData.RuneLightArea.enabled = false;
    }

    // 룬 발사 처리
    private void HandleRuneShoot()
    {
        // 마우스 클릭에 따른 발사 또는 배터리 UI 숨기기 처리
        HandleMouseClicksForShooting();
    }

    private void SetBatteryUIVisible(bool isVisible, float batteryPercentage)
    {
        batteryBorder.gameObject.SetActive(isVisible);
        StartCoroutine(ActivateBatteryUISequentially(isVisible, batteryPercentage));
    }

    // 룬 반환 처리
    private void HandleRuneReturn()
    {
        if (RuneIsFarEnough()) StopShooting();
    }

    private bool RuneIsFarEnough()
    {
        return Vector2.Distance(runeObj.transform.position, transform.position) >= RuneReturnDistance();
    }

    private void StopShooting()
    {
        isShoot = false;
        isMouseDelay = true;
        StartCoroutine(MouseClickDelay());
    }

    // 룬 파워 감소
    public void RunePowerLose()
    {
        RuneData.Instance.isPowerLose = true;
        runeManager.RuneBatteryDepletionSoundPlay();
        DecreaseRunePowerOverTime(0f, RuneData.Instance.runePowerBackTime);
    }

    // 룬 파워 회복
    public void RunePowerBack()
    {
        RuneData.Instance.isPowerLose = false;
        RuneData.RuneLightArea.enabled = true;
        DecreaseRunePowerOverTime(RuneData.Instance.runeOuterRadius, RuneData.Instance.runePowerBackTime);
    }

    private void DecreaseRunePowerOverTime(float targetRadius, float duration)
    {
        DOTween.To(() => runeSight.pointLightOuterRadius, x => runeSight.pointLightOuterRadius = x, targetRadius, duration);
    }


    // 룬 반환 거리 설정
    private float RuneReturnDistance()
    {
        return runeReturnDistance;
    }

    // 룬 이동 중지
    public void RuneStop()
    {
        target = runeObj.transform.position;
    }

    // 마우스 클릭 지연 코루틴
    private IEnumerator MouseClickDelay()
    {
        yield return new WaitForSeconds(shootDelayTime);
        ResetMouseDelay();
    }

    private void ResetMouseDelay()
    {
        isMouseDelay = false;
    }

    private RaycastHit2D GetRaycastHit()
    {
        return Physics2D.Raycast(runeObj.transform.position,
            GetDirectionToTarget(),
            GetDistanceToTarget(),
            layerMask);
    }

    private Vector2 GetDirectionToTarget()
    {
        return (target - (Vector2)runeObj.transform.position).normalized;
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(runeObj.transform.position, target);
    }

    private bool HitObjectIsCollidable(RaycastHit2D hit)
    {
        return hit.collider != null &&
               (hit.collider.gameObject.layer == (int)LayerName.Platform ||
                hit.collider.gameObject.layer == (int)LayerName.Wall ||
                hit.collider.gameObject.layer == (int)LayerName.RiskFactor);
    }

    public bool hasRestarted = false;
    public void RuneCharging()
    {
        if (isCharge)
        {
            if (!hasRestarted)
            {
                runeCharge.Play();
                hasRestarted = true;
            }
        }
        else
        {
            runeCharge.Stop();
            hasRestarted = false;  // 파티클이 멈추면 다시 시작할 수 있도록 플래그를 리셋합니다.
        }
    }
}

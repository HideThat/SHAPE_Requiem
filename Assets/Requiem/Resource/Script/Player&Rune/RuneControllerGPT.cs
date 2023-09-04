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

public class RuneControllerGPT : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static RuneControllerGPT Instance { get; private set; }

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
    [SerializeField] SpriteRenderer[] batteryUI;
    [SerializeField] SpriteRenderer batteryBorder;
    [SerializeField] public RuneManager runeManager;
    [SerializeField] public ParticleSystem runeCharge;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] GameObject magicCircle;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform leftArm;
    [SerializeField] List<ArmStatePair> StandArms = new List<ArmStatePair>();
    [SerializeField] RuneControlMode controlMode;
    [SerializeField] float speed;

    public bool m_isGetRune; // 룬 획득 판정

    private GameObject runeObj; // 룬 게임 오브젝트
    private LayerMask layerMask; // 충돌 감지 레이어 마스크

    public Tween runeMoveTween;
    public enum RuneControlMode
    {
        Click,
        Follow,
        Keyboard
    }
    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        InitializeRuneController(); // 룬 컨트롤러 초기화
    }
    void Update()
    {
        switch (controlMode)
        {
            case RuneControlMode.Click:
                if (m_isGetRune)
                {
                    RuneControl(); // 룬 제어
                    RuneMove(); // 룬 이동
                    RuneCharging(); // 룬 충전 효과
                }
                break;
            case RuneControlMode.Follow:
                FollowMouse();
                break;
            case RuneControlMode.Keyboard:
                KeyMove();
                break;
            default:
                break;
        }
        
    }
    private void InitializeRuneController()
    {
        RuneManager.Instance.isActive = false;
        target = transform.position;
        isShoot = false;
        layerMask = LayerMask.GetMask("Platform", "Wall", "RiskFactor");

        SetBatteryUIVisible(false, RuneManager.Instance.battery / 1000f);

        if (runeObj == null) Debug.Log("m_runeObj == null");
    }
    private void RuneControl()
    {
        if (!RuneManager.Instance.useControl) return;

        if (runeManager == null)
        {
            runeManager = GameObject.Find("Rune").GetComponent<RuneManager>();
        }

        if (runeObj == null)
        {
            runeObj = RuneManager.Instance.runeObj;
        }

        if (runeManager != null)
        {
            runeCharge = runeManager.transform.GetComponentInChildren<ParticleSystem>();
        }


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
        if (RuneManager.Instance.battery > 0) RuneManager.Instance.battery -= batteryDrainSpeed * Time.deltaTime;
        else if (RuneManager.Instance.battery <= 0 && !RuneManager.Instance.useControl)
        {
            RuneManager.Instance.RunePowerLose();
            RuneManager.Instance.battery = 0f;
        }
    }
    private void UpdateBatteryUI()
    {
        if (!isShoot) return;

        float batteryPercentage = RuneManager.Instance.battery / 1000f;
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
        DOTween.Kill(runeMoveTween);

        runeMoveTween = runeObj.transform.DOMove(target, moveTime);

    }
    private void HandleMouseClicksForShooting()
    {
        if (Input.GetMouseButtonDown(1) && !isMouseDelay && isShoot)
        {
            PlayRuneOffSoundAndDelayMouse();
            isShoot = false;
            SetBatteryUIVisible(false, RuneManager.Instance.battery / 1000f);
            float xDiration = target.x - transform.position.x; // 플레이어랑 룬의 상대적 위치에 따라 방향 결정
            StandArmSummon(StandArms, xDiration, ArmState.Return);
            if (transform.rotation.y == 0f)
            {
                Vector2 circlePos = new Vector2(transform.position.x + runePosition.x, transform.position.y + runePosition.y);
                MagicCircleSummon(circlePos);
            }
            else
            {
                Vector2 circlePos = new Vector2(transform.position.x - runePosition.x, transform.position.y + runePosition.y);
                MagicCircleSummon(circlePos);
            }

        }
        else if (Input.GetMouseButtonDown(0) && !isMouseDelay)
        {
            ChangeTargetToMouse();
            PlayRuneOnSoundAndDelayMouse();
            HandleShootingWithBattery();
            runeManager.RuneShootSoundPlay();
            RuneManager.Instance.isReturn = false;
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
        RuneManager.Instance.isActive = true;

        if (isShoot && RuneManager.Instance.battery > 0) RuneManager.Instance.battery -= additionalMovementReduction;

        isShoot = true;
        SetBatteryUIVisible(true, RuneManager.Instance.battery / 1000f);
    }
    private void ChangeTargetToMouse()// 마우스 위치로 목표 변경
    {
        #region 방어코드
        // 카메라를 찾지 못한 경우 로그를 출력하고 메서드를 종료합니다.
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // runeObj가 없는 경우 로그를 출력하고 메서드를 종료합니다.
        if (runeObj == null)
        {
            Debug.LogError("Rune Object is missing!");
            return;
        }

        // magicCircle이 없는 경우 로그를 출력하고 메서드를 종료합니다.
        if (magicCircle == null)
        {
            Debug.LogError("Magic Circle prefab is missing!");
            return;
        }
        #endregion
        Vector2 newTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
        Input.mousePosition.y, -Camera.main.transform.position.z));

        //Vector2 dir = newTarget - (Vector2)runeObj.transform.position;
        //float dis = Vector2.Distance(newTarget, (Vector2)runeObj.transform.position);

        //RaycastHit2D rayHit = Physics2D.Raycast(runeObj.transform.position, dir, dis, hitLayerMask);
        //if (rayHit) newTarget = rayHit.point;

        MagicCircleSummon(newTarget);

        Vector2 directionAB = newTarget - target; // 점 A와 B 사이의 벡터
        ArmState _armState = DetermineArmState(directionAB);
        float xDiration = newTarget.x - transform.position.x;

        StandArmSummon(StandArms, xDiration, _armState);
        target = newTarget;
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
        #region 방어코드
        // _Arms가 null이거나 비어 있는 경우 로그를 출력하고 메서드를 종료합니다.
        if (_Arms == null || _Arms.Count == 0)
        {
            Debug.LogError("Arms list is null or empty!");
            return;
        }

        // rightArm과 leftArm이 없는 경우 로그를 출력하고 메서드를 종료합니다.
        if (rightArm == null || leftArm == null)
        {
            Debug.LogError("Right or Left Arm Transform is missing!");
            return;
        }
        #endregion
        int index = GetArmIndex(_Arms, _armState);
        if (index == -1)
        {
            Debug.Log("손 생성하는데 뭔가 오류났음 ㅋㅋ");
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
    private void ReturnRune()// 룬 반환
    {
        if (transform.rotation.y == 0) target = new Vector2(transform.position.x + runePosition.x, transform.position.y + runePosition.y);
        else if (transform.rotation.y != 0f) target = new Vector2(transform.position.x + (-runePosition.x), transform.position.y + runePosition.y);
        if (!RuneManager.Instance.isReturn) runeManager.RuneReturnSoundPlay();
        RuneManager.Instance.isReturn = true;
        RuneManager.Instance.isActive = false;
    }
    private void HandleRuneShoot()// 룬 발사 처리
    {
        HandleMouseClicksForShooting();// 마우스 클릭에 따른 발사 또는 배터리 UI 숨기기 처리
    }
    private void SetBatteryUIVisible(bool isVisible, float batteryPercentage)
    {
        batteryBorder.gameObject.SetActive(isVisible);
        StartCoroutine(ActivateBatteryUISequentially(isVisible, batteryPercentage));
    }
    private void HandleRuneReturn()// 룬 반환 처리
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
    private float RuneReturnDistance()// 룬 반환 거리 설정
    {
        return runeReturnDistance;
    }
    public void RuneStop()// 룬 이동 중지
    {
        target = runeObj.transform.position;
    }
    
    private IEnumerator MouseClickDelay()// 마우스 클릭 지연 코루틴
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
    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition; // 마우스의 스크린 좌표를 가져옵니다.
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane)); // 스크린 좌표를 월드 좌표로 변환합니다.

        // z 좌표를 오브젝트의 원래 z 좌표로 설정합니다. (필요하다면)
        mouseWorldPosition.z = runeManager.transform.position.z;

        runeManager.transform.DOMove(mouseWorldPosition, 1f);

        //runeManager.transform.position = mouseWorldPosition;
    }

    void KeyMove()
    {
        float horizontalInput = Input.GetAxis("Debug Horizontal");  // 수평 입력 (왼쪽 또는 오른쪽 방향키)
        float verticalInput = Input.GetAxis("Debug Vertical");  // 수직 입력 (위 또는 아래 방향키)

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);  // 움직일 방향을 설정

        movement = movement.normalized;
        runeManager.transform.Translate(movement * speed * Time.deltaTime);  // 실제로 오브젝트를 움직임
    }
}

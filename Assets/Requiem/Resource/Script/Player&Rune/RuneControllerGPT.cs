using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneControllerGPT : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static RuneControllerGPT Instance { get; private set; }

    public Vector2 target; // ���� �̵� ��ǥ ��ġ
    public float moveTime; // �� �̵� �ð�
    public bool isShoot; // ���� �߻�Ǿ����� ����
    public bool isCharge = false;
    [SerializeField] private Vector2 runePosition; // ���� �ʱ� ��ġ
    [SerializeField] private Vector2 origin; // ���� ���� ��ġ
    [SerializeField] private float shootDelayTime; // �߻� ���� �ð�
    [SerializeField] private float runeReturnDistance; // ���� �ڵ����� �ǵ��ƿ� �Ÿ�
    [SerializeField] private bool isMouseDelay = false; // ���콺 Ŭ�� ���� ����
    [SerializeField] public float batteryDrainSpeed = 1f;
    [SerializeField] public float additionalMovementReduction = 50f;
    [SerializeField] SpriteRenderer[] batteryUI;
    [SerializeField] SpriteRenderer batteryBorder;
    [SerializeField] public RuneManager runeManager;
    [SerializeField] public ParticleSystem runeCharge;
    [SerializeField] LayerMask hitLayerMask;

    public bool m_isGetRune; // �� ȹ�� ����

    private GameObject runeObj; // �� ���� ������Ʈ
    private LayerMask layerMask; // �浹 ���� ���̾� ����ũ

    public Tween runeMoveTween;

    void Awake()
    {
        // �̱��� ���� ����
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
        InitializeRuneController(); // �� ��Ʈ�ѷ� �ʱ�ȭ
    }
    void Update()
    {
        if (m_isGetRune)
        {
            RuneControl(); // �� ����
            RuneMove(); // �� �̵�
            RuneCharging(); // �� ���� ȿ��
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


        HandleRuneShoot(); // �� �߻� ó��
        HandleRuneReturn(); // �� ��ȯ ó��

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

        if (isShoot)
        {
            Vector2 dir = target - (Vector2)runeObj.transform.position;
            float dis = Vector2.Distance(target, (Vector2)runeObj.transform.position);

            RaycastHit2D rayHit = Physics2D.Raycast(runeObj.transform.position, dir, dis, hitLayerMask);

            if (rayHit) runeMoveTween = runeObj.transform.DOMove(rayHit.point, moveTime);
            else runeMoveTween = runeObj.transform.DOMove(target, moveTime);
        }
        else runeMoveTween = runeObj.transform.DOMove(target, moveTime);
    }
    private void HandleMouseClicksForShooting()
    {
        if (Input.GetMouseButtonDown(1) && !isMouseDelay && isShoot)
        {
            PlayRuneOffSoundAndDelayMouse();
            isShoot = false;
            SetBatteryUIVisible(false, RuneManager.Instance.battery / 1000f);
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
        RuneManager.Instance.runeLightArea.enabled = true;

        if (isShoot && RuneManager.Instance.battery > 0) RuneManager.Instance.battery -= additionalMovementReduction;

        isShoot = true;
        SetBatteryUIVisible(true, RuneManager.Instance.battery / 1000f);
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
        if (transform.rotation.y == 0) target = new Vector2(transform.position.x + runePosition.x, transform.position.y + runePosition.y);
        else if (transform.rotation.y != 0f) target = new Vector2(transform.position.x + (-runePosition.x), transform.position.y + runePosition.y);
        if (!RuneManager.Instance.isReturn) runeManager.RuneReturnSoundPlay();
        RuneManager.Instance.isReturn = true;
        RuneManager.Instance.isActive = false;
        RuneManager.Instance.runeLightArea.enabled = false;
    }

    // �� �߻� ó��
    private void HandleRuneShoot()
    {
        // ���콺 Ŭ���� ���� �߻� �Ǵ� ���͸� UI ����� ó��
        HandleMouseClicksForShooting();
    }

    private void SetBatteryUIVisible(bool isVisible, float batteryPercentage)
    {
        batteryBorder.gameObject.SetActive(isVisible);
        StartCoroutine(ActivateBatteryUISequentially(isVisible, batteryPercentage));
    }

    // �� ��ȯ ó��
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

    


    // �� ��ȯ �Ÿ� ����
    private float RuneReturnDistance()
    {
        return runeReturnDistance;
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
            hasRestarted = false;  // ��ƼŬ�� ���߸� �ٽ� ������ �� �ֵ��� �÷��׸� �����մϴ�.
        }
    }
}

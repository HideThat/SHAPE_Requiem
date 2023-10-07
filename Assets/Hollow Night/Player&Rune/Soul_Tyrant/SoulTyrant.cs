using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using static UnityEngine.AudioSettings;


// 1. 구체 발사
// 2. 3연속 구체 발사
// 3. 대쉬
// 4. 구체 대쉬
// 5. 내려찍기
// 6. 내려찍기 페이크
public class SoulTyrant : Enemy
{
    [Header("Soul_Tyrant")]
    [SerializeField] LayerMask player;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource voiceSource;
    [SerializeField] AudioSource effectSource;
    [SerializeField] Animator downStroke_Wait;
    [SerializeField] EffectDestroy downStroke_Active;
    [SerializeField] EffectDestroy downStroke_Fake;
    [SerializeField] EffectDestroy burstEffectPrefab;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] float trailSpacing = 0.5f;  // The spacing between each trail sprite
    public float appearDelay = 1f;
    [SerializeField] FocusEffect focusEffectPrefab;
    [SerializeField] Hulauf hulaufPrefab;
    [SerializeField] Wave wavePrefab;
    public float radius = 5f;
    
    public List<Transform> hulaufRushList;
    
    
    public float hulaufRushSpeed = 5f;
    public float preHulaufRushDelay = 1f;
    public float posHulaufRushDelay = 1f;
    public float hulaufShootSpeed = 1f;
    public float hulaufShootForce = 1f;
    public float downstrokePositionY = 1f;
    public float downstrokeMoveDistance = 1f;
    public float downstrokeMoveSpeed = 1f;
    public float waitDownstroke = 1f;
    public float preDownstrokeDelay = 1f;
    public float posDownstrokeDelay = 1f;
    public float preDownstrokeFakeDelay = 1f;
    public float posDownstrokeFakeDelay = 1f;
    public float preShootWaveDelay = 1f;
    public float middleShootWaveDelay = 0.4f;
    public float posShootWaveDelay = 3f;
    public float wavePositionY;
    public float waveCorrectionX;
    public Vector2 currentPosition;
    public bool isDead = false;

    [Header("RandomTeleport")]
    public List<Transform> teleportPointList;
    [SerializeField] EffectDestroy teleportEffect;
    [SerializeField] EffectDestroy teleportTrailPrefab;
    [SerializeField] EffectDestroy lightBlowPrefab;
    public float preRandomTeleportDelay = 0f;
    public float posRandomTeleportDelay = 1f;
    public AudioClip randomTeleportClip;

    [Header("fire")]
    [SerializeField] GameObject misilePrefab;
    [SerializeField] Transform launchPoint;
    public float preFireDelay = 1f;  // 미사일 발사 전 딜레이
    public float postFireDelay = 0.5f; // 미사일 발사 후 딜레이
    public AudioClip fireReadyClip;
    public AudioClip fireClip;
    public AudioClip fireLaugh;


    [Header("rush")]
    public List<Transform> rushPointList;
    public float rushSpeed = 5f;
    public float preRushDelay = 1f;
    public float posRushDelay = 1f;
    public AudioClip rushReadyClip;
    public AudioClip rushSpiritClip;
    public AudioClip rushMoveClip;
    public AudioClip rushBurstClip;


    GameObject targetObject;
    float scaleX;
    float scaleY;

    public Coroutine currentPatern;

    protected override void Start()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        targetObject = PlayerCoroutine.Instance.gameObject;
        StartCoroutine(StartAppear());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RandomTeleport(preRandomTeleportDelay, posRandomTeleportDelay);
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(RushPattern(preRushDelay, posRushDelay));
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(SphereHulaufRushPattern(preHulaufRushDelay, posHulaufRushDelay));
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(DownstrokePattern(targetObject.transform, preDownstrokeDelay, posDownstrokeDelay));
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(FakeDownstrokePattern(targetObject.transform, preDownstrokeFakeDelay, posDownstrokeFakeDelay));
        if (Input.GetKeyDown(KeyCode.Y))
            StartCoroutine(ShootWave(preShootWaveDelay, middleShootWaveDelay, posShootWaveDelay));
        if (Input.GetKeyDown(KeyCode.U))
            StartCoroutine(TeleportAndFireMisilePatternX3());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerCoroutine>().Hit(collision.transform.position, transform.position, damage);
    }

    

    IEnumerator StartAppear()
    {
        animator.Play("Soul_Tyrant_Meditation");
        PlayerCoroutine.Instance.animator.Play("Idle");
        PlayerCoroutine.Instance.enabled = false;
        targetObject.GetComponent<Animator>().Play("Idle");
        targetObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(appearDelay);
        animator.Play("Soul_Tyrant_Idle");
        PlayerCoroutine.Instance.enabled = true;
        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            int i = Random.Range(0, 8);
            switch (i)
            {
                case 0:
                    yield return currentPatern = StartCoroutine(RandomTeleport(preRandomTeleportDelay, posRandomTeleportDelay));
                    break;
                case 1:
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
                    break;
                case 2:
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePatternX3());
                    break;
                case 3:
                    yield return currentPatern = StartCoroutine(RushPattern(preRushDelay, posRushDelay));
                    break;
                case 4:
                    yield return currentPatern = StartCoroutine(SphereHulaufRushPattern(preHulaufRushDelay, posHulaufRushDelay));
                    break;
                case 5:
                    yield return currentPatern = StartCoroutine(DownstrokePattern(targetObject.transform, preDownstrokeDelay, posDownstrokeDelay));
                    break;
                case 6:
                    yield return currentPatern = StartCoroutine(FakeDownstrokePattern(targetObject.transform, preDownstrokeFakeDelay, preDownstrokeFakeDelay));
                    break;
                case 7:
                    yield return currentPatern = StartCoroutine(ShootWave(preShootWaveDelay, middleShootWaveDelay, posShootWaveDelay));
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    IEnumerator RandomTeleport(float _preDelay, float _posDelay)
    {
        yield return new WaitForSeconds(_preDelay);
        //voiceSource.PlayOneShot(randomTeleportClip);
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        PerformTeleport(GetRandomTeleportPoint());
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        yield return new WaitForSeconds(_posDelay);
    }

    IEnumerator TeleportAndFireMisilePattern(float _preDelay, float _posDelay)
    {
        yield return StartCoroutine(RandomTeleport(0f, 0f));
        animator.Play("Soul_Tyrant_Meditation");
        // 기모으기 이펙트 생성
        FocusEffect effect = Instantiate(focusEffectPrefab);
        effect.transform.position = launchPoint.position;
        effect.SetFocusEffect(_preDelay, 5 / _preDelay);

        effectSource.PlayOneShot(fireReadyClip);
        voiceSource.PlayOneShot(fireLaugh);
        yield return new WaitForSeconds(_preDelay);
        yield return StartCoroutine(FireMisile(0f));
        yield return new WaitForSeconds(_posDelay);
        animator.Play("Soul_Tyrant_Idle");
    }

    IEnumerator TeleportAndFireMisilePatternX3()
    {
        yield return StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
        yield return StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
        yield return StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay * 6));
    }

    public Transform GetRandomTeleportPoint()
    {
        Transform selectedTransform;
        do
        {
            int randomIndex = Random.Range(0, teleportPointList.Count);
            selectedTransform = teleportPointList[randomIndex];
        } while (!IsOutsideTargetRadius(selectedTransform));
        return selectedTransform;
    }

    public bool IsOutsideTargetRadius(Transform point)
    {
        Vector3 targetPosition = targetObject.transform.position;
        return Vector3.Distance(targetPosition, point.position) > radius;
    }

    public void PerformTeleport(Transform teleportPoint)
    {
        voiceSource.PlayOneShot(randomTeleportClip);
        animator.Play("Soul_Tyrant_Idle");
        transform.position = teleportPoint.position;
        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
    }

    public void PerformTeleport(Vector2 teleportPoint)
    {
        animator.Play("Soul_Tyrant_Idle");
        transform.position = teleportPoint;
        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
    }

    public void PlaceTeleportTrail(Vector2 startPosition, Vector2 endPosition)
    {
        // Calculate the direction and distance between the two points
        Vector2 direction = (endPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, endPosition);

        // Calculate the number of trail instances needed
        int numberOfTrails = Mathf.FloorToInt(distance / trailSpacing);

        // Place the trail sprites along the line between the two points
        for (int i = 0; i <= numberOfTrails; i++)
        {
            Vector2 trailPosition = startPosition + i * trailSpacing * direction;
            EffectDestroy effect = Instantiate(teleportTrailPrefab, trailPosition, Quaternion.identity);
            effect.SetDisappear(2f);
            effect.SetDestroy(2.1f);
        }
    }

    void SummonTeleportEffect()
    {
        EffectDestroy effect = Instantiate(teleportEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.4f);
        SummonLightBlow(0.2f, transform.position, new Vector2(2f, 2f));
    }

    IEnumerator FireMisile(float _delay)
    {
        effectSource.Stop();
        effectSource.PlayOneShot(fireClip);
        GameObject miosile = Instantiate(misilePrefab, transform.position, Quaternion.identity);
        miosile.transform.position = launchPoint.position;
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator RushPattern(float _preDelay, float _posDelay)
    {
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        Transform rushStart;
        do
        {
            rushStart = rushPointList[Random.Range(0, rushPointList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        Transform rushEnd = rushPointList.Find(t => t != rushStart);
        animator.Play("A_Rush_Ready");
        voiceSource.PlayOneShot(rushReadyClip);
        FocusEffect effect = Instantiate(focusEffectPrefab);
        effect.transform.position = transform.position;
        effect.SetFocusEffect(_preDelay - 0.7f, 5 / (_preDelay - 0.7f));
        yield return new WaitForSeconds(_preDelay);
        voiceSource.Stop();
        voiceSource.PlayOneShot(rushSpiritClip);
        yield return new WaitForSeconds(0.2f);
        animator.Play("A_Rush_Active");
        effectSource.PlayOneShot(rushMoveClip);
        yield return StartCoroutine(RushStart(rushEnd, rushSpeed));
        animator.Play("A_Rush_Finish");
        effectSource.PlayOneShot(rushBurstClip);
        CameraManager.Instance.StopShake();
        CameraManager.Instance.CameraShake();
        if (rushEnd == rushPointList[0])
        {
            SummonBurstEffect(3f, new Vector2(-21, 0.5f), 60f);
        }
        else
        {
            SummonBurstEffect(3f, new Vector2(7f, 0.5f), -60f);
        }
        
        yield return new WaitForSeconds(_posDelay);
    }

    IEnumerator RushStart(Transform _end, float _speed)
    {
        while (Vector3.Distance(transform.position, _end.position) > 0.01f) // 일종의 "허용 오차"를 설정
        {
            Vector3 direction = (_end.position - transform.position).normalized;
            float step = _speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _end.position, step);
            yield return null;
        }
    }


    IEnumerator SphereHulaufRushPattern(float _preDelay, float _posDelay)
    {
        Vector2 point1 = transform.position;
        SummonTeleportEffect();
        Transform rushStart;
        do
        {
            rushStart = hulaufRushList[Random.Range(0, hulaufRushList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        Transform rushEnd = hulaufRushList.Find(t => t != rushStart);
        RotateBasedOnTargets(rushStart, rushEnd);
        Hulauf hulauf = Instantiate<Hulauf>(hulaufPrefab, transform);
        if (rushStart == hulaufRushList[0])
            hulauf.rotationSpeed = -hulauf.rotationSpeed;
        hulauf.transform.position = transform.position;
        yield return new WaitForSeconds(_preDelay);

        yield return StartCoroutine(RushStart(rushEnd, hulaufRushSpeed));
        hulauf.transform.parent = null;
        DisAppearBoss();
        hulauf.rotationSpeed = -hulauf.rotationSpeed;
        yield return StartCoroutine(hulauf.ShootHulaufCoroutine(rushStart, hulaufShootSpeed, hulaufShootForce));
        yield return StartCoroutine(RandomTeleport(0f, _posDelay));
    }

    void DisAppearBoss()
    {
        SummonTeleportEffect();
        spriteRenderer.color = Color.clear;
        m_collider2D.enabled = false;
    }

    void AppearBoss()
    {
        SummonTeleportEffect();
        spriteRenderer.color = Color.white;
        m_collider2D.enabled = true;
    }

    IEnumerator DownstrokePattern(Transform _target, float _preDelay, float _posDelay)
    {
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;
        PerformTeleport(pos);
        downStroke_Wait.gameObject.SetActive(true);
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(_preDelay);

        EffectDestroy effect = Instantiate(downStroke_Active);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(0.4f);
        downStroke_Wait.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        CameraManager.Instance.StopShake();
        CameraManager.Instance.CameraShake();
        SummonBurstEffect(3f, new Vector2(transform.position.x, 1.7f), 0f);
        yield return new WaitForSeconds(0.3f);
        SummonWave();
        yield return new WaitForSeconds(_posDelay);
        DisAppearBoss();
    }

    IEnumerator FakeDownstrokePattern(Transform _target, float _preDelay, float _posDelay)
    {
        Vector2 point1 = transform.position;
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;
        PerformTeleport(pos);
        Vector2 point2 = transform.position;
        PlaceTeleportTrail(point1, point2);
        downStroke_Wait.gameObject.SetActive(true);
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(_preDelay);

        EffectDestroy effect = Instantiate(downStroke_Fake);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(0.4f);
        downStroke_Wait.gameObject.SetActive(false);
        downStroke_Wait.Play("DownStroke_Fake");
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DownstrokePattern(_target, _preDelay, _posDelay));
    }

    void SummonWave()
    {
        Wave temp1 = Instantiate(wavePrefab);
        Wave temp2 = Instantiate(wavePrefab);
        Vector2 pos = transform.position;
        pos.y = wavePositionY;
        temp1.transform.position = pos;
        temp2.transform.position = pos;
        temp2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        temp1.SetMoveWave();
        temp2.SetMoveWave();
    }

    IEnumerator ShootWave(float _preDelay, float _middleDelay ,float _posDelay)
    {
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        Transform rushStart;
        do
        {
            rushStart = rushPointList[Random.Range(0, rushPointList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        PerformTeleport(rushStart);
        animator.Play("Soul_Tyrant_Crouch");
        FocusEffect effect = Instantiate(focusEffectPrefab);
        effect.transform.position = launchPoint.position;
        effect.SetFocusEffect(_preDelay, 5 / _preDelay);
        float dir = 0f;
        if (rushStart == rushPointList[0])
            dir = 180f;
        yield return new WaitForSeconds(_preDelay);
        yield return currentPatern = StartCoroutine(SummonWave(_middleDelay, dir));
        yield return currentPatern = StartCoroutine(SummonWave(_middleDelay, dir));
        yield return currentPatern = StartCoroutine(SummonWave(_posDelay, dir));
        yield return currentPatern = StartCoroutine(RandomTeleport(preRandomTeleportDelay, posRandomTeleportDelay));
    }

    IEnumerator SummonWave(float _delay, float _dir)
    {
        animator.SetTrigger("Crouch_Kick");
        Wave temp1 = Instantiate(wavePrefab);
        Vector2 pos = transform.position;
        pos.y = wavePositionY;
        temp1.transform.position = pos;
        temp1.transform.rotation = Quaternion.Euler(0f, _dir, 0f);
        temp1.SetMoveWave();
        animator.StopPlayback();

        yield return new WaitForSeconds(_delay);
    }

    void RotateBasedOnTargets(Transform _start, Transform target)
    {
        Vector2 scale;
        if (_start.position.x <= target.position.x)
            scale = new Vector2(scaleX, scaleY);
        else
            scale = new Vector2(-scaleX, scaleY);
        transform.localScale = scale;
    }

    void RotateBasedOnTargets(Vector2 _start, Transform target)
    {
        Vector2 scale;
        if (_start.x <= target.position.x)
            scale = new Vector2(scaleX, scaleY);
        else
            scale = new Vector2(-scaleX, scaleY);
        transform.localScale = scale;
    }

    void SummonLightBlow(float _time, Vector2 _point, Vector2 _size)
    {
        EffectDestroy effect = Instantiate(lightBlowPrefab);
        effect.transform.position = _point;
        effect.transform.localScale = _size;
        effect.SetFade(_time);
        effect.SetDestroy(_time);
    }

    void SummonBurstEffect(float _time, Vector2 _point, float _rotate)
    {
        EffectDestroy effect = Instantiate(burstEffectPrefab);
        effect.transform.position = _point;

        // ParticleSystem을 가져온 뒤 중지합니다.
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ps.Stop();

        // Shape 모듈에 접근합니다.
        ParticleSystem.ShapeModule shape = ps.shape;

        // 모듈 설정을 변경합니다.
        Vector3 currentRotation = shape.rotation;
        shape.rotation = new Vector3(currentRotation.x, _rotate, currentRotation.z);

        // ParticleSystem을 다시 시작합니다.
        ps.Play();

        effect.SetDestroy(_time);
    }


    public override void Dead()
    {
        base.Dead();
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();
            animator.Play("Soul_Tyrant_Dead");
            CameraManager.Instance.StopShake();
            CameraManager.Instance.CameraShake(5f, 8f, 1f);

            StartCoroutine(DeadCoroutine(5f));
        }
        
    }

    IEnumerator DeadCoroutine(float _delay)
    {
        EffectDestroy effect = Instantiate(DeadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(15f);

        yield return new WaitForSeconds(_delay);
        Destroy(effect.transform.GetChild(0).gameObject);
        SummonLightBlow(1f, transform.position, new Vector2(2f, 2f));
        

        Destroy(gameObject);
    }
}

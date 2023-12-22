using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sorcerer : Enemy
{
    [Header("Sorcerer")]
    public SceneChangeTorch torch;

    public bool setDeveloperMode;
    public Animator animator;
    public AudioSource appearSource;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public AudioClip appearClip;
    public AudioClip deadEffectClip;
    public AudioClip ChargeClip;
    public AudioClip MoveEffectClip;
    public AudioClip MoveVoiceClip;
    public AudioClip SpellEffectClip;
    public AudioClip SpellGhostClip;
    public AudioClip SpellSkullsClip;
    public AudioClip DeadClip;
    public float appearDelay;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] EffectDestroy lightBlowPrefab;
    public bool isDead;

    float scaleX;
    float scaleY;

    Coroutine currentCoroutine;
    Coroutine FSM;

    protected override void Start()
    {
        base.Start();
        originalHitClip = hitClip;
        targetObject = PlayerCoroutine.Instance.gameObject;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        StartCoroutine(StartAppear());
        StartCoroutine(RandomTeleportCoroutine(preTeleportDelay, posTeleportDelay));
    }

    private void Update()
    {
        if (setDeveloperMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                animator.Play("A_Sorcerer_Stand");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkullMisile));
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
            }
        }

    }

    IEnumerator FSM1()
    {
        while (true)
        {
            int i = UnityEngine.Random.Range(0, 7);

            switch (i)
            {
                case 0:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 1:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkullMisile));
                    break;
                case 2:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
                    break;
                case 3:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 4:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 5:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 6:
                    yield return currentCoroutine = StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
                    break;

                default:
                    break;
            }
        }
    }

    IEnumerator FSM2()
    {
        yield return null;
    }

    IEnumerator StartAppear()
    {
        appearSource.PlayOneShot(appearClip);
        yield return new WaitForSeconds(appearDelay);
        appearSource.DOFade(0f, 1f);
        if (!setDeveloperMode)
            FSM = StartCoroutine(FSM1());
    }

    [Header("Spell Cast")]
    public float preSpellCastDelay;
    public float posSpellCastDelay;

    IEnumerator SpellCast(float _preDelay, float _posDelay, Action _spell)
    {
        animator.Play("A_Sorcerer_SpellCast");
        effectSource.PlayOneShot(ChargeClip);
        yield return new WaitForSeconds(_preDelay);
        effectSource.Stop();
        effectSource.PlayOneShot(SpellEffectClip);
        animator.Play("A_Sorcerer_SpellFire");
        _spell.Invoke();
        yield return new WaitForSeconds(_posDelay / 2);
        animator.Play("A_Sorcerer_Stand");
        yield return new WaitForSeconds(_posDelay / 2);
    }

    IEnumerator SpellCastSky(float _preDelay, float _posDelay, Action _spell)
    {
        animator.Play("A_Sorcerer_SpellCast");
        yield return new WaitForSeconds(_preDelay);
        animator.Play("A_Sorcerer_SpellFire");
        _spell.Invoke();
        yield return new WaitForSeconds(_posDelay / 2);
        animator.Play("A_Sorcerer_Stand");
        yield return new WaitForSeconds(_posDelay / 2);
    }

    [Header("Random Teleport")]
    public int hitCount;
    public float preTeleportDelay;
    public float posTeleportDelay;
    public EffectDestroy teleportEffect;
    public EffectDestroy teleportTrailPrefab;
    public List<Transform> teleportPointList;
    public List<Transform> teleportPoint_SkyList;
    public float radius = 5f;
    public float trailSpacing = 0.5f;  // The spacing between each trail sprite
    GameObject targetObject;
    int currentHitCount;

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        if (!invincibility)
        {
            base.Hit(_damage, _hitDir, _audioSource);
            currentHitCount++;
        }
        else
        {
            if (hitClip != null)
                _audioSource.PlayOneShot(hitClip);
        }
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        if (!invincibility)
        {
            base.UpAttackHit(_damage, _hitDir, _audioSource);
            currentHitCount++;
        }
        else
        {
            if (hitClip != null)
                _audioSource.PlayOneShot(hitClip);
        }
    }

    IEnumerator RandomTeleportCoroutine(float _preDelay, float _posDelay)
    {
        while (true)
        {
            if (currentHitCount >= hitCount)
            {
                currentHitCount = 0;
                yield return new WaitForSeconds(_preDelay);
                SummonTeleportEffect();
                Vector2 point1 = transform.position;
                PerformTeleport(GetRandomTeleportPoint());
                Vector2 point2 = transform.position;
                voiceSource.PlayOneShot(MoveEffectClip);
                SummonTeleportEffect();
                PlaceTeleportTrail(point1, point2);
                yield return new WaitForSeconds(_posDelay);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator RandomTeleportSky(float _preDelay, float _posDelay)
    {
        yield return new WaitForSeconds(_preDelay);
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        PerformTeleport(GetRandomTeleportPointSky());
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        yield return new WaitForSeconds(_posDelay);
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

    public void PerformTeleport(Transform teleportPoint)
    {
        transform.position = teleportPoint.position;
        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
    }

    public Transform GetRandomTeleportPoint()
    {
        Transform selectedTransform;
        do
        {
            int randomIndex = UnityEngine.Random.Range(0, teleportPointList.Count);
            selectedTransform = teleportPointList[randomIndex];
        } while (!IsOutsideTargetRadius(selectedTransform));
        return selectedTransform;
    }

    public Transform GetRandomTeleportPointSky()
    {
        Transform selectedTransform;
        do
        {
            int randomIndex = UnityEngine.Random.Range(0, teleportPoint_SkyList.Count);
            selectedTransform = teleportPoint_SkyList[randomIndex];
        } while (!IsOutsideTargetRadius(selectedTransform));
        return selectedTransform;
    }

    public bool IsOutsideTargetRadius(Transform point)
    {
        Vector3 targetPosition = targetObject.transform.position;
        return Vector3.Distance(targetPosition, point.position) > radius;
    }

    void RotateBasedOnTargets(Transform _start, Transform target)
    {
        Vector2 scale;
        if (_start.position.x <= target.position.x)
            scale = new Vector2(-scaleX, scaleY);
        else
            scale = new Vector2(scaleX, scaleY);
        transform.localScale = scale;
    }

    void SummonTeleportEffect()
    {
        EffectDestroy effect = Instantiate(teleportEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.4f);
        SummonLightBlow(0.2f, transform.position, new Vector2(2f, 2f));
    }

    void DisAppearBoss()
    {
        spriteRenderer.color = Color.clear;
        m_collider2D.enabled = false;
    }

    void AppearBoss()
    {
        SummonTeleportEffect();
        spriteRenderer.color = currentColor;
        m_collider2D.enabled = true;
    }

    [Header("Summon Ghost")]
    public Transform ghostPrefab;
    public EffectDestroy summonCirclePrefab;
    public float summonTime;
    public int summonCount;
    public int maxGhostCount;

    void SummonGhost()
    {
        StartCoroutine(SummonGhostCoroutine());
    }

    IEnumerator SummonGhostCoroutine()
    {
        Vector2 point = GetRandomTeleportPointSky().position;
        EffectDestroy effect = Instantiate(summonCirclePrefab);
        effect.transform.position = point;
        effect.SetRolling(summonTime);
        yield return new WaitForSeconds(summonTime/2);
        voiceSource.PlayOneShot(SpellGhostClip);
        yield return new WaitForSeconds(summonTime/2);
        SummonLightBlow(0.2f, point, new(0.5f, 0.5f));
        effect.SetDisappear(0.5f);
        effect.SetDestroy(0.5f);
        Transform ghost = Instantiate(ghostPrefab);
        ghost.GetComponent<Ghost>().sorcerer = this;
        ghosts.Add(ghost.GetComponent<Ghost>());
        ghost.position = point;
        summonCount++;

        if (summonCount >= maxGhostCount)
        {
            summonCount = 0;
            StopCoroutine(FSM);
            yield return currentCoroutine = StartCoroutine(HellFireCoroutine());
        }
    }

    [Header("Summon Skull Misile")]
    public Transform skullMisilePrefab;

    void SummonSkullMisile()
    {
        StartCoroutine(SummonSkulMisileCoroutine());
    }

    IEnumerator SummonSkulMisileCoroutine()
    {
        Vector2 point = GetRandomTeleportPointSky().position;
        EffectDestroy effect = Instantiate(summonCirclePrefab);
        effect.transform.position = point;
        effect.SetRolling(summonTime);
        yield return new WaitForSeconds(summonTime);
        SummonLightBlow(0.2f, point, new(0.5f, 0.5f));
        effect.SetDisappear(0.5f);
        effect.SetDestroy(0.5f);
        Transform skullMisile = Instantiate(skullMisilePrefab);
        skullMisile.position = point;

    }

    [Header("Summon Skeletons")]
    public Skelleton[] skeletons;
    public Transform[] skeletonSummonPoint;

    void SummonSkeletons()
    {
        StartCoroutine(SummonSkeletonsCoroutine());
    }

    IEnumerator SummonSkeletonsCoroutine()
    {
        Vector2 point1 = skeletonSummonPoint[0].position;
        Vector2 point2 = skeletonSummonPoint[1].position;
        EffectDestroy effect1 = Instantiate(summonCirclePrefab);
        EffectDestroy effect2 = Instantiate(summonCirclePrefab);
        effect1.transform.position = point1;
        effect1.SetRolling(summonTime);
        effect2.transform.position = point2;
        effect2.SetRolling(summonTime);
        yield return new WaitForSeconds(summonTime / 2);
        voiceSource.PlayOneShot(SpellSkullsClip);
        yield return new WaitForSeconds(summonTime / 2);
        effect1.SetDisappear(0.5f);
        effect1.SetDestroy(0.5f);
        effect2.SetDisappear(0.5f);
        effect2.SetDestroy(0.5f);
        Skelleton skull1 = Instantiate(skeletons[0]);
        skull1.target = point2;
        Skelleton skull2 = Instantiate(skeletons[1]);
        skull2.target = point1;
    }


    [Header("Hell Fire")]
    public EffectDestroy fireAura;
    public EffectDestroy flameRoutine;
    public EffectDestroy flameActive;
    public Transform focusEffect;
    public Transform fireAuraPos;
    public Transform hellFirePos;
    public Transform flameRoutinePos;
    public Transform focusEffectPos;
    public List<Ghost> ghosts;
    public List<Ghost_Hand> ghost_Hands;
    public float preHellFireDelay;
    public float middleHellFireDelay;
    public float posHellFireDelay;
    public bool invincibility;
    public AudioClip ChangeHitClip;

    AudioClip originalHitClip;

    IEnumerator HellFireCoroutine()
    {
        // 헬파이어 패턴
        // 맵상에 유령이 4마리 이상 소환될 경우 -> 무적인 상태로 맵 가운데로 이동, 녹색팔과 유령들을 자신에게로 끌어들이고, 화염 분출 시작
        invincibility = true;
        hitClip = ChangeHitClip;

        currentHitCount = 0;
        animator.Play("A_Sorcerer_Stand");
        // hellFirePos에 위험 표시 해줘야함.
        EffectDestroy aura = Instantiate(fireAura);
        aura.transform.position = fireAuraPos.position;
        yield return new WaitForSeconds(preHellFireDelay);
        
        aura.SetDisappear(1f);
        aura.SetDestroy(1.2f);
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        PerformTeleport(hellFirePos);
        Vector2 point2 = transform.position;
        voiceSource.PlayOneShot(MoveEffectClip);
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        animator.Play("A_Sorcerer_SpellCast");
        foreach (var item in ghosts.ToList())
            item.HellFireReady();
        // ghost_Hands 리스트의 복사본을 생성하여 순회
        foreach (var item in ghost_Hands.ToList())
            item.HellFireReady();
        EffectDestroy flame1 = Instantiate(flameRoutine);
        flame1.transform.position = flameRoutinePos.position;
        Transform effect = Instantiate(focusEffect);
        effect.transform.position = focusEffectPos.position;
        yield return new WaitForSeconds(middleHellFireDelay);
        flame1.GetComponent<Animator>().Play("A_FlameFinish");
        flame1.SetDestroy(0.7f);
        yield return new WaitForSeconds(0.5f);
        effectSource.Stop();
        effectSource.PlayOneShot(SpellEffectClip);
        animator.Play("A_Sorcerer_SpellFire");
        // 헬파이어 음성
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SummonHellFireActiveCoroutine());
        animator.Play("A_Sorcerer_Stand");
        yield return new WaitForSeconds(posHellFireDelay);
        hitClip = originalHitClip;
        invincibility = false;
        SummonTeleportEffect();
        point1 = transform.position;
        PerformTeleport(GetRandomTeleportPoint());
        point2 = transform.position;
        voiceSource.PlayOneShot(MoveEffectClip);
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        FSM = StartCoroutine(FSM1());
    }

    [SerializeField] private int fireSummonCount = 10; // 소환 횟수
    [SerializeField] private float delayTime = 1f; // 딜레이 타임
    [SerializeField] private float sideDistance = 1f; // 사이 거리
    [SerializeField] private float firePosY;
    [SerializeField] AudioClip hellfireClip;
    IEnumerator SummonHellFireActiveCoroutine()
    {
        for (int i = 1; i <= fireSummonCount; i++)
        {
            // 왼쪽과 오른쪽으로 소환 위치 계산
            Vector2 leftPosition = new(transform.position.x - sideDistance * i, firePosY);
            Vector2 rightPosition = new(transform.position.x + sideDistance * i, firePosY);

            // 왼쪽과 오른쪽에 flameActive 소환
            Instantiate(flameActive, leftPosition, Quaternion.identity).SetDestroy(0.8f);
            Instantiate(flameActive, rightPosition, Quaternion.identity).SetDestroy(0.8f);
            effectSource.PlayOneShot(hellfireClip);
            CameraManager.Instance.CameraShake();
            // 딜레이 타임만큼 기다림
            yield return new WaitForSeconds(delayTime);
        }

        yield return null;
    }

    public override void Dead()
    {
        base.Dead();
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();

            CameraManager.Instance.StopShake();
            CameraManager.Instance.CameraShake(5f, 8f, 1f);

            StartCoroutine(DeadCoroutine(5f));
        }

    }

    IEnumerator DeadCoroutine(float _delay)
    {
        voiceSource.PlayOneShot(DeadClip);
        EffectDestroy effect = Instantiate(DeadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(15f);
        voiceSource.Stop();
        effectSource.PlayOneShot(deadEffectClip);
        animator.Play("A_Sorcerer_SpellCast");
        yield return new WaitForSeconds(_delay);
        Destroy(effect.transform.GetChild(0).gameObject);
        SummonLightBlow(1f, transform.position, new Vector2(3f, 3f));
        DisAppearBoss();
        yield return new WaitForSeconds(summonTime+0.1f);
        Sound_Manager.Instance.PlayBGM(0);
        torch.TorchMove(4f);
        yield return null;
        Destroy(gameObject);
    }

    void SummonLightBlow(float _time, Vector2 _point, Vector2 _size)
    {
        EffectDestroy effect = Instantiate(lightBlowPrefab);
        effect.transform.position = _point;
        effect.transform.localScale = _size;
        effect.SetFade(_time);
        effect.SetDestroy(_time);
    }
}

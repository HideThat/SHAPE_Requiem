using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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

    Transform lastGhost;
    Transform lastMisile;
    float scaleX;
    float scaleY;

    protected override void Start()
    {
        base.Start();
        targetObject = PlayerCoroutine.Instance.gameObject;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        StartCoroutine(StartAppear());
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
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 1:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkullMisile));
                    break;
                case 2:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
                    break;
                case 3:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 4:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 5:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonGhost));
                    break;
                case 6:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
                    break;

                default:
                    yield return StartCoroutine(SpellCast(preSpellCastDelay, posSpellCastDelay, SummonSkeletons));
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
            StartCoroutine(FSM1());
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
        yield return StartCoroutine(RandomTeleport(preTeleportDelay, posTeleportDelay));
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

        yield return StartCoroutine(RandomTeleportSky(preTeleportDelay, posTeleportDelay));
    }

    [Header("Random Teleport")]
    public float preTeleportDelay;
    public float posTeleportDelay;
    public EffectDestroy teleportEffect;
    public EffectDestroy teleportTrailPrefab;
    public List<Transform> teleportPointList;
    public List<Transform> teleportPoint_SkyList;
    public float radius = 5f;
    public float trailSpacing = 0.5f;  // The spacing between each trail sprite
    GameObject targetObject;
    IEnumerator RandomTeleport(float _preDelay, float _posDelay)
    {
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
        int rand = UnityEngine.Random.Range(0, 3);
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
        ghost.position = point;

        lastGhost = ghost;
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

        lastMisile = skullMisile;
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
        SummonGhost();
        SummonGhost();
        SummonGhost();
        SummonSkeletons();
        SummonSkullMisile();
        SummonSkullMisile();
        SummonSkullMisile();
        yield return new WaitForSeconds(summonTime+0.1f);

        while (lastGhost != null || lastMisile != null)
        {
            yield return null;
        }

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

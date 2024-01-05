using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TutorialBoss : MonoBehaviour
{
    public Animator animator;
    public Transform rock;
    public Transform wallCollider;
    public Transform bossAppearEffect;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public AudioSource appearSource;

    void Start()
    {
        foreach (var item in misileTransform1)
            item.parent = null;
        foreach (var item in misileTransform2)
            item.parent = null;
        foreach (var item in misileTransform3)
            item.parent = null;

        StartCoroutine(AppearBoss());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 movePos;
    public float moveTime;

    IEnumerator FSM()
    {
        
        yield return Pattern1();
        yield return Pattern2();
        yield return Pattern3();
    }

    IEnumerator AppearBoss()
    {
        bool finishMove = false;

        transform.DOMove(movePos, moveTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            finishMove = true;
        });

        while (!finishMove)
        {
            CameraManager.Instance.CameraShake();
            yield return new WaitForSeconds(0.5f);
        }
        animator.Play("A_Climb");
        rock.parent = null;
        wallCollider.parent = null;
        bossAppearEffect.parent = null;
        burstPoint.parent = null;
        yield return new WaitForSeconds(0.7f);
        bossAppearEffect.gameObject.SetActive(true);
        yield return CameraShakeLoop(2f);
        StartCoroutine(FSM());
    }

    public FocusEffect focusEffectPrefab;
    public GameObject misilePrefab;
    public AudioClip fireClip;
    public AudioClip fireReadyClip;
    public Transform[] misileTransform1;
    public Transform[] misileTransform2;
    public Transform[] misileTransform3;
    public bool checkHit = false;
    public GameObject keyX;

    IEnumerator Pattern1()
    {
        // ���� �̻��� ���ߴ�
        // ������ ��ġ�� �̻��� �ϳ� ��ȯ
        // �ΰ� ��ȯ
        // ���� ��ȯ

        animator.Play("A_LInteractionPull");
        yield return new WaitForSeconds(0.5f);
        yield return FireMisile(1f, 2f, misileTransform1);
        yield return FireMisile(1f, 2f, misileTransform2);
        yield return FireMisile(1f, 2f, misileTransform3);

        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            animator.Play("A_LInteractionPull_Reverse");
            yield return new WaitForSeconds(0.5f);
            animator.Play("A_LInteractionPull");
            yield return new WaitForSeconds(0.5f);
            yield return FireMisile(1f, 2f, misileTransform1);
            yield return FireMisile(1f, 2f, misileTransform2);
            yield return FireMisile(1f, 2f, misileTransform3);
        }

        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            animator.Play("A_LInteractionPull_Reverse");
            yield return new WaitForSeconds(0.5f);
            animator.Play("A_LInteractionPull");
            yield return new WaitForSeconds(0.5f);
            yield return FireMisile(1f, 2f, misileTransform1);
            yield return FireMisile(1f, 2f, misileTransform2);
            yield return FireMisile(1f, 2f, misileTransform3);
        }
        animator.Play("A_LInteractionPull_Reverse");
        keyX.SetActive(false);
        yield return new WaitForSeconds(2f);
    }

    public GameObject misilePrefab2;
    public GameObject keyUP;
    public Transform[] waveTransform1;
    public Transform[] waveTransform2;
    public Transform[] waveTransform3;

    IEnumerator Pattern2()
    {
        // �̻��� ������ ��ȯ
        // �� ����Ű �˷���
        animator.Play("A_LInteractionPull");
        yield return new WaitForSeconds(0.5f);
        yield return FireMisile2(1f, 3f, waveTransform1);
        yield return FireMisile2(1f, 3f, waveTransform2);
        yield return FireMisile2(1f, 3f, waveTransform3);


        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            keyUP.SetActive(true);
            animator.Play("A_LInteractionPull_Reverse");
            yield return new WaitForSeconds(0.5f);
            animator.Play("A_LInteractionPull");
            yield return new WaitForSeconds(0.5f);
            yield return FireMisile2(1f, 3f, waveTransform1);
            yield return FireMisile2(1f, 3f, waveTransform2);
            yield return FireMisile2(1f, 3f, waveTransform3);
        }

        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            keyUP.SetActive(true);
            animator.Play("A_LInteractionPull_Reverse");
            yield return new WaitForSeconds(0.5f);
            animator.Play("A_LInteractionPull");
            yield return new WaitForSeconds(0.5f);
            yield return FireMisile2(1f, 3f, waveTransform1);
            yield return FireMisile2(1f, 3f, waveTransform2);
            yield return FireMisile2(1f, 3f, waveTransform3);
        }

        animator.Play("A_LInteractionPull_Reverse");
        keyX.SetActive(false);
        keyUP.SetActive(false);
        yield return new WaitForSeconds(2f);
    }

    public TutoRockMislie rockMislie;
    public Transform[] rockPoints;
    public EffectDestroy burstImpact;
    public Transform burstPoint;
    public GameObject keyDown;

    IEnumerator Pattern3()
    {
        // ���� �ϳ� �߻�
        // ���� �ΰ� �߻�
        // �� ��ü ����
        yield return FireRock();
        yield return new WaitForSeconds(3f);
        yield return FireRock();
        yield return new WaitForSeconds(1.5f);
        yield return FireRock();
        yield return new WaitForSeconds(1.5f);
        yield return FireRock();
        yield return new WaitForSeconds(1.5f);
        yield return FireRockLast();

        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            keyDown.SetActive(true);
            yield return FireRock();
            yield return new WaitForSeconds(3f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRockLast();
        }

        if (checkHit)
        {
            checkHit = false;
            keyX.SetActive(true);
            keyDown.SetActive(true);
            yield return FireRock();
            yield return new WaitForSeconds(3f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRock();
            yield return new WaitForSeconds(1.5f);
            yield return FireRockLast();
        }

        keyX.SetActive(false);
        keyDown.SetActive(false);
    }

    IEnumerator FireRock()
    {
        animator.Play("A_GroundSlam");
        yield return new WaitForSeconds(0.1f);
        CameraManager.Instance.CameraShake();
        SummonBurstEffect(3f, burstPoint.position, 0f);
        TutoRockMislie rock = Instantiate(rockMislie, rockPoints[0].position, Quaternion.identity);
        rock.MoveUp();
        rock.MoveFront();
        rock.tutorialBoss = this;
        yield return null;
    }

    IEnumerator FireRockLast()
    {
        yield return new WaitForSeconds(1f);
        animator.Play("A_GroundSlam_UP");
        Vector2 origin = transform.position;
        transform.DOMoveY(3f, 0.7f);
        yield return new WaitForSeconds(0.7f);
        animator.Play("A_GroundSlam_Ready");
        FocusEffect effect = Instantiate(focusEffectPrefab);
        Vector2 vector = new(transform.position.x, transform.position.y + 2f);
        effect.transform.position = vector;
        effect.SetFocusEffect(2f, 5 / 2f);
        yield return new WaitForSeconds(2.5f);
        animator.Play("A_GroundSlam_Down");
        transform.DOMoveY(origin.y, 0.25f).SetEase(Ease.Linear).OnComplete(()=>
        {
            animator.Play("A_GroundSlam_Active");
        });
        yield return new WaitForSeconds(0.35f);
        CameraManager.Instance.CameraShake();
        SummonBurstEffect(3f, burstPoint.position, 0f);
        yield return new WaitForSeconds(0.2f);
        List<TutoRockMislie> rockList = new List<TutoRockMislie>();
        for (int i = 0; i < rockPoints.Length; i++)
        {
            TutoRockMislie rock = Instantiate(rockMislie, rockPoints[i].position, Quaternion.identity);
            rockList.Add(rock);
            rock.tutorialBoss = this;
            rock.MoveUp();
        }
        yield return new WaitForSeconds(2f);
        foreach (var item in rockList)
        {
            item.MoveDown();
        }
    }

    IEnumerator FireMisile(float _preDelay, float _delay, Transform[] transforms)
    {
        effectSource.PlayOneShot(fireReadyClip);
        // ������� ����Ʈ ����
        for (int i = 0; i < transforms.Length; i++)
        {
            FocusEffect effect = Instantiate(focusEffectPrefab);
            effect.transform.position = transforms[i].position;
            effect.SetFocusEffect(_preDelay, 5 / _preDelay);
        }

        yield return new WaitForSeconds(_preDelay);
        effectSource.Stop();
        effectSource.PlayOneShot(fireClip);
        for (int i = 0; i < transforms.Length; i++)
        {
            GameObject miosile = Instantiate(misilePrefab, transform.position, Quaternion.identity);
            miosile.GetComponent<TutoMisile>().tutorialBoss = this;
            miosile.transform.position = transforms[i].position;
        }
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator FireMisile2(float _preDelay, float _delay, Transform[] transforms)
    {
        effectSource.PlayOneShot(fireReadyClip);
        // ������� ����Ʈ ����
        for (int i = 0; i < transforms.Length; i++)
        {
            FocusEffect effect = Instantiate(focusEffectPrefab);
            effect.transform.position = transforms[i].position;
            effect.SetFocusEffect(_preDelay, 5 / _preDelay);
        }

        yield return new WaitForSeconds(_preDelay);
        effectSource.Stop();
        effectSource.PlayOneShot(fireClip);
        for (int i = 0; i < transforms.Length; i++)
        {
            GameObject miosile = Instantiate(misilePrefab2, transform.position, Quaternion.identity);
            miosile.GetComponent<TutoMisile2>().tutorialBoss = this;
            miosile.transform.position = transforms[i].position;
        }
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator CameraShakeLoop(float _time)
    {
        float elapsedTime = 0f; // ��� �ð� ����

        while (elapsedTime < _time)
        {
            CameraManager.Instance.CameraShake(); // ī�޶� ����
            yield return new WaitForSeconds(0.5f); // 0.5�� ���

            elapsedTime += 0.5f; // ��� �ð� ������Ʈ
        }
    }

    public Vector2 errorPos;

    public void ClimbFinishChangePos()
    {
        transform.position = new(transform.position.x + errorPos.x, transform.position.y + errorPos.y);
    }

    void SummonBurstEffect(float _time, Vector2 _point, float _rotate)
    {
        EffectDestroy effect = Instantiate(burstImpact);
        effect.transform.position = _point;

        // ParticleSystem�� ������ �� �����մϴ�.
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ps.Stop();

        // Shape ��⿡ �����մϴ�.
        ParticleSystem.ShapeModule shape = ps.shape;

        // ��� ������ �����մϴ�.
        Vector3 currentRotation = shape.rotation;
        shape.rotation = new Vector3(currentRotation.x, _rotate, currentRotation.z);

        // ParticleSystem�� �ٽ� �����մϴ�.
        ps.Play();

        effect.SetDestroy(_time);
    }
}

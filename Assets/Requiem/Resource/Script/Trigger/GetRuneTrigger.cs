using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class GetRuneTrigger : MonoBehaviour
{
    [SerializeField] private RuneStatue runeStatue;
    [SerializeField] private RuneManager runeManager;
    [SerializeField] private Light2D m_light;
    [SerializeField] private float animationTime;

    public float rotationSpeed = 10f;
    public float convergenceSpeed = 1f;

    private bool m_isActive = false;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        runeManager = RuneData.RuneObj.GetComponent<RuneManager>();

        if (runeStatue == null) Debug.Log("runeStatue == null");
        if (runeManager == null) Debug.Log("runeManager == null");
        if (m_light == null) Debug.Log("m_light == null");
    }

    private void Update()
    {
        if (m_isActive)
        {
            RotateRuneManager();
            MoveRuneManager();
            FadeOutLight();
        }
    }

    private void RotateRuneManager()
    {
        runeManager.transform.Rotate(Vector3.forward * rotationSpeed);
    }

    private void MoveRuneManager()
    {
        runeManager.transform.position =
            Vector2.MoveTowards(runeManager.transform.position, runeStatue.transform.position, Time.deltaTime * convergenceSpeed);
    }

    private void FadeOutLight()
    {
        DOTween.To(() => m_light.intensity, x => m_light.intensity = x, 0f, animationTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CanActivateRune(collision) && runeStatue != null)
        {
            ActivateRune();
            StartCoroutine(GetLuneDelay());
        }

        if (CanActivateRune(collision) && runeStatue == null)
        {
            DeactivateRune();
        }
    }

    private bool CanActivateRune(Collider2D collision)
    {
        return !m_isActive && collision.gameObject.CompareTag("Player") && !PlayerData.PlayerIsGetRune;
    }

    private void ActivateRune()
    {
        m_isActive = true;
        runeStatue.EnterTheRune();
        runeStatue.isActive = true;
    }

    private IEnumerator GetLuneDelay()
    {
        yield return new WaitForSeconds(animationTime);

        DeactivateRune();
    }

    private void DeactivateRune()
    {
        m_isActive = false;
        runeManager.transform.rotation = Quaternion.identity;
        PlayerData.PlayerIsGetRune = true;
    }
}

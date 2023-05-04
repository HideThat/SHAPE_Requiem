// 1�� �����丵

using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite; // Ȱ��ȭ �� �̹���
    [SerializeField] private Sprite inactiveSprite; // �� Ȱ��ȭ �� �̹���
    [SerializeField] private AudioClip audioClip; // ȿ����

    Switch platformSwitch; // �÷��� ����ġ
    Collider2D platformCollider; // �÷��� �ݶ��̴�
    SpriteRenderer platformRenderer; // �÷��� ��������Ʈ ������
    GameObject light1; // �÷��� ����Ʈ1
    GameObject light2; // �÷��� ����Ʈ2
    AudioSource audioSource; // �÷��� ����� �ҽ�

    private bool isActivated = false; // �÷��� Ȱ��ȭ ����

    public bool IsActivated
    {
        get { return isActivated; }
        set
        {
            if (value != isActivated)
            {
                isActivated = value;
                OnActivationChanged();
            }
        }
    }

    private void Start()
    {
        platformSwitch = transform.Find("Swich").GetComponent<Switch>();
        platformCollider = transform.Find("Platform").GetComponent<Collider2D>();
        platformRenderer = transform.Find("Platform").GetComponent<SpriteRenderer>();
        audioSource = transform.Find("Platform").GetComponent<AudioSource>();
        light1 = platformCollider.transform.Find("Light1").gameObject;
        light2 = platformCollider.transform.Find("Light2").gameObject;

        if (platformSwitch == null) Debug.Log("platformSwitch == null");
        if (platformCollider == null) Debug.Log("platformCollider == null");
        if (platformRenderer == null) Debug.Log("platformRenderer == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (light1 == null) Debug.Log("light1 == null");
        if (light2 == null) Debug.Log("light2 == null");
        if (activeSprite == null) Debug.Log("activeSprite == null");
        if (inactiveSprite == null) Debug.Log("inactiveSprite == null");
        if (audioClip == null) Debug.Log("audioClip == null");

        if (light2 != null)
            light2.SetActive(false); // �ʱ⿡�� ����Ʈ2 ��Ȱ��ȭ
    }

    private void Update()
    {
        MovePlatform();
    }

    private void OnActivationChanged() // �÷��� Ȱ��ȭ ���°� ����� ������ ȿ���� ���
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void MovePlatform()
    {
        if (platformSwitch.isActive)
        {
            platformRenderer.sprite = inactiveSprite; // �÷��� ��Ȱ��ȭ ���� ��������Ʈ�� ����
            platformCollider.enabled = false; // �÷��� ��Ȱ��ȭ

            if (light1 != null)
                light1.SetActive(false);

            if (light2 != null)
                light2.SetActive(true);
        }
        else
        {
            platformRenderer.sprite = activeSprite; // �÷��� Ȱ��ȭ ���� ��������Ʈ�� ����
            platformCollider.enabled = true; 

            if (light1 != null)
                light1.SetActive(true);

            if (light2 != null)
                light2.SetActive(false);
        }
    }
}

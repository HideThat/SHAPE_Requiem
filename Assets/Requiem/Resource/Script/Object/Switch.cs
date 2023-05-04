// 1�� �����丵

using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite; // Ȱ��ȭ ����ġ
    [SerializeField] private Sprite inactiveSprite; // �� Ȱ��ȭ ����ġ
    [SerializeField] private AudioClip switchOnAudio; // Ȱ��ȭ ����
    [SerializeField] private AudioClip switchOffAudio; // �� Ȱ��ȭ ����

    private SpriteRenderer spriteRenderer; // �ڽ��� ��������Ʈ ������
    private AudioSource audioSource; // �ڽ��� ����� �ҽ�
    public bool isActive; // Ȱ��ȭ ����

    private void Start()
    {
        InitializeComponents();
        SetInitialState();
    }

    // �ʿ��� ������Ʈ ��������
    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null) Debug.Log("spriteRenderer == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (inactiveSprite == null) Debug.Log("inactiveSprite == null");
        if (activeSprite == null) Debug.Log("activeSprite == null");
        if (switchOnAudio == null) Debug.Log("switchOnAudio == null");
        if (switchOffAudio == null) Debug.Log("switchOffAudio == null");
    }

    // ����ġ�� �ʱ� ���� ����
    private void SetInitialState()
    {
        spriteRenderer.sprite = inactiveSprite;
        isActive = false;
    }

    // ����ġ ��ȣ�ۿ� ó���� ���� OnTrigger �Լ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsRuneCollision(collision))
        {
            ToggleSwitchState();
            PlaySwitchAudio();
            UpdateSwitchSprite();
        }
    }

    // ��� �浹�� ��� Ȯ��
    private bool IsRuneCollision(Collider2D collision)
    {
        return collision.gameObject.layer == (int)LayerName.Rune && RuneData.RuneActive;
    }

    // ����ġ ���� ��ȯ
    private void ToggleSwitchState()
    {
        isActive = !isActive;
    }

    // ����ġ ����� ���
    private void PlaySwitchAudio()
    {
        audioSource.PlayOneShot(isActive ? switchOnAudio : switchOffAudio);
    }

    // ����ġ ��������Ʈ ������Ʈ
    private void UpdateSwitchSprite()
    {
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
    }

    // �ʱ�ȭ �Լ�
    public void Initialize()
    {
        isActive = false;
    }
}

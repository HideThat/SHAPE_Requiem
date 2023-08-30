using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class TextData
{
    public string text;
    public float textBox_x;
    public float textBox_y;
    public float fontSize;
    public TMP_FontAsset fontType;
}

[Serializable]
public class TextDataGroup
{
    public List<TextData> textDatas;
}

public class NPC : MonoBehaviour
{
    public string NPC_name;
    public Animator animator; // NPC �ִϸ��̼�

    [Header("��ȭ �ý���")]
    public bool talkable = false; // ��ȭ ���� ����
    public Image textBoxImage; // �ؽ�Ʈ �ڽ� �̹���
    public TextMeshProUGUI textBox; // �ؽ�Ʈ �ڽ� ����
    public List<TextDataGroup> textGroups;

    [Header("�ΰ��� ������")]
    public bool isTalking = false; // ��ȭ ������ ����
    public bool isPlayerInRange = false; // �÷��̾ ���� ���� �ִ��� ����
    public bool notComtpleteTalk = false;
    public int textIndex = 0;
    public int currentTextIndex = 0; // ���� ��ȭ �ε���
    public int finishTextIndex = -1;
    public Coroutine coroutine;

    protected void Start()
    {
        textBoxImage.gameObject.SetActive(false); // ���� �� �ؽ�Ʈ �ڽ� �����
    }


    protected void Update()
    {
        // �÷��̾ ���� ���� �ְ� 'F' Ű�� ������ ��ȭ ����
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && talkable) 
        {
            ConversationSystem(textGroups[textIndex].textDatas);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ ���� ���� ������ isPlayerInRange�� true�� ����
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            StopCoroutine(coroutine);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        // �÷��̾ ���� ������ ������ isPlayerInRange�� false�� ����
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isTalking)
            {
                coroutine = StartCoroutine(ResetConversationAfterDelay(2f)); // 3�� �� ��ȭ �ʱ�ȭ
            }
        }
    }

    IEnumerator ResetConversationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isPlayerInRange) // �÷��̾ ���� ���� ���� ���� ��츸 �ʱ�ȭ
        {
            NotCompleteTalking();
        }
    }


    void ConversationSystem(List<TextData> text)
    {
        if (isTalking) // ��ȭ ���� ���
        {
            ContinueTalking(text);
        }
        else // ��ȭ�� ���� ���۵��� �ʾ��� ���
        {
            StartTalking(text);
        }
    }

    Coroutine displayTextCoroutine; // ���� ���� ���� DisplayText �ڷ�ƾ�� �����ϱ� ���� ����

    void StartTalking(List<TextData> texts)
    {
        isTalking = true;
        textBoxImage.gameObject.SetActive(true);

        if (displayTextCoroutine != null) // �̹� ���� ���� �ڷ�ƾ�� �ִٸ� ����
        {
            StopCoroutine(displayTextCoroutine);
        }
        displayTextCoroutine = StartCoroutine(DisplayText(texts));
    }

    void ContinueTalking(List<TextData> texts)
    {
        currentTextIndex++;
        if (currentTextIndex < texts.Count)
        {
            if (displayTextCoroutine != null) // �̹� ���� ���� �ڷ�ƾ�� �ִٸ� ����
            {
                StopCoroutine(displayTextCoroutine);
            }
            displayTextCoroutine = StartCoroutine(DisplayText(texts));
        }
        else
        {
            EndTalking();
        }
    }



    IEnumerator DisplayText(List<TextData> texts)
    {
        // ���� �ε����� �ؽ�Ʈ �����͸� ǥ��
        var data = texts[currentTextIndex];
        textBox.rectTransform.sizeDelta = new Vector2(data.textBox_x, data.textBox_y);
        textBox.fontSize = data.fontSize;
        textBox.font = data.fontType;

        // �ϳ��� ���ڸ� ���
        textBox.text = ""; // �ʱ�ȭ
        for (int i = 0; i < data.text.Length; i++)
        {
            textBox.text += data.text[i];
            yield return new WaitForSeconds(0.03f); // 0.1�� ��� (�� ���� �����ؼ� ���ڰ� ������ �ӵ��� ���� ����)
        }

        yield return null; // �� �κ��� �ʿ信 ���� �߰����� ������ ���� �� �ֽ��ϴ�.
    }



    void EndTalking()
    {
        // ��ȭ ����
        isTalking = false;
        currentTextIndex = 0;
        textBoxImage.gameObject.SetActive(false);
        finishTextIndex = textIndex;
        notComtpleteTalk = false;
        StopCoroutine(coroutine);
    }

    void NotCompleteTalking()
    {
        // �߰��� ��ȭ ����
        isTalking = false;
        currentTextIndex = 0;
        textBoxImage.gameObject.SetActive(false);
        notComtpleteTalk = true;
    }

    public void AnimationPlay(string _animationName)
    {
        // �ִϸ��̼� ���
        animator.Play(_animationName);
    }
}

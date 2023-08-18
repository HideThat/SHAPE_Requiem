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

public class NPC : MonoBehaviour
{
    public string NPC_name;
    public Image textBoxImage;
    public TextMeshProUGUI textBox;
    public Animator animator;
    public List<TextData> textData;
    public bool talkable = false;

    private bool isPlayerInRange = false; // �÷��̾ ���� ���� �ִ��� �Ǵ�

    void Start()
    {
        // ��ȭ ������ �ؽ�Ʈ �ڽ� �ٽ� �����
        textBoxImage.gameObject.SetActive(false);
    }

    
    void Update()
    {
        // ���⼭ state�� ���� ������ �����ϸ� �˴ϴ�.
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && talkable)
        {
            StartCoroutine(InteractWithPlayer(0));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // "Player"�� �÷��̾��� �±׸��̾�� ��
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    IEnumerator InteractWithPlayer(int index)
    {
        talkable = false;
        // ��ȭ ���� �� �ؽ�Ʈ �ڽ� Ȱ��ȭ
        textBoxImage.gameObject.SetActive(true);

        // ��ȭ ���� ���
        for (int i = index; i < textData.Count; i++)
        {
            // ��ȭ �� �÷��̾ ������ ����� ���
            if (!isPlayerInRange)
            {
                // �� �� ���� isPlayerInRange�� false�� ��ȭ �Ͻ� �ߴ�
                yield return new WaitForSeconds(2f); // ��: 2�� ���� ���

                // �÷��̾ �ٽ� ���� ���� ���� ������ ���
                yield return new WaitUntil(() => isPlayerInRange);
            }

            // �ؽ�Ʈ �ڽ� ũ�� ����
            textBox.rectTransform.sizeDelta = new Vector2(textData[i].textBox_x, textData[i].textBox_y);

            // �ؽ�Ʈ �۲� �� ũ�� ����
            textBox.fontSize = textData[i].fontSize;
            textBox.font = textData[i].fontType;

            // �ؽ�Ʈ ���� ����
            textBox.text = textData[i].text;

            // ��ȣ�ۿ� Ű�� ���� ������ ���
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        }

        // ��ȭ ������ �ؽ�Ʈ �ڽ� �ٽ� �����
        textBoxImage.gameObject.SetActive(false);
        talkable = true; // �ٽ� ��ȭ ���� ���·� ����
    }




    public void AnimationPlay(string _animationName)
    {
        animator.Play(_animationName); // �ִϸ������� "StateIndex" �Ķ���� ���� ����
    }
}

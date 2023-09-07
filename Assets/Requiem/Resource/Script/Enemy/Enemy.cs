using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public int HP;
    public int damage; // ���� ������ ���ط��� �����ϴ� ����
    public Collider2D m_collider2D; // ���� �浹ü�� �����ϴ� ����

    public bool runeIn;

    public virtual void ResetEnemy()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rune")) runeIn = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rune")) runeIn = false;
    }
}

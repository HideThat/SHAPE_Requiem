using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected string m_name; // ���� �̸��� �����ϴ� ����
    public int damage; // ���� ������ ���ط��� �����ϴ� ����
    protected Collider2D m_collider2D; // ���� �浹ü�� �����ϴ� ����

    public virtual void ResetEnemy()
    {

    }
}

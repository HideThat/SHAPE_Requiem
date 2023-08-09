using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected string m_name; // 적의 이름을 저장하는 변수
    public int damage; // 적이 입히는 피해량을 저장하는 변수
    protected Collider2D m_collider2D; // 적의 충돌체를 저장하는 변수

    public virtual void ResetEnemy()
    {

    }
}

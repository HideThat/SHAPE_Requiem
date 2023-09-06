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
    public int damage; // 적이 입히는 피해량을 저장하는 변수
    public Collider2D m_collider2D; // 적의 충돌체를 저장하는 변수
    public GameObject damageBox;

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

    public virtual void Hit(int _damage)
    {
        Debug.Log("데미지 들어감");
        HP -= _damage;
        GameObject dmgBox = Instantiate(damageBox);
        dmgBox.transform.position = transform.position;
        
        TextMeshProUGUI damageText = dmgBox.transform.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = _damage.ToString();
        damageText.DOColor(Color.clear, 2f);

        Rigidbody2D rigid = dmgBox.GetComponent<Rigidbody2D>();

        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), 1f);

        rigid.AddForce(dir.normalized * 2f, ForceMode2D.Impulse);

        StartCoroutine(ObjectDestroyDelay(2f, dmgBox));
    }

    IEnumerator ObjectDestroyDelay(float _delay, GameObject _object)
    {


        yield return new WaitForSeconds(_delay);

        Destroy(_object);
    }
}

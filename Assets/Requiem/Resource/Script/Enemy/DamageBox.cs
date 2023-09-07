using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageBox : MonoBehaviour
{
    public TextMeshProUGUI TMPtext;
    public Rigidbody2D rigid;
    public float disappearDelay;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), 1f);

        rigid.AddForce(dir.normalized * 2f, ForceMode2D.Impulse);

        TMPtext.DOColor(Color.clear, disappearDelay);

        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(disappearDelay);

        Destroy(gameObject);
    }
}

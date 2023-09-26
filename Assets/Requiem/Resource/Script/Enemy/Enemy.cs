using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP;
    public int damage; // 적이 입히는 피해량을 저장하는 변수
    public Collider2D m_collider2D; // 적의 충돌체를 저장하는 변수
    public SpriteRenderer spriteRenderer;
    public Blood bloodPrefab; // Blood 프리팹
    public float minBloodSize;
    public float maxBloodSize;
    public float minBloodForce;
    public float maxBloodForce;
    public int bloodCount;
    public Color hitColor = Color.red;

    public bool runeIn;

    public virtual void ResetEnemy()
    {
        // 리셋 로직
    }

    public virtual void Hit(int _damage, Vector2 _hitDir)
    {
        HP -= _damage;

        Color currentColor = spriteRenderer.color;

        if (HP > 0)
        {
            spriteRenderer.DOColor(hitColor, 0.2f).OnComplete(() =>
            {
                spriteRenderer.DOColor(currentColor, 0.2f);
            });

            for (int i = 0; i < bloodCount; i++)
            {
                // 방사형으로 퍼질 각도를 랜덤하게 선택
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

                // 해당 각도로 방향 벡터 계산
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                float force = Random.Range(minBloodForce, maxBloodForce);
                Vector2 bloodSize = new Vector2(Random.Range(minBloodSize, maxBloodSize), Random.Range(minBloodSize, maxBloodSize));

                Blood spawnedBlood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                spawnedBlood.SetBlood(bloodSize, direction, force);
            }
        }
        else
        {
            Dead();
        }
        
    }

    public virtual void Dead()
    {
        
    }
}



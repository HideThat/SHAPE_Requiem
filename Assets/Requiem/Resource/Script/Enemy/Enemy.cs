using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP;
    public int damage; // ���� ������ ���ط��� �����ϴ� ����
    public Collider2D m_collider2D; // ���� �浹ü�� �����ϴ� ����
    public SpriteRenderer spriteRenderer;
    public Blood bloodPrefab; // Blood ������
    public float minBloodSize;
    public float maxBloodSize;
    public float minBloodForce;
    public float maxBloodForce;
    public int bloodCount;
    public Color hitColor = Color.red;

    public bool runeIn;

    public virtual void ResetEnemy()
    {
        // ���� ����
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
                // ��������� ���� ������ �����ϰ� ����
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

                // �ش� ������ ���� ���� ���
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



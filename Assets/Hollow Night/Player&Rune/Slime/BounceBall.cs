using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBall : Enemy
{
    [Header("Slime Ball")]
    public Enemy slime;
    public float rotateSpeed;


    
    void Update()
    {
        Vector3 neRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.y + rotateSpeed * Time.deltaTime);
        transform.Rotate(neRotation); 
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);
        Debug.Log("������ �� ��Ʈ");

        // maxHP�� HP�� ������ ���
        float healthRatio = (float)HP / maxHP; // ���� ���, maxHP�� 100�̰� HP�� 90�̸� 0.9�� �˴ϴ�.
        Debug.Log(healthRatio);
        // currentColor�� ����
        // 1 - healthRatio�� �ϸ�, HP�� �پ�� �� ���� �����մϴ�. ���� ���, healthRatio�� 0.9�� 1 - 0.9 = 0.1�� �˴ϴ�.
        currentColor = new Color(1f, healthRatio, healthRatio, 1f);
    }
}

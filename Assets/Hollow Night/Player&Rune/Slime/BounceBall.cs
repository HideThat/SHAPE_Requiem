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
        Debug.Log("슬라임 볼 히트");

        // maxHP와 HP의 비율을 계산
        float healthRatio = (float)HP / maxHP; // 예를 들어, maxHP가 100이고 HP가 90이면 0.9가 됩니다.
        Debug.Log(healthRatio);
        // currentColor를 조정
        // 1 - healthRatio를 하면, HP가 줄어들 때 값이 증가합니다. 예를 들어, healthRatio가 0.9면 1 - 0.9 = 0.1이 됩니다.
        currentColor = new Color(1f, healthRatio, healthRatio, 1f);
    }
}

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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownHorn : Enemy
{
    [Header("Down Horn")]
    public Enemy slime;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        slime.Hit(_damage, _hitDir, _audioSource);
    }
}

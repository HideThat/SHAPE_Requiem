using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.PixelArtPlatformer_Dungeon;

public class Lift : MonoBehaviour
{
    [SerializeField] Transform reber1;
    [SerializeField] Transform reber2;
    [SerializeField] Transform LiftObject;
    [SerializeField] Cainos_Switch cainosSwitch1;
    [SerializeField] Cainos_Switch cainosSwitch2;

    [SerializeField] BoxCollider2D switchCollider1;
    [SerializeField] BoxCollider2D switchCollider2;

    public enum LiftState
    {
        Move,
        Stop
    }

    public bool goLeft;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

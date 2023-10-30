using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] MeshRenderer backGround;
    [SerializeField] float scrollingSpeed;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 vector = new(backGround.material.mainTextureOffset.x, backGround.material.mainTextureOffset.y + scrollingSpeed * Time.deltaTime);
        backGround.material.mainTextureOffset = vector;
    }

    
}

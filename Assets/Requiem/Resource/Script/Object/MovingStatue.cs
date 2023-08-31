using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cainos.PixelArtPlatformer_Dungeon;

public class MovingStatue : MonoBehaviour
{
    public bool isActive = false;
    public Cainos_Switch cainos_Switch;
    public Transform movingStatue;
    public Vector2 destination;
    public float moveTime;
    public BoxCollider2D boxCollider2;
    public LayerMask layerMask;
    public Image KeyBlock;
    public TextMeshProUGUI keyText;

    public bool isMove = false;

    private Tween keyTween;
    private Color currentKeyColor;

    private void Start()
    {
        currentKeyColor = KeyBlock.color;
    }

    void Update()
    {
        if (isActive && !isMove)
        { 
            cainos_Switch.TurnOn();
            isMove = true; // 중복 실행을 방지
            MoveToDestination();
        }

        if (!isMove)
        {
            if (Physics2D.OverlapBox(transform.position, boxCollider2.size, transform.rotation.z, layerMask))
            {
                DOTween.Kill(keyTween);
                keyTween = KeyBlock.DOColor(currentKeyColor, 1.5f);
                keyText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    isActive = true;
                }
            }
            else
            {
                DOTween.Kill(keyTween);
                keyTween = KeyBlock.DOColor(Color.clear, 1.5f);
                keyText.gameObject.SetActive(false);
            }
        }
        else
        {
            KeyBlock.gameObject.SetActive(false);
            keyText.gameObject.SetActive(false);
        }
        
    }

    private void MoveToDestination()
    {
        // DOTween을 이용하여 destination으로 이동
        movingStatue.DOMove(destination, moveTime).OnComplete(() =>
        {
            
        });
    }

    public void AlreadyMove()
    {
        if (isMove)
        {
            cainos_Switch.TurnOn();
            movingStatue.position = destination;
        }
    }
}

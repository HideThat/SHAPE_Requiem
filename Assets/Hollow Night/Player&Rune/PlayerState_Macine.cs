using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewPlayerState;

public class PlayerState_Macine : Singleton<PlayerState_Macine>
{
    public MyPlayerState currentStateName;
    public float speed = 5.0f;
    public LayerMask platform; // 플랫폼 레이어 마스크
    public Transform wallCastTransform;
    public float wallCastDistance; // 땅과의 충돌 판정

    public P_State currentState;
    public Rigidbody2D rigid;
    public Animator animator;

    public bool isTouchingRightWall = false;
    public bool isTouchingLeftWall = false;

    private void Start()
    {
        currentState = new IdleState();
        currentState.Enter(this);
    }

    // Update is called once per frame
    private void Update()
    {
        currentState.Execute(this);
        currentStateName = currentState.playerState;

        if (Input.GetAxisRaw("Debug Horizontal") != 0)
        {
            currentState = new MoveState();
            currentState.Enter(this);
        }
        else if (currentState.playerState == MyPlayerState.Move)
        {
            currentState.Exit(this);
        }
    }

    private bool CheckWallCollision()
    {
        RaycastHit2D hitInfoRight = new();
        RaycastHit2D hitInfoLeft = new();

        if (transform.rotation.y == 0f)
            hitInfoRight = Physics2D.Raycast(wallCastTransform.position, Vector2.right, wallCastDistance, platform);
        else
            hitInfoLeft = Physics2D.Raycast(wallCastTransform.position, Vector2.left, wallCastDistance, platform);

        if (hitInfoRight.collider != null)
            isTouchingRightWall = true;
        else
            isTouchingRightWall = false;

        if (hitInfoLeft.collider != null)
            isTouchingLeftWall = true;
        else
            isTouchingLeftWall = false;

        // 오른쪽 또는 왼쪽으로 Ray가 어떤 오브젝트에 충돌했다면, 그 오브젝트는 벽일 가능성이 높습니다.
        if (hitInfoRight.collider != null || hitInfoLeft.collider != null)
            return true; // 벽과 충돌

        return false; // 벽과 미충돌
    }
}

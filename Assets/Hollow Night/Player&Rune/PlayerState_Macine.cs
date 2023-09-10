using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewPlayerState;

public class PlayerState_Macine : Singleton<PlayerState_Macine>
{
    public MyPlayerState currentStateName;
    public float speed = 5.0f;
    public LayerMask platform; // �÷��� ���̾� ����ũ
    public Transform wallCastTransform;
    public float wallCastDistance; // ������ �浹 ����

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

        // ������ �Ǵ� �������� Ray�� � ������Ʈ�� �浹�ߴٸ�, �� ������Ʈ�� ���� ���ɼ��� �����ϴ�.
        if (hitInfoRight.collider != null || hitInfoLeft.collider != null)
            return true; // ���� �浹

        return false; // ���� ���浹
    }
}

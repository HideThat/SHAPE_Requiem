using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum MyPlayerState
{
    None, Idle, Move
}

namespace NewPlayerState
{
    public class IdleState : P_State
    {
        public override void Enter(PlayerState_Macine _player)
        {
            _player.currentStateName = MyPlayerState.Idle;
        }

        public override void Execute(PlayerState_Macine _player)
        {
            _player.currentStateName = MyPlayerState.Idle;
        }

        public override void Exit(PlayerState_Macine _player)
        {

        }
    }

    public class MoveState : P_State
    {
        public override void Enter(PlayerState_Macine _player)
        {
            _player.currentStateName = MyPlayerState.Move;
            _player.animator.SetBool("IsMove", true);
        }


        public override void Execute(PlayerState_Macine _player)
        {
            float dir = Input.GetAxisRaw("Debug Horizontal");

            Rigidbody2D rigid = _player.rigid;
            Animator animator = _player.animator;
            bool isTouchingRightWall = _player.isTouchingRightWall;
            bool isTouchingLeftWall = _player.isTouchingLeftWall;

            if (dir > 0)
            {
                if (!isTouchingRightWall)
                {
                    rigid.velocity = new Vector2(dir * _player.speed, rigid.velocity.y);
                    _player.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                }
            }
            else if (dir < 0)
            {
                if (!isTouchingLeftWall)
                {
                    rigid.velocity = new Vector2(dir * _player.speed, rigid.velocity.y);
                    _player.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                }
            }
        }

        public override void Exit(PlayerState_Macine _player)
        {
            _player.animator.SetBool("IsMove", false);
            _player.rigid.velocity = new Vector2(0, _player.rigid.velocity.y);
            _player.currentStateName = MyPlayerState.None;
        }
    }
}


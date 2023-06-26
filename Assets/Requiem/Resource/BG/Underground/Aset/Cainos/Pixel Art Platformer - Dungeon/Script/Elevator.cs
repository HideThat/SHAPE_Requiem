using UnityEngine;

using Cainos.LucidEditor;
using Cainos.Common;

namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class Elevator : MonoBehaviour
    {
        // 엘리베이터 이동에 관한 매개변수
        [FoldoutGroup("Params")] public Vector2 lengthRange = new Vector2(2, 5); // 엘리베이터가 이동할 수 있는 길이 범위
        [FoldoutGroup("Params")] public float waitTime = 1.0f; // 엘리베이터가 길이 범위의 양 끝에서 대기할 시간
        [FoldoutGroup("Params")] public float moveSpeed = 3.0f; // 엘리베이터의 이동 속도
        [FoldoutGroup("Params")] public State startState = State.Up; // 엘리베이터의 시작 상태

        [FoldoutGroup("Reference")] public Rigidbody2D platform; // 플랫폼을 제어하는 Rigidbody2D 컴포넌트
        [FoldoutGroup("Reference")] public SpriteRenderer chainL; // 왼쪽 체인의 SpriteRenderer
        [FoldoutGroup("Reference")] public SpriteRenderer chainR; // 오른쪽 체인의 SpriteRenderer

        // 엘리베이터의 현재 길이
        [FoldoutGroup("Runtime"), ShowInInspector]
        public float Length
        {
            // get 함수와 set 함수가 각각 길이를 반환하고 설정합니다.
            // set 함수에서는 체인의 크기와 플랫폼의 위치를 조정하여 엘리베이터의 길이를 시각적으로 표현합니다.
            get { return length; }
            set
            {
                if (value < 0) value = 0.0f;
                this.length = value;

                platform.transform.localPosition = new Vector3(0.0f, -value, 0.0f);
                chainL.size = new Vector2(0.09375f, value + chainLengthOffset);
                chainR.size = new Vector2(0.09375f, value + chainLengthOffset);
            }
        }
        private float length;

        // 엘리베이터의 현재 상태
        [FoldoutGroup("Runtime"), ShowInInspector]
        public State CurState
        {
            // get 함수와 set 함수가 각각 현재 상태를 반환하고 설정합니다.
            get { return curState; }
            set
            {
                curState = value;
            }
        }
        private State curState;


        // 엘리베이터가 현재 대기 중인지 여부
        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsWaiting
        {
            // get 함수와 set 함수가 각각 대기 중 여부를 반환하고 설정합니다.
            // set 함수에서는 대기 시간 타이머를 0으로 초기화합니다.
            get { return isWaiting; }
            set
            {
                if (isWaiting == value) return;
                isWaiting = value;
                waitTimer = 0.0f;
            }
        }
        private bool isWaiting = false;


        // 나머지 변수들
        private float waitTimer; // 대기 시간을 측정하는 타이머
        private float curSpeed; // 현재 이동 속도
        private float targetLength; // 목표 길이
        private float chainLengthOffset; // 체인의 길이 오프셋
        private SecondOrderDynamics secondOrderDynamics = new SecondOrderDynamics(4.0f, 0.3f, -0.3f); // 움직임을 제어하는 SecondOrderDynamics 객체


        // Start 함수에서는 초기 설정을 합니다.
        private void Start()
        {
            chainLengthOffset = chainL.GetComponent<SpriteRenderer>().size.y + platform.transform.localPosition.y;

            curState = startState;
            Length = curState == State.Up ? lengthRange.y : lengthRange.x;
            targetLength = Length;

            secondOrderDynamics.Reset(targetLength);
        }

        // Update 함수에서는 엘리베이터의 움직임을 제어하고
        // 대기 시간이 끝났는지를 확인하거나
        // 움직임의 방향을 바꾸는 등의 동작을 수행합니다.
        private void Update()
        {
            if (IsWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > waitTime) IsWaiting = false;
                curSpeed = 0.0f;
            }
            else
            {
                if (curState == State.Up)
                {
                    curSpeed = -moveSpeed;
                    if (targetLength < lengthRange.x)
                    {
                        curState = State.Down;
                        IsWaiting = true;
                    }
                }
                else if (curState == State.Down)
                {
                    curSpeed = moveSpeed;
                    if (targetLength > lengthRange.y)
                    {
                        curState = State.Up;
                        IsWaiting = true;
                    }
                }
            }

            targetLength += curSpeed * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            Length = secondOrderDynamics.Update(targetLength, Time.fixedDeltaTime);
        }

        

        // 엘리베이터의 상태를 나타내는 열거형(enum)
        public enum State
        {
            Up, // 엘리베이터가 올라가고 있는 상태를 나타냅니다.
            Down // 엘리베이터가 내려가고 있는 상태를 나타냅니다.
        }
    }
}


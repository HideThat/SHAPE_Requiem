using UnityEngine;

using Cainos.LucidEditor;
using Cainos.Common;

namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class Elevator : MonoBehaviour
    {
        // ���������� �̵��� ���� �Ű�����
        [FoldoutGroup("Params")] public Vector2 lengthRange = new Vector2(2, 5); // ���������Ͱ� �̵��� �� �ִ� ���� ����
        [FoldoutGroup("Params")] public float waitTime = 1.0f; // ���������Ͱ� ���� ������ �� ������ ����� �ð�
        [FoldoutGroup("Params")] public float moveSpeed = 3.0f; // ������������ �̵� �ӵ�
        [FoldoutGroup("Params")] public State startState = State.Up; // ������������ ���� ����

        [FoldoutGroup("Reference")] public Rigidbody2D platform; // �÷����� �����ϴ� Rigidbody2D ������Ʈ
        [FoldoutGroup("Reference")] public SpriteRenderer chainL; // ���� ü���� SpriteRenderer
        [FoldoutGroup("Reference")] public SpriteRenderer chainR; // ������ ü���� SpriteRenderer

        // ������������ ���� ����
        [FoldoutGroup("Runtime"), ShowInInspector]
        public float Length
        {
            // get �Լ��� set �Լ��� ���� ���̸� ��ȯ�ϰ� �����մϴ�.
            // set �Լ������� ü���� ũ��� �÷����� ��ġ�� �����Ͽ� ������������ ���̸� �ð������� ǥ���մϴ�.
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

        // ������������ ���� ����
        [FoldoutGroup("Runtime"), ShowInInspector]
        public State CurState
        {
            // get �Լ��� set �Լ��� ���� ���� ���¸� ��ȯ�ϰ� �����մϴ�.
            get { return curState; }
            set
            {
                curState = value;
            }
        }
        private State curState;


        // ���������Ͱ� ���� ��� ������ ����
        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsWaiting
        {
            // get �Լ��� set �Լ��� ���� ��� �� ���θ� ��ȯ�ϰ� �����մϴ�.
            // set �Լ������� ��� �ð� Ÿ�̸Ӹ� 0���� �ʱ�ȭ�մϴ�.
            get { return isWaiting; }
            set
            {
                if (isWaiting == value) return;
                isWaiting = value;
                waitTimer = 0.0f;
            }
        }
        private bool isWaiting = false;


        // ������ ������
        private float waitTimer; // ��� �ð��� �����ϴ� Ÿ�̸�
        private float curSpeed; // ���� �̵� �ӵ�
        private float targetLength; // ��ǥ ����
        private float chainLengthOffset; // ü���� ���� ������
        private SecondOrderDynamics secondOrderDynamics = new SecondOrderDynamics(4.0f, 0.3f, -0.3f); // �������� �����ϴ� SecondOrderDynamics ��ü


        // Start �Լ������� �ʱ� ������ �մϴ�.
        private void Start()
        {
            chainLengthOffset = chainL.GetComponent<SpriteRenderer>().size.y + platform.transform.localPosition.y;

            curState = startState;
            Length = curState == State.Up ? lengthRange.y : lengthRange.x;
            targetLength = Length;

            secondOrderDynamics.Reset(targetLength);
        }

        // Update �Լ������� ������������ �������� �����ϰ�
        // ��� �ð��� ���������� Ȯ���ϰų�
        // �������� ������ �ٲٴ� ���� ������ �����մϴ�.
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

        

        // ������������ ���¸� ��Ÿ���� ������(enum)
        public enum State
        {
            Up, // ���������Ͱ� �ö󰡰� �ִ� ���¸� ��Ÿ���ϴ�.
            Down // ���������Ͱ� �������� �ִ� ���¸� ��Ÿ���ϴ�.
        }
    }
}


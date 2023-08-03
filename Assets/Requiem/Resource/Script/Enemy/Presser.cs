using System.Collections;
using UnityEngine;

public class Presser : MonoBehaviour
{
    public float fallTime = 1f; // ������Ʈ�� �������� �� �ɸ��� �ð�
    public float waitTime = 2f; // ������Ʈ�� �ٴڿ� �ӹ��� �ð�
    public float returnTime = 1f; // ������Ʈ�� ���� ��ġ�� ���ư��� �� �ɸ��� �ð�
    public float restTime = 1f; // ������Ʈ�� ���� ��ġ���� ����ϴ� �ð�
    public Vector3 fallPosition; // ������Ʈ�� �������� ��ġ

    private Vector3 originalPosition; // ������Ʈ�� ���� ��ġ

    void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(FallAndReturn());
    }

    IEnumerator FallAndReturn()
    {
        while (true)
        {
            // ������Ʈ�� ���� ��ġ���� ����մϴ�
            yield return new WaitForSeconds(restTime);

            // ������Ʈ�� ����߸��ϴ�
            float elapsedTime = 0;
            while (elapsedTime < fallTime)
            {
                transform.position = Vector3.Lerp(originalPosition, fallPosition, (elapsedTime / fallTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ������Ʈ�� �ٴڿ� �ӹ������� �մϴ�
            yield return new WaitForSeconds(waitTime);

            // ������Ʈ�� ���� ��ġ�� ���������ϴ�
            elapsedTime = 0;
            while (elapsedTime < returnTime)
            {
                transform.position = Vector3.Lerp(fallPosition, originalPosition, (elapsedTime / returnTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}

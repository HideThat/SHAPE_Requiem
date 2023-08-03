using System.Collections;
using UnityEngine;

public class Presser : MonoBehaviour
{
    public float fallTime = 1f; // 오브젝트가 떨어지는 데 걸리는 시간
    public float waitTime = 2f; // 오브젝트가 바닥에 머무는 시간
    public float returnTime = 1f; // 오브젝트가 원래 위치로 돌아가는 데 걸리는 시간
    public float restTime = 1f; // 오브젝트가 원래 위치에서 대기하는 시간
    public Vector3 fallPosition; // 오브젝트가 떨어지는 위치

    private Vector3 originalPosition; // 오브젝트의 원래 위치

    void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(FallAndReturn());
    }

    IEnumerator FallAndReturn()
    {
        while (true)
        {
            // 오브젝트가 원래 위치에서 대기합니다
            yield return new WaitForSeconds(restTime);

            // 오브젝트를 떨어뜨립니다
            float elapsedTime = 0;
            while (elapsedTime < fallTime)
            {
                transform.position = Vector3.Lerp(originalPosition, fallPosition, (elapsedTime / fallTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 오브젝트가 바닥에 머무르도록 합니다
            yield return new WaitForSeconds(waitTime);

            // 오브젝트를 원래 위치로 돌려놓습니다
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

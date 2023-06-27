using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivAreaManager : MonoBehaviour
{
    [SerializeField] DivArea[] divAreaArr;



    private static DivAreaManager instance;
    public static DivAreaManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DivAreaManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("FadeManager");
                    instance = obj.AddComponent<DivAreaManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        divAreaArr = new DivArea[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            divAreaArr[i] = transform.GetChild(i).GetComponent<DivArea>();
        }
    }

    public void DivAreaActive(bool isActive)
    {
        for (int i = 0; i < divAreaArr.Length; i++)
        {
            divAreaArr[i].enabled = isActive;
        }
    }
}

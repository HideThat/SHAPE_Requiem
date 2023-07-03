using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingFloorManager : MonoBehaviour
{
    public static RisingFloorManager Instance { get; private set; }

    [SerializeField] RisingFloor2[] risingFloor2s;
    [SerializeField] WallTrigger[] wallTriggers;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        risingFloor2s = FindObjectsOfType<RisingFloor2>();
        wallTriggers = FindObjectsOfType<WallTrigger>();
    }

    public void ResetAllRisingFloors()
    {
        foreach (RisingFloor2 risingFloor2s in risingFloor2s)
        {
            risingFloor2s.ResetFloor();
        }

        foreach (WallTrigger wallTriggers in wallTriggers)
        {
            wallTriggers.ResetTrigger();
        }
    }
}


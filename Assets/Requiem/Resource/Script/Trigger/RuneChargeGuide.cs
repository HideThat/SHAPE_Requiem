using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneChargeGuide : MonoBehaviour
{
    bool isActive;
    [SerializeField] GameObject runeChargeGuide;

    private void Start()
    {
        runeChargeGuide.SetActive(false);
    }

}

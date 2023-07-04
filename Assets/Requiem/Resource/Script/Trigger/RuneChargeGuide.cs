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

    void Update()
    {
        if (!isActive && RuneData.RuneBattery <= 0)
        {
            runeChargeGuide.SetActive(true);
            isActive = true;
        }

        if (isActive && runeChargeGuide.active && RuneData.RuneBattery > 0)
        {
            Destroy(runeChargeGuide);
            Destroy(gameObject);
        }
    }
}

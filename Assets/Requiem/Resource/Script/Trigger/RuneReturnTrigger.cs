using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneReturnTrigger : MonoBehaviour
{
	bool isActive = false;
    [SerializeField] RuneManager runeManager;
    [SerializeField] RuneControllerGPT runeController;
    [SerializeField] GameObject runeReturnGuide;

    private void Start()
    {
        runeReturnGuide.SetActive(false);
    }

    private void Update()
    {
        if (!isActive && runeManager.fullChargeCount != 0 && runeController.isShoot)
        {
            Invoke("GuideOpen", 3f);
            isActive = true;
        }

        if (runeReturnGuide.active && Input.GetMouseButton(1))
        {
            Destroy(runeReturnGuide);
            Destroy(gameObject);
        }
    }

    void GuideOpen()
    {
        runeReturnGuide.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using Cinemachine;
using DG.Tweening;

public class DivArea : AreaData
{
    Tilemap tilemap;
    [SerializeField] Collider2D cameraArea;
    [SerializeField] float lensSize;
    CinemachineVirtualCamera mainCM;
    Tween myTween;

    [SerializeField] public bool PlayerIn;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        cameraArea = transform.GetChild(0).GetComponent<Collider2D>();
        cameraArea.transform.parent = null;
        mainCM = DataController.MainCM;


        tilemap.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update()
    {
        if (PlayerIn)
        {
            ChangeCameraArea();
            if (myTween == null || !myTween.IsActive() || myTween.IsComplete())
            {
                myTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, lensSize, 4f);
            }
        }
        else if (myTween != null)
        {
            myTween.Kill();
            myTween = null;
        }
    }

    


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerData.PlayerIsDead)
        {
            PlayerIn = true;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerData.PlayerIsDead)
        {
            PlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerData.PlayerIsDead)
        {
            PlayerIn = false;
            if (myTween != null)
            {
                myTween.Kill();
                myTween = null;
            }
        }
    }

    public void ChangeCameraArea()
    {
        mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea;
    }
}

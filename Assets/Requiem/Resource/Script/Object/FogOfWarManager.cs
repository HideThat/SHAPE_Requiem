using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class FogOfWarManager : MonoBehaviour
{
    [SerializeField]
    private int mapWidth = 100; // 맵의 가로 크기
    [SerializeField]
    private int mapHeight = 100; // 맵의 세로 크기

    private Texture2D fogTexture; // 안개를 표현하는 텍스처
    private Color[] fogColors; // 텍스처의 각 픽셀의 색상을 저장할 배열

    private void Awake()
    {
        // 안개 텍스처 생성
        fogTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false);
        fogColors = new Color[fogTexture.width * fogTexture.height];

        // 텍스처의 모든 픽셀을 검정색으로 설정
        for (int i = 0; i < fogColors.Length; i++)
        {
            fogColors[i] = Color.black;
        }

        fogTexture.SetPixels(fogColors);
        fogTexture.Apply();

        // 생성한 안개 텍스처를 렌더러의 메인 텍스처로 설정
        GetComponent<Renderer>().material.mainTexture = fogTexture;
    }
}
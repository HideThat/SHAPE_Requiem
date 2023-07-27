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

    private void Update()
    {
        // 모든 Light2D 컴포넌트를 찾는다.
        Light2D[] lights = FindObjectsOfType<Light2D>();

        // 각각의 Light에 대해
        foreach (var light in lights)
        {
            // 라이트의 위치를 안개 텍스처의 좌표계로 변환
            Vector3 lightPos = Camera.main.WorldToScreenPoint(light.transform.position);

            // 라이트 주변의 안개를 제거
            for (int x = (int)(lightPos.x - light.pointLightOuterRadius); x < lightPos.x + light.pointLightOuterRadius; x++)
            {
                for (int y = (int)(lightPos.y - light.pointLightOuterRadius); y < lightPos.y + light.pointLightOuterRadius; y++)
                {
                    // 픽셀이 라이트의 범위 내에 있는지 확인
                    if (Vector2.Distance(new Vector2(x, y), lightPos) < light.pointLightOuterRadius)
                    {
                        fogColors[y * fogTexture.width + x] = Color.clear; // 안개를 제거(픽셀을 투명하게 만듦)
                    }
                }
            }
        }

        // 변경사항을 텍스처에 적용
        fogTexture.SetPixels(fogColors);
        fogTexture.Apply();
    }
}




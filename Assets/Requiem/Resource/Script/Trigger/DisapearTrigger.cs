using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class DisapearTrigger : MonoBehaviour
{

    [SerializeField] Tilemap m_tileMap;
    [SerializeField] float m_changeTime;

    float m_colorAlpha = 1f;
    Color m_color = new Color(1f, 1f, 1f, 1f);

    public bool playerIn = false;
    public bool runeIn = false;

    bool m_isActive
    {
        get { return playerIn || runeIn; }
    }

    void Start()
    {
        m_tileMap = GetComponent<Tilemap>();
    }

    void Update()
    {
        ColorChange();
        m_color = new Color(1f, 1f, 1f, m_colorAlpha);
        m_tileMap.color = m_color;


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIn = true;
        }

        if (collision.CompareTag("Rune"))
        {
            runeIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIn = false;
        }
        
        if (collision.CompareTag("Rune"))
        {
            runeIn = false;
        }
    }

    void ColorChange()
    {
        if (m_isActive)
        {
            DOTween.To(() => m_colorAlpha, x => m_colorAlpha = x, 0f, m_changeTime);
        }
        else
        {
            DOTween.To(() => m_colorAlpha, x => m_colorAlpha = x, 1f, m_changeTime);
        }
    }
}

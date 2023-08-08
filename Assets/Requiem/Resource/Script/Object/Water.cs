using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] RuneManager rune;


    private void Start()
    {
        rune = RuneData.RuneObj.GetComponent<RuneManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case (int)LayerName.Rune:
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case (int)LayerName.Rune:
                break;
            default:
                break;
        }
    }
}

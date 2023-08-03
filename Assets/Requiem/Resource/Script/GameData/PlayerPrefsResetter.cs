using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsResetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResetPlayerPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

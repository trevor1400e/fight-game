using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{

    public int level;

    public bool firstTime;
    // Start is called before the first frame update
    void Start()
    {
        level = ES3.Load("level", 1);
        firstTime = ES3.Load("firstTime", true);
        Debug.Log("Level is: " + level.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

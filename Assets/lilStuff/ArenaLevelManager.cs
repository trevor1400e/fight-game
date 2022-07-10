using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArenaLevelManager : MonoBehaviour
{

    public int level;

    public GameObject soldierEnemy;
    public GameObject spawn1;
    
    //soldier, archer, axe guy, spear guy, animals, bosses?
    
    [SerializeField]
    string[] enemies = {"sol", "bow", "knight"};
    // Start is called before the first frame update
    void Start()
    {
        level = ES3.Load("level", 1);
        EnemyArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnemyArray()
    {
        if (level == 1)
        {
            string[] newEnemies = {"sol", "sol", "sol", "sol", "sol", "sol", "sol", "sol", "sol","sol", "sol", "sol", "sol", "sol", "sol", "sol", "sol", "sol"};
            enemies = newEnemies;
        }else{
            string[] newEnemies = {"sol"};
            enemies = newEnemies;
        }

        foreach (string enemy in enemies)
        {
            Instantiate(soldierEnemy, spawn1.transform.position, spawn1.transform.rotation);
            Debug.Log(enemy);
        }
    }
}

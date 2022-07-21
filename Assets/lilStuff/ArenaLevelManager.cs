using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ArenaLevelManager : MonoBehaviour
{

    public int level;
    public int enemyCount;

    public GameObject EnemySword;
    public GameObject EnemyAxe;
    public GameObject EnemySpear;
    public GameObject EnemyBow;

    public GameObject spawn1;
    public GameObject spawn2;

    public GameObject complete;
    
    //soldier, archer, axe guy, spear guy, animals, bosses?
    
    [SerializeField]
    string[] enemies = {"fillershit"};
    // Start is called before the first frame update
    void Start()
    {
        level = ES3.Load("level", 1);
        enemyCount = 0;
        EnemyArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount == 0 && complete.activeSelf == false)
        {
            Debug.Log("durr copmelted");
            complete.SetActive(true);
        }
    }
    

    void EnemyArray()
    {
        
        switch (level) 
        {
            case 1:
                //gotta be a better way to do this c# pls
                string[] newEnemies = {"sol"};
                enemies = newEnemies;
                break;
            case 2:
                string[] newEnemies2 = {"sol", "axe", "bow", "spear"};
                enemies = newEnemies2;
                break;
            case 3:
                Console.WriteLine("level 3");
                break;

        }
        

        foreach (string enemy in enemies)
        {
            enemyCount += 1;
            switch (enemy) 
            {
                case "sol":
                    Instantiate(EnemySword, spawn1.transform.position, spawn1.transform.rotation);
                    break;
                case "axe":
                    Instantiate(EnemyAxe, spawn2.transform.position, spawn2.transform.rotation);
                    break;
                case "spear":
                    Instantiate(EnemySpear, spawn2.transform.position, spawn2.transform.rotation);
                    break;
                case "bow":
                    Instantiate(EnemyBow, spawn2.transform.position, spawn2.transform.rotation);
                    break;

            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class hostileEncounter : MonoBehaviour {

//    public string level;    // combat level to load
    public List<EnemyData> enemies = new List<EnemyData>(); // list of enemies
    public bool isActive = true;


    // Use this for initialization
    void Start()
    {
        if(GameManager.control.enemyEncounter != "" && GameObject.Find(GameManager.control.enemyEncounter).name == gameObject.name)
        {
            if (GameManager.control.mostRecentBattleWon == true)
            {
                GameManager.control.mostRecentBattleWon = false;
                gameObject.SetActive(false);
            }
        }

    }

    public void startBattle()
    {
        GameManager.control.enemies = enemies;
        GameManager.control.enemyEncounter = gameObject.name;
        GameManager.control.currentMapScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Battle Screen");  //loads the level
    }

}

[Serializable]
public class EnemyData
{
    public string name;
    public int level;
    public int gridPos;
}
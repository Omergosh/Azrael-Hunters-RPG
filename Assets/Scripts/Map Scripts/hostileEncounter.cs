using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class hostileEncounter : MonoBehaviour {

//    public string level;    // Combat level to load
    public List<EnemyData> enemies = new List<EnemyData>(); // List of enemies


    // Use this for initialization
    void Start()
    {
        if (GameManager.control.overworldEnemies.Contains(gameObject.name))
        {
                gameObject.SetActive(false);
        }
    }

    public void startBattle()
    {
        GameManager.control.enemies = enemies;
        GameManager.control.enemyEncounter = gameObject.name;
        GameManager.control.overworldEnemies.Add(gameObject.name);
        GameManager.control.currentMapScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Battle Screen");  // Loads the level
    }

}

[Serializable]
public class EnemyData
{
    public string name;
    public int level;
    public int gridPos;
}
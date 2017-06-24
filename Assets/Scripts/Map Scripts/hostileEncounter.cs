using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class hostileEncounter : MonoBehaviour {

    private GameObject player;
//    public string level;    // combat level to load
    public List<EnemyData> enemies = new List<EnemyData>(); // list of enemies

    // Use this for initialization
    void Start()
    {

    }

    public void startBattle()
    {
        GameManager.control.enemies = enemies;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle Screen");  //loads the level
    }

}

[Serializable]
public class EnemyData
{
    public string name;
    public int level;
    public int gridPos;
}
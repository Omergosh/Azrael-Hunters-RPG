using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hostileEncounter : MonoBehaviour {

    private GameObject player;
//    public string level;    // combat level to load
    public List<string> enemies = new List<string>(); // list of enemies

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

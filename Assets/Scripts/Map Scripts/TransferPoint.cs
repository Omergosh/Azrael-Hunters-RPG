using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class TransferPoint : MonoBehaviour {

    public GameObject player;
    public string level;    // level to load
    public Vector2 pos;
    public Vector2 directionV;  //direction of player
    public bool respawnEnemies;

    // Use this for initialization
    void Start () {
		
	}

	public void transfer () {
        GameManager.control.pos = pos;
        GameManager.control.dir = directionV;
        if (respawnEnemies)
        {
            GameManager.control.overworldEnemies.Clear();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);  //loads the level
	}

}

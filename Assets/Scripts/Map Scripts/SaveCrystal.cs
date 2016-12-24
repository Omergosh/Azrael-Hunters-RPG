using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCrystal : MonoBehaviour {

    GameObject canvas;
    GameObject UI_SaveCrystal;
    GameObject player;

    void Start ()
    {
        canvas = GameObject.Find("Canvas");   //finding the Canvas gameObject
        UI_SaveCrystal = canvas.transform.Find("UI_SaveCrystal").gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
    }
	

    public void save(int num)
    {
        GameManager.control.Save(num);
        UI_SaveCrystal.SetActive(false);
        PlayerControlsOverworld playerScript = player.GetComponent<PlayerControlsOverworld>();
        playerScript.interacting = false;
        playerScript.canMove = true;
    }
}

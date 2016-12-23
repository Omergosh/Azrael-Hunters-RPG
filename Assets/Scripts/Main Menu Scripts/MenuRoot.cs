using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoot : MonoBehaviour {
    public GameObject Title;
    private GameObject NewGame;
    private GameObject LoadGame;
    private GameObject Options;
    private GameObject Credits_Button;
    private GameObject Exit;
    private GameObject childObject;
    private GameObject Back;

    public void Start()
    {
        Title = GameObject.Find("Canvas/Menu Root/Title");
        NewGame = GameObject.Find("Canvas/Menu Root/New Game");
        LoadGame = GameObject.Find("Canvas/Menu Root/Load Game");
        Options = GameObject.Find("Canvas/Menu Root/Options");
        Credits_Button = GameObject.Find("Canvas/Menu Root/Credits_Button");
        Exit = GameObject.Find("Canvas/Menu Root/Exit");
        Back = GameObject.Find("Canvas/Menu Root/Back");
    }
    public void showMenu()  //hides the previous menu and shows the main menu
    {
        
        foreach (Transform child in transform)
        {
            if (child.gameObject == Title || child.gameObject == NewGame || child.gameObject == LoadGame || child.gameObject == Options || child.gameObject == Credits_Button || child.gameObject == Exit)    //yes I know its sloppy
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (childObject != null)
        {
            childObject.SetActive(false);
            Back.SetActive(false);
        }
    }

    public void hideMenu(string nameOfGameObject)   //goes into a submenu
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        childObject = GameObject.Find(nameOfGameObject);
        childObject.SetActive(true);
        
        Back.SetActive(true);
    }

    public void hide(string nameOfGameObject)   //hides one object
    {
        childObject = GameObject.Find(nameOfGameObject);
        childObject.SetActive(false);
    }

    public void show(string nameOfGameObject)   //shows one object
    {
        childObject = GameObject.Find(nameOfGameObject);
        childObject.SetActive(true);
    }
}

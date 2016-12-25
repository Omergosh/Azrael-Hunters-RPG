using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoot : MonoBehaviour {

    private GameObject childObject;
    private Transform Back;

    public void Start()
    {
        Back = transform.Find("Back");
    }

    public void showMainMenu()  //hides the previous menu and shows the main menu
    {
        
        foreach (Transform child in transform)
        {
            if(child.gameObject == transform.Find("Menu Root").gameObject)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (childObject != null)    //closing the previous submenu
        {
            childObject.SetActive(false);
            Back.gameObject.SetActive(false);
        }
    }

    public void showMenu(string nameOfGameObject)   //goes into a submenu by the name of the string passed
    {

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        Transform childObject = transform.Find(nameOfGameObject);
        childObject.gameObject.SetActive(true);
        
        Back.gameObject.SetActive(true);
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

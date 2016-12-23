using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoot : MonoBehaviour {
    public GameObject Back;
    public GameObject childObject;
  

    public void showMenu()  //hides the previous menu and shows the main menu
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
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
        Back = GameObject.Find("Canvas/Menu Root/Back");
        Back.SetActive(true);
    }
}

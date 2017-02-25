using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour {

    private GameObject canvasGameObject;
    public void onClick()
    {
        canvasGameObject = GameObject.Find("Canvas");
        MenuRoot menuScript = canvasGameObject.GetComponent<MenuRoot>();
        menuScript.showMainMenu();
    }
}

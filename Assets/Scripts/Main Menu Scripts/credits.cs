﻿using UnityEngine;
using System.Collections;

public class credits : MonoBehaviour {

    private GameObject canvasGameObject;
    public void onClick()
    {
        canvasGameObject = GameObject.Find("Canvas");
        MenuRoot menuScript = canvasGameObject.GetComponent<MenuRoot>();
        menuScript.showMenu("Credits Root");
    }
}

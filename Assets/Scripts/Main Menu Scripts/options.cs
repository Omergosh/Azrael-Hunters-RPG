using UnityEngine;
using System.Collections;

public class options : MonoBehaviour {

    private GameObject MenuRoot;
    public void onClick()
    {
        MenuRoot = GameObject.Find("Canvas");
        MenuRoot menuScript = MenuRoot.GetComponent<MenuRoot>();
        menuScript.showMenu("Options Root");
    }
}

using UnityEngine;
using System.Collections;

public class options : MonoBehaviour {

    private GameObject MenuRoot;
    public void onClick()
    {
        MenuRoot = GameObject.Find("Menu Root");
        MenuRoot menuScript = MenuRoot.GetComponent<MenuRoot>();
        menuScript.hideMenu("Canvas/Menu Root/Options Root");
    }
}

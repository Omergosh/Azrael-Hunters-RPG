using UnityEngine;
using System.Collections;

public class loadGame : MonoBehaviour {

    private GameObject MenuRoot;
    public void onClick()
    {
        MenuRoot = GameObject.Find("Menu Root");
        MenuRoot menuScript = MenuRoot.GetComponent<MenuRoot>();
        menuScript.hideMenu("Canvas/Menu Root/Load Game Root");
    }
}

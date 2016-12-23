using UnityEngine;
using System.Collections;

public class loadGame : MonoBehaviour {

    private GameObject MenuRoot;
    public void onClick()
    {
        MenuRoot = GameObject.Find("Canvas");
        MenuRoot menuScript = MenuRoot.GetComponent<MenuRoot>();
        menuScript.showMenu("Load Game Root");
    }
}

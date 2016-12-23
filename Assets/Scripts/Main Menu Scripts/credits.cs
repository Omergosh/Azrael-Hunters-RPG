using UnityEngine;
using System.Collections;

public class credits : MonoBehaviour {
    private GameObject MenuRoot;
    public void onClick()
    {
        MenuRoot = GameObject.Find("Menu Root");
        MenuRoot menuScript = MenuRoot.GetComponent<MenuRoot>();
        menuScript.hideMenu("Canvas/Menu Root/Credits");
    }
}

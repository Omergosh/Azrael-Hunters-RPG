using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleScreen : MonoBehaviour {
	
    public string mainMenu;

	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(mainMenu);
        }
	}
}

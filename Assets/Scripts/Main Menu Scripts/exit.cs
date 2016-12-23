using UnityEngine;
using System.Collections;

public class exit : MonoBehaviour {

    public void onClick()
    {
        Application.Quit();  //simply exiting the game for now
        Debug.Log("Game exited!");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class newGame : MonoBehaviour {

    public void onClick(string level)
    {
        GameManager.control.Reset();
        SceneManager.LoadScene(level);  //simply loading the map for now
    }
}

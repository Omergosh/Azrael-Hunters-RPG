using UnityEngine;

public class TransferPoint : MonoBehaviour {

    public GameObject player;
    public string level;    // Level to load
    public Vector2 pos;
    public Vector2 directionV;  // Direction of player
    public bool respawnEnemies;

    // Use this for initialization
    void Start () {
		
	}

	public void transfer () {
        GameManager.control.pos = pos;
        GameManager.control.dir = directionV;
        if (respawnEnemies)
        {
            GameManager.control.overworldEnemies.Clear();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);  // Loads the level
	}

}

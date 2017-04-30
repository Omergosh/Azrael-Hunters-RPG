using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager control;

    public float health;
    public float exp;
    public string scene;
    public Vector2 pos;
    public Vector2 dir;
    GameObject player;

    public List<string> enemies;    // list of enemies for battle

	void Awake ()
    {
		if(control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if(control != this)
        {
            Destroy(gameObject);
        }
	}

    public void Reset()
    {
        scene = "";
        pos = Vector2.zero;
        dir = Vector2.zero;
    }

    public void Save(int saveNumber)    //saves file number "saveNumber"
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat");

        PlayerData data = new PlayerData();
        data.health = health;
        data.exp = exp;
        data.scene = SceneManager.GetActiveScene().name;

        player = GameObject.FindGameObjectWithTag("Player");
        pos = player.GetComponent<PlayerControlsOverworld>().pos;
        data.posx = (int)pos.x;
        data.posy = (int)pos.y;
        dir = player.GetComponent<PlayerControlsOverworld>().directionV;
        data.dirx = dir.x;
        data.diry = dir.y;
        scene = data.scene;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Data saved");
    }

    public void Load(int saveNumber)    //loads file number "saveNumber"
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat"))
        {
            Debug.Log("Loading data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            health = data.health;
            exp = data.exp;
            pos = new Vector2(data.posx, data.posy);
            dir = new Vector2(data.dirx, data.diry);
            SceneManager.LoadScene(data.scene);
        }
        else
        {
            Debug.Log("No data to load");
        }
    }
}

[Serializable]  //allows for class to be written to a file
class PlayerData
{
    public float health;
    public float exp;
    public string scene;
    public int posx;
    public int posy;
    public float dirx;
    public float diry;

}
